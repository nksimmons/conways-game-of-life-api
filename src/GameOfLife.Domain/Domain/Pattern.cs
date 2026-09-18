using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using GameOfLife.Domain.Common;

namespace GameOfLife.Domain.Domain;

/// <summary>
///     An arrangement of live and dead cells: a seed, or any generation computed from one.
///     Cells are bit-packed row-major; callers use <see cref="IsAlive" /> and never see the backing storage.
/// </summary>
public sealed class Pattern : IEquatable<Pattern>
{
    private readonly ulong[] _bits;

    private Pattern(int width, int height, ulong[] bits, int population)
    {
        Width = width;
        Height = height;
        _bits = bits;
        Population = population;
    }

    public int Width { get; }

    public int Height { get; }

    public int Population { get; }

    /// <summary>The packed backing storage, for persistence only. Never exposed as a mutable array.</summary>
    public ReadOnlySpan<byte> PackedBytes => MemoryMarshal.AsBytes(_bits.AsSpan());

    public bool Equals(Pattern? other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return Width == other.Width && Height == other.Height && _bits.AsSpan().SequenceEqual(other._bits);
    }

    /// <summary>Builds a pattern from row-major 0/1 values, as received at the API boundary.</summary>
    public static Pattern FromRows(IReadOnlyList<IReadOnlyList<int>> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        if (rows.Count == 0) throw new ArgumentException("A pattern must have at least one row.", nameof(rows));
        if (rows.Any(row => false)) throw new ArgumentException("Rows cannot be null.", nameof(rows));

        var width = rows[0].Count;
        if (width == 0) throw new ArgumentException("A pattern must have at least one column.", nameof(rows));

        var height = rows.Count;
        var bits = new ulong[WordCount(width, height)];
        var population = 0;

        foreach (var (currentRow, row) in rows.WithIndex())
        {
            if (currentRow.Count != width)
                throw new ArgumentException("Every row must have the same number of columns.", nameof(rows));

            foreach (var (value, col) in currentRow.WithIndex())
            {
                if (value is not (0 or 1))
                    throw new ArgumentException($"Cell values must be 0 or 1; found {value}.", nameof(rows));

                if (value != 1) continue;

                SetBit(bits, row * width + col);
                population++;
            }
        }

        return new Pattern(width, height, bits, population);
    }

    /// <summary>Reconstructs a pattern from its packed bytes, as read back from storage.</summary>
    public static Pattern FromPackedBytes(int width, int height, ReadOnlySpan<byte> packedBytes)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be positive.");

        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be positive.");

        var expectedWords = WordCount(width, height);
        var expectedBytes = expectedWords * sizeof(ulong);
        if (packedBytes.Length != expectedBytes)
            throw new ArgumentException(
                $"Expected {expectedBytes} packed bytes for a {width}x{height} pattern, got {packedBytes.Length}.",
                nameof(packedBytes));

        var bits = new ulong[expectedWords];
        MemoryMarshal.Cast<byte, ulong>(packedBytes).CopyTo(bits);

        var population = bits.Sum(BitOperations.PopCount);

        return new Pattern(width, height, bits, population);
    }

    public bool IsAlive(int row, int col)
    {
        if ((uint)row >= (uint)Height)
            throw new ArgumentOutOfRangeException(nameof(row), row, "Row is outside the pattern.");

        if ((uint)col >= (uint)Width)
            throw new ArgumentOutOfRangeException(nameof(col), col, "Column is outside the pattern.");

        return GetBit(_bits, row * Width + col);
    }

    /// <summary>
    ///     Computes the next generation under the given rule and topology. Every cell reads only this
    ///     pattern's (the previous generation's) state; the result is an entirely new instance, so no
    ///     cell ever observes a neighbour that has already been updated.
    /// </summary>
    public Pattern NextGeneration(ILifeRule rule, ITopology topology, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(rule);
        ArgumentNullException.ThrowIfNull(topology);

        var nextBits = new ulong[_bits.Length];
        var population = 0;
        // Reuse the columns across rows instead of allocating an iterator for every row.
        var columns = Enumerable.Range(0, Width).ToArray();

        foreach (var row in Enumerable.Range(0, Height))
        {
            ct.ThrowIfCancellationRequested();

            foreach (var col in columns)
            {
                var alive = IsAlive(row, col);
                var liveNeighbors = topology.CountLiveNeighbors(this, row, col);

                if (!rule.NextState(alive, liveNeighbors)) continue;

                SetBit(nextBits, row * Width + col);
                population++;
            }
        }

        return new Pattern(Width, Height, nextBits, population);
    }

    public StateHash ComputeHash()
    {
        Span<byte> header = stackalloc byte[8];
        BinaryPrimitives.WriteInt32LittleEndian(header[..4], Width);
        BinaryPrimitives.WriteInt32LittleEndian(header[4..], Height);

        using var sha256 = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        sha256.AppendData(header);
        sha256.AppendData(MemoryMarshal.AsBytes(_bits.AsSpan()));
        Span<byte> digest = stackalloc byte[32];
        sha256.GetHashAndReset(digest);
        return StateHash.FromSha256Digest(digest);
    }

    public override bool Equals(object? obj) => Equals(obj as Pattern);

    // Deliberately not ComputeHash(): that is a SHA-256 over the whole grid, far too expensive for a
    // hash code. Width, height, and population are cheap and consistent with Equals.
    public override int GetHashCode() => HashCode.Combine(Width, Height, Population);

    // Cast to long before multiplying: at today's 256x256 / 65,536-cell caps this never overflows
    // int, but AGENTS.md §3.5 says raising the caps is a design change, not a config tweak, and this
    // keeps that future change from silently wrapping into a negative word count.
    private static int WordCount(int width, int height) => (int)(((long)width * height + 63) / 64);

    private static void SetBit(ulong[] bits, int index) => bits[index >> 6] |= 1UL << (index & 63);

    private static bool GetBit(ulong[] bits, int index) => (bits[index >> 6] & (1UL << (index & 63))) != 0;
}
