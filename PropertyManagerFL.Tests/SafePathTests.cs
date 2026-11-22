using PropertyManagerFL.Infrastructure.Security;

namespace PropertyManagerFL.Tests;

public class SafePathTests
{
    [Fact]
    public void FileName_ReturnsFinalSegmentWhenPathTraversalIsSupplied()
    {
        var result = SafePath.FileName(Path.Combine("..", "..", "evil.txt"));

        Assert.Equal("evil.txt", result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(".")]
    [InlineData("..")]
    public void FileName_RejectsEmptyOrRelativeNames(string? value)
    {
        Assert.Throws<ArgumentException>(() => SafePath.FileName(value));
    }

    [Fact]
    public void SqlIdentifier_AcceptsPlainIdentifiers()
    {
        Assert.Equal("EstadoCivil_2026", SafePath.SqlIdentifier("EstadoCivil_2026"));
    }

    [Theory]
    [InlineData("Bad.Table")]
    [InlineData("'; DROP TABLE users; --")]
    [InlineData("Name With Space")]
    [InlineData("")]
    public void SqlIdentifier_RejectsAnythingElse(string value)
    {
        Assert.Throws<ArgumentException>(() => SafePath.SqlIdentifier(value));
    }

    [Fact]
    public void CombineContained_KeepsValidSubPathInsideRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), "pmfl-safepath-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(root, "sub"));

        try
        {
            var result = SafePath.CombineContained(root, "sub", "file.txt");

            Assert.StartsWith(Path.GetFullPath(root) + Path.DirectorySeparatorChar,
                result, StringComparison.OrdinalIgnoreCase);
            Assert.EndsWith(Path.Combine("sub", "file.txt"), result, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void CombineContained_RejectsPathOutsideRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), "pmfl-safepath-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);

        try
        {
            Assert.Throws<UnauthorizedAccessException>(() =>
                SafePath.CombineContained(root, Path.Combine("..", "outside.txt")));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
