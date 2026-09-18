using System.Buffers.Binary;

namespace GameOfLife.Domain.Domain;

/// <summary>A 256-bit SHA-256 digest of a <see cref="Pattern"/>, used as a cheap dictionary key for cycle detection.</summary>
public readonly struct StateHash : IEquatable<StateHash>
{
    // Four ulongs rather than a byte[32] because this is constructed once per generation inside the
    // fate-search loop: an array would allocate per generation and, comparing by reference, would need
    // a custom IEqualityComparer to work as a dictionary key at all.
    private readonly ulong _a;
    private readonly ulong _b;
    private readonly ulong _c;
    private readonly ulong _d;

    private StateHash(ulong a, ulong b, ulong c, ulong d)
    {
        _a = a;
        _b = b;
        _c = c;
        _d = d;
    }

    public static StateHash FromSha256Digest(ReadOnlySpan<byte> digest)
    {
        if (digest.Length != 32)
        {
            throw new ArgumentException("A SHA-256 digest is exactly 32 bytes.", nameof(digest));
        }

        return new StateHash(
            BinaryPrimitives.ReadUInt64LittleEndian(digest[..8]),
            BinaryPrimitives.ReadUInt64LittleEndian(digest[8..16]),
            BinaryPrimitives.ReadUInt64LittleEndian(digest[16..24]),
            BinaryPrimitives.ReadUInt64LittleEndian(digest[24..32]));
    }

    private byte[] ToByteArray()
    {
        var bytes = new byte[32];
        BinaryPrimitives.WriteUInt64LittleEndian(bytes.AsSpan(0, 8), _a);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes.AsSpan(8, 8), _b);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes.AsSpan(16, 8), _c);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes.AsSpan(24, 8), _d);
        return bytes;
    }

    public bool Equals(StateHash other) => _a == other._a && _b == other._b && _c == other._c && _d == other._d;

    public override bool Equals(object? obj) => obj is StateHash other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_a, _b, _c, _d);

    public static bool operator ==(StateHash left, StateHash right) => left.Equals(right);

    public static bool operator !=(StateHash left, StateHash right) => !left.Equals(right);

    public override string ToString() => Convert.ToHexString(ToByteArray()).ToLowerInvariant();
}
