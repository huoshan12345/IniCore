using static IniCore.Tests.TestConfigs;

namespace IniCore.Tests;

public class IniConfigExtensionsTests
{
    public record TestCase(string Name, string Input, string ExpectedJson, string ExpectedIni)
    {
        public override string ToString() => Name;
    }

    public static readonly IEnumerable<object[]> TestCases = new TestCase[]
    {
        new(nameof(CommonGit), CommonGit, CommonGitJson, CommonGitIni),
        new(nameof(ClashSubTemplate), ClashSubTemplate, ClashSubTemplateJson, ClashSubTemplateIni),
    }.SelectMany(m => new[]
    {
        m,
        m with { Name = m.Name + "_LF", Input = RegexReplacer.CRLF_TO_LF.Replace(m.Input) },
        m with { Name = m.Name + "_CRLF", Input = RegexReplacer.LF_TO_CRLF.Replace(m.Input) },
    }).Select(m => new object[] { m });

    [Theory]
    [MemberData(nameof(TestCases))]
    public void ToStructuredJsonObject_Test(TestCase testCase)
    {
        var (_, input, expected, _) = testCase;
        var config = IniParser.Parse(input);
        var actual = config.ToStructuredJsonObject().ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(TestCases))]
    public void ToIniConfig_Test(TestCase testCase)
    {
        var (_, input, _, expected) = testCase;
        var config = IniParser.Parse(input);
        var actual = config.ToStructuredJsonObject().ToIniConfig().ToString();
        Assert.Equal(expected, actual.TrimEnd());
    }

    [Fact]
    public void ToStructuredJsonObject_ToObject_Test()
    {
        var config = IniParser.Parse(CommonGit);
        var gitConfig = config.ToStructuredJsonObject().Deserialize<GitConfig>();
        Assert.NotNull(gitConfig);

        var expected = new GitConfig
        {
            Core = new GitConfigCore
            {
                FileMode = false,
                GitProxy =
                [
                    "ssh for kernel.org",
                    "default-proxy",
                ],
            },
            Diff = new GitConfigDiff
            {
                External = "/usr/local/bin/diff-wrapper",
                Renames = true,
            },
            Include = new GitConfigInclude
            {
                Path =
                [
                    "/path/to/foo.inc",
                    "foo.inc",
                    "~/foo.inc",
                ]
            },
            Branches =
            [
                new GitConfigBranch
                {
                    Branch = "devel",
                    Remote = "origin",
                    Merge = "refs/heads/devel",
                },
            ],
            IncludeIfs =
            [
                new GitConfigIncludeIf
                {
                    Branch = "gitdir:/path/to/foo/.git",
                    Path = ["/path/to/foo.inc"],
                },
                new GitConfigIncludeIf
                {
                    Branch = "gitdir:/path/to/group/",
                    Path = ["/path/to/foo.inc", "foo.inc"],
                },
                new GitConfigIncludeIf
                {
                    Branch = "gitdir:~/to/group/",
                    Path = ["/path/to/foo.inc"],
                },
                new GitConfigIncludeIf
                {
                    Branch = "onbranch:foo-branch",
                    Path = ["foo.inc"],
                },
                new GitConfigIncludeIf
                {
                    Branch = "hasconfig:remote.*.url:https://example.com/**",
                    Path = ["foo.inc"],
                },
            ],
            Remotes =
            [
                new GitConfigRemote
                {
                    Branch = "origin",
                    Url = "https://example.com/git",
                },
            ],
        };

        Assert.Equivalent(expected, gitConfig);
    }
}
