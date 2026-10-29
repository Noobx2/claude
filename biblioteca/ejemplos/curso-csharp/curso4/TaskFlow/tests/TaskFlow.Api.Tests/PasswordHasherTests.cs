using TaskFlow.Infrastructure.Security;

namespace TaskFlow.Api.Tests;

public class PasswordHasherTests
{
    [Fact]
    public void Verify_AcceptsCorrectPassword_RejectsWrongOne()
    {
        string stored = PasswordHasher.Hash("mi-clave-larga");
        Assert.True(PasswordHasher.Verify("mi-clave-larga", stored));
        Assert.False(PasswordHasher.Verify("otra-clave", stored));
    }

    [Fact]
    public void Hash_SamePasswordTwice_ProducesDifferentHashes()   // por la sal aleatoria
    {
        Assert.NotEqual(PasswordHasher.Hash("igual"), PasswordHasher.Hash("igual"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("no-es-un-hash")]
    [InlineData("1.2")]
    public void Verify_WithGarbageStoredValue_ReturnsFalse(string stored)
    {
        Assert.False(PasswordHasher.Verify("x", stored));
    }
}
