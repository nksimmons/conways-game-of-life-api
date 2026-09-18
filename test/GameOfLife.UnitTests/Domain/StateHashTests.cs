using GameOfLife.Domain.Domain;

namespace GameOfLife.UnitTests.Domain;

public sealed class StateHashTests
{
    [Fact]
    public void FromSha256Digest_rejects_wrong_length()
    {
        Assert.Throws<ArgumentException>(() => StateHash.FromSha256Digest(new byte[16]));
        Assert.Throws<ArgumentException>(() => StateHash.FromSha256Digest(new byte[33]));
    }

    [Fact]
    public void ToString_renders_the_digest_as_lowercase_hex()
    {
        var digest = Enumerable.Range(0, 32).Select(i => (byte)i).ToArray();
        var hash = StateHash.FromSha256Digest(digest);

        Assert.Equal(Convert.ToHexString(digest).ToLowerInvariant(), hash.ToString());
    }

    [Fact]
    public void Equality_is_by_value_and_usable_as_a_dictionary_key()
    {
        var digest = Enumerable.Range(0, 32).Select(i => (byte)i).ToArray();
        var a = StateHash.FromSha256Digest(digest);
        var b = StateHash.FromSha256Digest(digest);

        var differentDigest = Enumerable.Range(1, 32).Select(i => (byte)i).ToArray();
        var c = StateHash.FromSha256Digest(differentDigest);

        Assert.Equal(a, b);
        Assert.True(a == b);
        Assert.NotEqual(a, c);
        Assert.True(a != c);

        var dictionary = new Dictionary<StateHash, int> { [a] = 1 };
        Assert.True(dictionary.ContainsKey(b));
        Assert.False(dictionary.ContainsKey(c));
    }
}