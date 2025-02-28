namespace IniCore.Tests;

public static class TestConfigs
{
    private static readonly string TestDataDir = Path.Combine(AppContext.BaseDirectory, "TestData");

    public const string Empty1 = "";
    public const string Empty2 = "  \n\t\n\n \n ";

    public const string Simple = """
                                  ; last modified 1 April 2001 by John Doe
                                  [owner]
                                  name = John Doe
                                  organization = Acme Widgets Inc.
                                  
                                  [database]
                                  ; use IP address in case network name resolution is not working
                                  server = 192.0.2.62     
                                  port = 143
                                  file = "payroll.dat"
                                  """;


    public const string SectionHeaderEndsWithDot = "[owner.";
    public const string EmptySubsectionName = "[owner.]";

    public const string Complex = """
                                 [project]
                                 name = orchard rental service (with app)
                                 target region = "Bay Area"
                                 ; TODO: advertise vacant positions
                                 legal team = (vacant)

                                 [fruit "Apple"]
                                 trademark issues = foreseeable
                                 taste = known

                                 [fruit.Date]
                                 taste = novel
                                 Trademark Issues="truly unlikely"

                                 [fruit "Raspberry"]
                                 anticipated problems  ="logistics (fragile fruit)"
                                 Trademark Issues=possible

                                 [fruit.raspberry.proponents.fred]
                                 date = 2021-11-23, 08:54 +0900
                                 comment = "I like red fruit."
                                 [fruit "Date/proponents/alfred"]
                                 comment: Why, I would buy dates.
                                 # folding: Is "\\\\\nn" interpreted as "\\n" or "\n"?
                                 #   Or does "\\\\" prevent folding?
                                 editor  =My name may contain a newline.
                                 """;

    public const string Escape = """
                                 [project]
                                 name = orchard rental service (with app)
                                 target region = "Bay Area"
                                 ; TODO: advertise vacant positions
                                 legal team = (vacant)

                                 [fruit "Apple"]
                                 trademark issues = foreseeable
                                 taste = known

                                 [fruit.Date]
                                 taste = novel
                                 Trademark Issues="truly unlikely"

                                 [fruit "Raspberry"]
                                 anticipated problems  ="logistics (fragile fruit)"
                                 Trademark Issues=\
                                  possible

                                 [fruit.raspberry.proponents.fred]
                                 date = 2021-11-23, 08:54 +0900
                                 comment = "I like red fruit."
                                 [fruit "Date/proponents/alfred"]
                                 comment: Why,  \
                                  \
                                  \
                                  I would buy dates.
                                 # folding: Is "\\\\\nn" interpreted as "\\n" or "\n"?
                                 #   Or does "\\\\" prevent folding?
                                 editor  =My name may contain a \\
                                 newline.
                                 """;

    public const string SectionNesting = $"""
                                       [section]
                                       domain = example.com
                                       
                                       [section.subsection]
                                       foo = bar
                                       """;

    public const string RelativeNesting = """
                                          [section]
                                          domain = example.com

                                          [.subsection]
                                          foo = bar
                                          """;

    public static readonly string CommonGit = ReadTestFile("common.gitconfig.ini");
    public static readonly string CommonGitJson = ReadTestFile("common.gitconfig.json");
    public static readonly string CommonGitIni = ReadTestFile("common.gitconfig.parsed.ini");
    public static readonly string ClashSubTemplate = ReadTestFile("clash_sub_template.ini");
    public static readonly string ClashSubTemplateJson = ReadTestFile("clash_sub_template.json");
    public static readonly string ClashSubTemplateIni = ReadTestFile("clash_sub_template.parsed.ini");

    private static string ReadTestFile(string fileName)
    {
        return TestDataDir
            .Pipe(m => Path.Combine(m, fileName))
            .Pipe(static m => File.ReadAllText(m));
    }
}