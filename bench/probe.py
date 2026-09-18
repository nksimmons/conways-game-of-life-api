#!/usr/bin/env python3
"""Boundary and load harness for the Game of Life API.

The historical HTTP measurements in README.md can be rerun with these subcommands.
The design document states them as facts, so they need to be reproducible rather
than remembered. Standard library only, so there is nothing to install.

Start the API in Release first, since Debug timings are several times slower and
not what the document reports:

    dotnet run -c Release --project src/GameOfLife.Api --no-launch-profile

Then, for example:

    python3 bench/probe.py validate --base http://localhost:5000

`validate` exits non-zero if any probe returns an unexpected status, so it can
gate a pipeline. The timing subcommands always exit zero; they report, and a
human decides whether the numbers are acceptable.
"""
from __future__ import annotations

import argparse
import json
import random
import statistics
import sys
import time
import urllib.error
import urllib.request
from concurrent.futures import ThreadPoolExecutor

CAP = 256
"""Board dimension cap from GameOfLifeOptions. Probes are built relative to it."""


class Client:
    def __init__(self, base: str) -> None:
        self.base = base.rstrip("/")

    def request(self, method, path, body=None, headers=None, raw=False):
        """Returns (status, body, headers, seconds). Never raises for an HTTP error status."""
        data = None
        hdrs = dict(headers or {})
        if body is not None:
            data = body.encode() if raw else json.dumps(body).encode()
            hdrs.setdefault("Content-Type", "application/json")
        req = urllib.request.Request(self.base + path, data=data, headers=hdrs, method=method)
        started = time.perf_counter()
        try:
            with urllib.request.urlopen(req) as resp:
                return resp.status, resp.read(), resp.headers, time.perf_counter() - started
        except urllib.error.HTTPError as e:
            return e.code, e.read(), e.headers, time.perf_counter() - started
        except Exception as e:
            return f"ERR:{type(e).__name__}", b"", None, time.perf_counter() - started

    def get(self, path, headers=None):
        return self.request("GET", path, headers=headers)

    def create(self, cells) -> str:
        status, payload, _, _ = self.request("POST", "/api/v1/boards", {"cells": cells})
        if status != 201:
            raise SystemExit(f"expected 201 from board creation, got {status}: {payload[:200]!r}")
        return json.loads(payload)["boardId"]


def grid(fill, seed=None):
    if fill == "alive":
        return [[1] * CAP for _ in range(CAP)]
    if fill == "dead":
        return [[0] * CAP for _ in range(CAP)]
    if fill == "checker":
        return [[(r + c) % 2 for c in range(CAP)] for r in range(CAP)]
    rng = random.Random(seed)
    density = float(fill)
    return [[1 if rng.random() < density else 0 for _ in range(CAP)] for _ in range(CAP)]


# --------------------------------------------------------------------- validate
def validate(client: Client) -> int:
    """Hostile and malformed payloads. Backs README.md §10.5."""
    over = [0] * (CAP + 1)
    body_cases = [
        ("missing cells key", {}, 400),
        ("cells null", {"cells": None}, 400),
        ("cells empty array", {"cells": []}, 400),
        ("cells with empty row", {"cells": [[]]}, 400),
        ("ragged rows", {"cells": [[0, 1], [0]]}, 400),
        ("cell value 2", {"cells": [[0, 2]]}, 400),
        ("negative cell value", {"cells": [[-1]]}, 400),
        ("trailing null row", {"cells": [[0, 1], None]}, 400),
        ("leading null row", {"cells": [None, [0, 1]]}, 400),
        ("string cell", {"cells": [["a"]]}, 400),
        ("bool cell", {"cells": [[True]]}, 400),
        ("float cell", {"cells": [[0.5]]}, 400),
        ("cells is a string", {"cells": "nope"}, 400),
        ("cells is one-dimensional", {"cells": [0, 1, 0]}, 400),
        ("int64 overflow in a cell", {"cells": [[99999999999999]]}, 400),
        ("width one over cap", {"cells": [over]}, 400),
        ("height one over cap", {"cells": [[0] for _ in range(CAP + 1)]}, 400),
        (f"exactly at cap {CAP}x{CAP}", {"cells": [[0] * CAP for _ in range(CAP)]}, 201),
        ("minimum 1x1", {"cells": [[1]]}, 201),
    ]
    raw_cases = [
        ("not json at all", "this is not json", None, 400),
        ("truncated json", '{"cells":[[0,1]', None, 400),
        ("json nesting bomb", "[" * 200 + "]" * 200, None, 400),
        ("wrong content-type", '{"cells":[[1]]}', {"Content-Type": "text/plain"}, 415),
        ("missing content-type", '{"cells":[[1]]}', {"Content-Type": ""}, 415),
        # Must be refused before model binding, by RequestSizeLimit on the action.
        ("oversized body ~1.4MB", '{"cells":[[' + ",".join(["0"] * 700000) + "]]}", None, 413),
    ]

    print(f"  {'probe':34}{'expect':>7}{'actual':>8}   problem+json")
    print("  " + "-" * 66)
    mismatches = []

    def report(name, expected, status, headers):
        ct = (headers.get("Content-Type") or "") if headers else ""
        ok = status == expected
        if not ok:
            mismatches.append(name)
        shape = "yes" if "problem+json" in ct else "-"
        print(f"  {name:34}{expected:>7}{str(status):>8}   {shape:<4}{'' if ok else '  <-- MISMATCH'}")

    for name, body, expected in body_cases:
        status, _, headers, _ = client.request("POST", "/api/v1/boards", body)
        report(name, expected, status, headers)

    for name, raw, hdrs, expected in raw_cases:
        status, _, headers, _ = client.request("POST", "/api/v1/boards", raw, headers=hdrs, raw=True)
        report(name, expected, status, headers)

    board = client.create([[0, 0, 0], [1, 1, 1], [0, 0, 0]])
    unknown = "00000000-0000-0000-0000-000000000000"
    for name, path, expected in [
        ("n = 0", f"/api/v1/boards/{board}/generations/0", 200),
        ("n = 1000 (cap)", f"/api/v1/boards/{board}/generations/1000", 200),
        ("n = 1001", f"/api/v1/boards/{board}/generations/1001", 400),
        ("n = -1", f"/api/v1/boards/{board}/generations/-1", 400),
        ("n = int overflow", f"/api/v1/boards/{board}/generations/99999999999999", 404),
        ("n not numeric", f"/api/v1/boards/{board}/generations/abc", 404),
        ("unknown board", f"/api/v1/boards/{unknown}/generations/1", 404),
        ("malformed guid", "/api/v1/boards/not-a-guid/generations/1", 404),
        ("unknown board, final", f"/api/v1/boards/{unknown}/final", 404),
    ]:
        status, _, headers, _ = client.get(path)
        report(name, expected, status, headers)

    total = len(body_cases) + len(raw_cases) + 9
    print(f"\n  {total - len(mismatches)}/{total} probes returned the intended status.")
    if mismatches:
        print(f"  MISMATCHES: {mismatches}")
    return len(mismatches)


# ------------------------------------------------------------------------- load
def load(client: Client) -> int:
    """Cost by seed shape at the caps. Backs "Which input is actually the worst one"."""
    seeds = [
        ("every cell alive", grid("alive")),
        ("every cell dead", grid("dead")),
        ("checkerboard", grid("checker")),
        ("random, 30% alive", grid("0.3", seed=1)),
        ("random, 50% alive", grid("0.5", seed=1)),
    ]
    print(f"  {'seed (' + str(CAP) + 'x' + str(CAP) + ')':22}{'gen/1000':>10}{'final':>9}   outcome")
    print("  " + "-" * 66)
    for name, cells in seeds:
        board = client.create(cells)
        client.get(f"/api/v1/boards/{board}/generations/5")  # warm the JIT
        _, _, _, gen_s = client.get(f"/api/v1/boards/{board}/generations/1000")
        status, payload, _, final_s = client.get(f"/api/v1/boards/{board}/final")
        if status == 200:
            d = json.loads(payload)
            outcome = f"stabilises at gen {d['stabilizedAtGeneration']}, period {d['period']}"
        else:
            outcome = f"{status}, no cycle within budget"
        print(f"  {name:22}{gen_s:9.2f}s{final_s:8.2f}s   {outcome}")
    return 0


# ------------------------------------------------------------------------ cache
def cache(client: Client) -> int:
    """Cost of a conditional request. Backs README.md §5.5."""
    board = client.create(grid("0.5", seed=11))
    base = f"/api/v1/boards/{board}/generations/1000"
    for _ in range(3):
        client.get(f"/api/v1/boards/{board}/generations/20")  # warm the JIT

    _, _, headers, _ = client.get(base)
    etag = headers["ETag"]
    print(f"  validator: {etag}")
    print(f"\n  {'request':32}{'status':>7}{'time':>9}{'bytes':>9}")
    print("  " + "-" * 58)
    for label, hdrs in [("unconditional", None), ("If-None-Match (matching)", {"If-None-Match": etag})]:
        for _ in range(2):
            status, payload, _, elapsed = client.get(base, headers=hdrs)
            print(f"  {label:32}{status:>7}{elapsed:8.3f}s{len(payload):>9}")
    print("\n  A conditional request must not pay for the evolution loop; if the two rows")
    print("  are the same order of magnitude, the validator is being computed from content.")
    return 0


# --------------------------------------------------------------------- saturate
def saturate(client: Client, concurrency: int, samples: int) -> int:
    """Is the process still answerable when every permit is held?

    Backs "How many permits, and why not one per core". A permit count equal to the
    core count let saturation stall the whole process for about six seconds, because
    the evolution loop is synchronous and leaves no CPU for the request pipeline.
    """
    board = client.create(grid("0.5", seed=4))
    print(f"  {concurrency} concurrent /final at the cap, sampling /health/live every 250 ms\n")
    print(f"  {'t(s)':>6}{'health':>8}{'ms':>9}   {'board':>6}{'ms':>7}")
    print("  " + "-" * 46)

    started = time.perf_counter()
    worst = 0.0
    with ThreadPoolExecutor(max_workers=concurrency + 10) as pool:
        futures = [pool.submit(client.get, f"/api/v1/boards/{board}/final") for _ in range(concurrency)]
        for _ in range(samples):
            hs, _, _, he = client.get("/health/live")
            bs, _, _, be = client.get(f"/api/v1/boards/{board}")
            worst = max(worst, he)
            flag = "  <-- would fail a 5s probe" if he > 5 else ("  <-- slow" if he > 1 else "")
            print(f"  {time.perf_counter() - started:6.2f}{str(hs):>8}{he * 1000:8.0f}   {str(bs):>6}{be * 1000:6.0f}{flag}")
            time.sleep(0.25)
        outcomes = [f.result() for f in futures]

    codes: dict = {}
    for status, _, _, _ in outcomes:
        codes[status] = codes.get(status, 0) + 1
    rejected = [e for s, _, _, e in outcomes if s == 503]
    print(f"\n  /final outcomes        : {codes}")
    print(f"  worst liveness latency : {worst * 1000:.0f} ms")
    if rejected:
        print(f"  503 rejection latency  : median {statistics.median(rejected) * 1000:.0f} ms")
    return 0


COMMANDS = {"validate": validate, "load": load, "cache": cache, "saturate": saturate}


def main() -> int:
    parser = argparse.ArgumentParser(
        description=__doc__,
        formatter_class=argparse.RawDescriptionHelpFormatter,
    )
    parser.add_argument("command", choices=list(COMMANDS) + ["all"])
    parser.add_argument("--base", default="http://localhost:5000", help="API base URL")
    parser.add_argument("--concurrency", type=int, default=14, help="saturate: simultaneous /final requests")
    parser.add_argument("--samples", type=int, default=24, help="saturate: liveness samples to take")
    args = parser.parse_args()

    client = Client(args.base)
    status, _, _, _ = client.get("/health/ready")
    if status != 200:
        print(f"API at {args.base} is not ready (/health/ready returned {status}).", file=sys.stderr)
        return 2

    order = ["validate", "load", "cache", "saturate"] if args.command == "all" else [args.command]
    failures = 0
    for name in order:
        print("=" * 70)
        print(name.upper())
        print("=" * 70)
        if name == "saturate":
            failures += saturate(client, args.concurrency, args.samples)
        else:
            failures += COMMANDS[name](client)
        print()
    return 1 if failures else 0


if __name__ == "__main__":
    raise SystemExit(main())
