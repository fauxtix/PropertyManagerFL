using PropertyManagerFL.Core.Helpers;

namespace PropertyManagerFL.Tests;

public class MergeFieldFormatTests
{
    [Fact]
    public void Result_UsesWordGuillemetsAroundFieldName()
    {
        string result = MergeFieldFormat.Result("LocalEmissao");

        Assert.Equal("\u00ABLocalEmissao\u00BB", result);
        Assert.Equal(0x00AB, (int)result[0]);
        Assert.Equal(0x00BB, (int)result[^1]);
        Assert.DoesNotContain("\uFFFD", result);
        Assert.False(result.StartsWith('\''));
    }

    [Fact]
    public void Code_MatchesExistingDotxTemplates()
    {
        Assert.Equal("MERGEFIELD  LocalEmissao  \\* MERGEFORMAT ", MergeFieldFormat.Code("LocalEmissao"));
    }
}
