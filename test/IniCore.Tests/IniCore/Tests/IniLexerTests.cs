using static IniCore.Tests.TestConfigs;

namespace IniCore.Tests;

public class IniLexerTests
{
    public record TestCase(string Name, string Input, IniToken[] Expected)
    {
        public override string ToString() => Name;
    }

    public static readonly IEnumerable<object[]> TestCases = new TestCase[]
    {
        new(nameof(Empty1), Empty1, []),
        new(nameof(Empty2), Empty2, []),
        new(nameof(SectionNesting), SectionNesting, 
        [
            IniTokenType.SectionName.Make("section"),
            ..IniTokens.Entry("domain", "example.com"),
            IniTokenType.SectionName.Make("section"),
            IniTokenType.SubsectionName.Make("subsection"),
            ..IniTokens.Entry("foo", "bar"),
        ]),
        new(nameof(RelativeNesting), RelativeNesting,
        [
            IniTokenType.SectionName.Make("section"),
            ..IniTokens.Entry("domain", "example.com"),
            IniTokenType.SectionName.Make(""),
            IniTokenType.SubsectionName.Make("subsection"),
            ..IniTokens.Entry("foo", "bar"),
        ]),
        new(nameof(SectionHeaderEndsWithDot), SectionHeaderEndsWithDot,
        [
            IniTokenType.SectionName.Make("owner"),
            IniTokenType.Error.Make("SubsectionName cannot be empty"),
        ]),
        new(nameof(EmptySubsectionName), EmptySubsectionName,
        [
            IniTokenType.SectionName.Make("owner"),
            IniTokenType.Error.Make("SubsectionName cannot be empty"),
        ]),
        new(nameof(Simple), Simple,
        [
            IniTokenType.Comment.Make("; last modified 1 April 2001 by John Doe"),
            IniTokenType.SectionName.Make("owner"),
            ..IniTokens.Entry("name", "John Doe"),
            ..IniTokens.Entry("organization", "Acme Widgets Inc."),

            IniTokenType.SectionName.Make("database"),
            IniTokenType.Comment.Make("; use IP address in case network name resolution is not working"),
            ..IniTokens.Entry("server", "192.0.2.62"),
            ..IniTokens.Entry("port", "143"),
            ..IniTokens.Entry("file", "payroll.dat"),
        ]),
        new(nameof(Complex), Complex,
        [
            IniTokenType.SectionName.Make("project"),
            ..IniTokens.Entry("name", "orchard rental service (with app)"),
            ..IniTokens.Entry("target region", "Bay Area"),
            IniTokenType.Comment.Make("; TODO: advertise vacant positions"),
            ..IniTokens.Entry("legal team", "(vacant)"),

            IniTokenType.SectionName.Make("fruit \"Apple\""),
            ..IniTokens.Entry("trademark issues", "foreseeable"),
            ..IniTokens.Entry("taste", "known"),

            IniTokenType.SectionName.Make("fruit"),
            IniTokenType.SubsectionName.Make("Date"),
            ..IniTokens.Entry("taste", "novel"),
            ..IniTokens.Entry("Trademark Issues", "truly unlikely"),

            IniTokenType.SectionName.Make("fruit \"Raspberry\""),
            ..IniTokens.Entry("anticipated problems", "logistics (fragile fruit)"),
            ..IniTokens.Entry("Trademark Issues", "possible"),

            IniTokenType.SectionName.Make("fruit"),
            IniTokenType.SubsectionName.Make("raspberry"),
            IniTokenType.SubsectionName.Make("proponents"),
            IniTokenType.SubsectionName.Make("fred"),
            ..IniTokens.Entry("date", "2021-11-23, 08:54 +0900"),
            ..IniTokens.Entry("comment", "I like red fruit."),

            IniTokenType.SectionName.Make("fruit \"Date/proponents/alfred\""),
            ..IniTokens.Entry("comment", "Why, I would buy dates."),
            IniTokenType.Comment.Make("""# folding: Is "\\\\\nn" interpreted as "\\n" or "\n"?"""),
            IniTokenType.Comment.Make("""#   Or does "\\\\" prevent folding?"""),
            ..IniTokens.Entry("editor", "My name may contain a newline."),
        ]),
        new(nameof(CommonGit), CommonGit,
        [
            IniTokens.Comment("# Core variables"),
            IniTokens.SectionName("core"),
            IniTokens.Comment("; Don't trust file modes"),
            ..IniTokens.Entry("filemode", "false"),

            IniTokens.Comment("# Our diff algorithm"),
            IniTokens.SectionName("diff"),
            ..IniTokens.Entry("external", "/usr/local/bin/diff-wrapper"),
            ..IniTokens.Entry("renames", "true"),

            IniTokens.SectionName("branch \"devel\""),
            ..IniTokens.Entry("remote", "origin"),
            ..IniTokens.Entry("merge", "refs/heads/devel"),

            IniTokens.Comment("# Proxy settings"),
            IniTokens.SectionName("core"),
            ..IniTokens.Entry("gitProxy", "ssh for kernel.org"),
            ..IniTokens.Entry("gitProxy", "default-proxy"), IniTokens.Comment("; for the rest"),

            IniTokens.SectionName("include"),
            ..IniTokens.Entry("path", "/path/to/foo.inc"), IniTokens.Comment("; include by absolute path"),
            ..IniTokens.Entry("path", "foo.inc"), IniTokens.Comment("; find \"foo.inc\" relative to the current file"),
            ..IniTokens.Entry("path", "~/foo.inc"), IniTokens.Comment("; find \"foo.inc\" in your `$HOME` directory"),

            IniTokens.Comment("; include if $GIT_DIR is /path/to/foo/.git"),
            IniTokens.SectionName("includeIf \"gitdir:/path/to/foo/.git\""),
            ..IniTokens.Entry("path", "/path/to/foo.inc"),

            IniTokens.Comment("; include for all repositories inside /path/to/group"),
            IniTokens.SectionName("includeIf \"gitdir:/path/to/group/\""),
            ..IniTokens.Entry("path", "/path/to/foo.inc"),

            IniTokens.Comment("; include for all repositories inside $HOME/to/group"),
            IniTokens.SectionName("includeIf \"gitdir:~/to/group/\""),
            ..IniTokens.Entry("path", "/path/to/foo.inc"),

            IniTokens.Comment("; relative paths are always relative to the including"),
            IniTokens.Comment("; file (if the condition is true); their location is not"),
            IniTokens.Comment("; affected by the condition"),
            IniTokens.SectionName("includeIf \"gitdir:/path/to/group/\""),
            ..IniTokens.Entry("path", "foo.inc"),

            IniTokens.Comment("; include only if we are in a worktree where foo-branch is"),
            IniTokens.Comment("; currently checked out"),
            IniTokens.SectionName("includeIf \"onbranch:foo-branch\""),
            ..IniTokens.Entry("path", "foo.inc"),

            IniTokens.Comment("; include only if a remote with the given URL exists (note"),
            IniTokens.Comment("; that such a URL may be provided later in a file or in a"),
            IniTokens.Comment("; file read after this file is read, as seen in this example)"),
            IniTokens.SectionName("includeIf \"hasconfig:remote.*.url:https://example.com/**\""),
            ..IniTokens.Entry("path", "foo.inc"),

            IniTokens.SectionName("remote \"origin\""),
            ..IniTokens.Entry("url", "https://example.com/git"),
        ]),
        new(nameof(ClashSubTemplate), ClashSubTemplate,
        [
            IniTokens.SectionName("custom"),
            IniTokens.Comment(";不要随意改变关键字，否则会导致出错"),
            IniTokens.Comment(";acl4SSR规则-在线版"),
            IniTokens.Comment(";去广告：支持"),
            IniTokens.Comment(";自动测速：支持"),
            IniTokens.Comment(";微软分流：不支持"),
            IniTokens.Comment(";苹果分流：不支持"),
            IniTokens.Comment(";增强中国IP段：不支持"),
            IniTokens.Comment(";增强国外GFW：不支持"),
            ..IniTokens.Entry("ruleset", "全球直连,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/LocalAreaNetwork.list"),
            ..IniTokens.Entry("ruleset", "全球直连,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/UnBan.list"),
            ..IniTokens.Entry("ruleset", "全球拦截,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/BanAD.list"),
            ..IniTokens.Entry("ruleset", "全球拦截,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/BanProgramAD.list"),
            ..IniTokens.Entry("ruleset", "全球直连,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/GoogleCN.list"),
            ..IniTokens.Entry("ruleset", "全球直连,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/RuleSet/SteamCN.list"),
            ..IniTokens.Entry("ruleset", "节点选择,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/Telegram.list"),
            ..IniTokens.Entry("ruleset", "节点选择,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/ProxyMedia.list"),
            ..IniTokens.Entry("ruleset", "节点选择,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/ProxyLite.list"),
            ..IniTokens.Entry("ruleset", "全球直连,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/ChinaDomain.list"),
            ..IniTokens.Entry("ruleset", "全球直连,https://raw.githubusercontent.com/ACL4SSR/ACL4SSR/master/Clash/ChinaCompanyIp.list"),
            IniTokens.Comment(";ruleset=全球直连,[]GEOIP,LAN"),
            ..IniTokens.Entry("ruleset", "全球直连,[]GEOIP,CN"),
            ..IniTokens.Entry("ruleset", "漏网之鱼,[]FINAL"),
            ..IniTokens.Entry("custom_proxy_group", "节点选择`select`[]自动选择`[]DIRECT`.*"),
            ..IniTokens.Entry("custom_proxy_group", "自动选择`url-test`.*`http://www.gstatic.com/generate_204`300,,50"),
            ..IniTokens.Entry("custom_proxy_group", "全球直连`select`[]DIRECT`[]节点选择`[]自动选择"),
            ..IniTokens.Entry("custom_proxy_group", "全球拦截`select`[]REJECT`[]DIRECT"),
            ..IniTokens.Entry("custom_proxy_group", "漏网之鱼`select`[]节点选择`[]全球直连`[]自动选择`.*"),
            ..IniTokens.Entry("enable_rule_generator", "true"),
            ..IniTokens.Entry("overwrite_original_rules", "true"),
        ]),
    }.Select(m => new object[] { m });

    [Theory]
    [MemberData(nameof(TestCases))]
    public void LexTokens_Test(TestCase testCase)
    {
        var (_, input, expected) = testCase;
        var lexer = new IniLexer(input);
        var i = 0;

        foreach (var it in lexer.LexTokens())
        {
            if (it.Type == IniTokenType.LineFeed)
                continue;

            if (i >= expected.Length)
            {
                Assert.Fail($"token {i}, unexpected item: {it}");
            }

            var item = expected[i];

            if (it.Type != item.Type || it.Value != item.Value)
            {
                Assert.Fail($"item {i}, expected item: {item}, actual: {it}");
            }

            i++;
        }

        if (expected.Length is var len && len != i)
        {
            Assert.Fail($"expected to lex {len} items, actually lexed {i}");
        }
    }
}