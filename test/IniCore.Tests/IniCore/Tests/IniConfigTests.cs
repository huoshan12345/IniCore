using static IniCore.Tests.TestConfigs;

namespace IniCore.Tests;

public class IniConfigTests
{
    public record TestCase(string Name, string Input, string Expected)
    {
        public override string ToString() => Name;
    }

    public static readonly IEnumerable<object[]> TestCases = new TestCase[]
    {
        new(nameof(CommonGit), CommonGit, CommonGitIni),
        new(nameof(ClashSubTemplate), ClashSubTemplate, ClashSubTemplateIni),
    }.SelectMany(m => new[]
    {
        m,
        m with { Name = m.Name + "_LF", Input = RegexReplacer.CRLF_TO_LF.Replace(m.Input) },
        m with { Name = m.Name + "_CRLF", Input = RegexReplacer.LF_TO_CRLF.Replace(m.Input) },
    }).Select(m => new object[] { m });

    [Theory]
    [MemberData(nameof(TestCases))]
    public void ToString_Test(TestCase testCase)
    {
        var (name, input, expected) = testCase;
        if (name.EndsWith("_LF"))
        {
            Assert.DoesNotContain(input, "\r\n");
        }
        else if (name.EndsWith("_CRLF"))
        {
            char? previous = null;
            foreach (var item in input)
            {
                if (item == '\n')
                {
                    Assert.Equal('\r', previous);
                }
                previous = item;
            }
        }

        var config = IniParser.Parse(input);
        Assert.Equal(expected, config.ToString().TrimEnd());
    }
}