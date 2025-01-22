using static Ini.Net.Tests.TestConfigs;

namespace Ini.Net.Tests;

public class IniParserTests
{
    public record TestCase(string Name, string Input, IniConfig Expected)
    {
        public override string ToString() => Name;
    }

    public static readonly IEnumerable<object[]> TestCases = new TestCase[]
    {
        new(nameof(Empty1), Empty1, new()),
        new(nameof(Empty2), Empty2, new()),
        new(nameof(SectionNesting), SectionNesting, new()
        {
            Sections =
            [
                new("section", new IniEntry("domain", "example.com"))
                {
                    SubSections = 
                    [
                        new("subsection", new IniEntry("foo", "bar")),
                    ],
                },
            ],
        }),
        new(nameof(RelativeNesting), RelativeNesting, new()
        {
            Sections =
            [
                new("section", new IniEntry("domain", "example.com"))
                {
                    SubSections =
                    [
                        new("subsection", new IniEntry("foo", "bar")),
                    ],
                },
            ],
        }),
        new(nameof(Simple), Simple, new()
        {
            Sections =
            [
                new("owner",
                    new("name", "John Doe"),
                    new("organization", "Acme Widgets Inc.")),
                new("database",
                    new("server", "192.0.2.62"),
                    new("port", "143"),
                    new("file", "payroll.dat")),
            ],
        }),
        new(nameof(Complex), Complex, new()
        {
            Sections =
            [
                new("project",
                    new("name", "orchard rental service (with app)"),
                    new("target region", "Bay Area"),
                    new("legal team", "(vacant)")),
                new("fruit \"Apple\"",
                    new("trademark issues", "foreseeable"),
                    new("taste", "known")),
                new("fruit")
                {
                    SubSections =
                    [
                        new("Date",
                            new("taste", "novel"),
                            new("Trademark Issues", "truly unlikely")),
                        new("raspberry")
                        {
                            SubSections =
                            [
                                new("proponents")
                                {
                                    SubSections =
                                    [
                                        new("fred",
                                            new("date", "2021-11-23, 08:54 +0900"),
                                            new("comment", "I like red fruit.")),
                                    ],
                                }
                            ],
                        },
                    ],
                },
                new("fruit \"Raspberry\"",
                    new("anticipated problems", "logistics (fragile fruit)"),
                    new("Trademark Issues", "possible")),
                new("fruit \"Date/proponents/alfred\"",
                    new("comment", "Why, I would buy dates."),
                    new("editor", "My name may contain a newline.")),
            ],
        }),
        new(nameof(CommonGit), CommonGit, new()
        {
            Sections =
            [
                new("core",
                    new("filemode", "false"),
                    new("gitProxy", "ssh for kernel.org"),
                    new("gitProxy", "default-proxy")),
                new("diff",
                    new("external", "/usr/local/bin/diff-wrapper"),
                    new("renames", "true")),
                new("branch \"devel\"",
                    new("remote", "origin"),
                    new("merge", "refs/heads/devel")),
                new("include",
                    new("path", "/path/to/foo.inc"),
                    new("path", "foo.inc"),
                    new("path", "~/foo.inc")),
                new("includeIf \"gitdir:/path/to/foo/.git\"", new IniEntry("path", "/path/to/foo.inc")),
                new("includeIf \"gitdir:/path/to/group/\"",
                    new("path", "/path/to/foo.inc"),
                    new("path", "foo.inc")),
                new("includeIf \"gitdir:~/to/group/\"", new IniEntry("path", "/path/to/foo.inc")),
                new("includeIf \"onbranch:foo-branch\"", new IniEntry("path", "foo.inc")),
                new("includeIf \"hasconfig:remote.*.url:https://example.com/**\"", new IniEntry("path", "foo.inc")),
                new("remote \"origin\"", new IniEntry("url", "https://example.com/git")),
            ],
        }),
        new(nameof(ClashSubTemplate), ClashSubTemplate, new()
        {
            Sections =
            [
                new("custom",
                    new("ruleset", "🎯 全球直连,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/LocalAreaNetwork.list"),
                    new("ruleset", "🎯 全球直连,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/UnBan.list"),
                    new("ruleset", "🛑 全球拦截,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/BanAD.list"),
                    new("ruleset", "🛑 全球拦截,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/BanProgramAD.list"),
                    new("ruleset", "🎯 全球直连,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/GoogleCN.list"),
                    new("ruleset", "🎯 全球直连,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/RuleSet/SteamCN.list"),
                    new("ruleset", "🚀 节点选择,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/Telegram.list"),
                    new("ruleset", "🚀 节点选择,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/ProxyMedia.list"),
                    new("ruleset", "🚀 节点选择,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/ProxyLite.list"),
                    new("ruleset", "🎯 全球直连,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/ChinaDomain.list"),
                    new("ruleset", "🎯 全球直连,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/ChinaCompanyIp.list"),
                    new("ruleset", "🎯 全球直连,[]GEOIP,CN"),
                    new("ruleset", "🐟 漏网之鱼,[]FINAL"),
                    new("custom_proxy_group", "🚀 节点选择`select`[]♻️ 自动选择`[]DIRECT`.*"),
                    new("custom_proxy_group", "♻️ 自动选择`url-test`.*`http://www.gstatic.com/generate_204`300,,50"),
                    new("custom_proxy_group", "🎯 全球直连`select`[]DIRECT`[]🚀 节点选择`[]♻️ 自动选择"),
                    new("custom_proxy_group", "🛑 全球拦截`select`[]REJECT`[]DIRECT"),
                    new("custom_proxy_group", "🐟 漏网之鱼`select`[]🚀 节点选择`[]🎯 全球直连`[]♻️ 自动选择`.*"),
                    new("enable_rule_generator", "true"),
                    new("overwrite_original_rules", "true")),
            ],
        }),
    }.Select(m => new object[] { m });

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Parse_Test(TestCase testCase)
    {
        var (_, input, expected) = testCase;
        var actual = IniParser.Parse(input);
        AssertEx.EveryMemberEqual(expected, actual);
    }
}