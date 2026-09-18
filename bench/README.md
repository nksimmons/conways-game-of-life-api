# bench

`docs/design.md` quotes measured numbers. This is how they were produced, so a reader can check them rather than take them on trust.

Standard library Python only, nothing to install. Start the API in **Release** first, because Debug timings are several times slower and are not what the document reports:

```bash
dotnet run -c Release --project src/GameOfLife.Api --no-launch-profile
python3 bench/probe.py all --base http://localhost:5000
```

| Subcommand | What it measures | Design document section |
|---|---|---|
| `validate` | 34 malformed and hostile payloads against the boundary rules | §10.5 |
| `load` | Cost of `generations/1000` and `final` by seed shape at the caps | §10.1.1 |
| `cache` | Cost of a conditional request against an unconditional one | §5.5 |
| `saturate` | Liveness while every evaluation permit is held | §8.4 |

`validate` exits non-zero if any probe returns an unexpected status, so it can gate a pipeline. The timing subcommands always exit zero: they report, and a human decides whether the numbers are acceptable.

Grid seeds are fixed, so generation counts reproduce exactly; wall-clock times will differ by machine. `saturate` is the one to re-run after changing `MaxConcurrentEvaluations`, since it is the check that a saturated node still answers its health probe.
