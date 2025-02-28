namespace IniCore;

public static class IniParser
{
    public static IniConfig Parse(string input)
    {
        var lexer = new IniLexer(input);
        var config = new IniConfig();
        IniSection? section = null;

        using var e = lexer.LexTokens().GetEnumerator();

        while (e.MoveNext())
        {
            var token = e.Current!;

            if (token.Type == IniTokenType.SectionName)
            {
                var sections = config.Sections;

                if (token.Value is "")
                {
                    if (section is null)
                    {
                        throw new InvalidOperationException($"Invalid config: encountered a relative subsection without a preceding section at line {token.Line}.");
                    }
                }
                else
                {
                    section = sections.GetOrAdd(token.Value);
                }

                sections = section.SubSections;

                while (e.MoveNext())
                {
                    token = e.Current!;

                    if (token.Type != IniTokenType.SubsectionName)
                        break;

                    section = sections.GetOrAdd(token.Value);
                    sections = section.SubSections;
                }

                continue;
            }

            switch (token.Type)
            {
                case IniTokenType.Error:
                {
                    throw new InvalidOperationException("Invalid config: encountered an error: " + token.Value);
                }
                case IniTokenType.Comment:
                {
                    //TODO: support comments.
                    break;
                }
                case IniTokenType.LineFeed:
                {
                    break;
                }
                case IniTokenType.Key:
                {
                    if (e.MoveNext())
                    {
                        var next = e.Current!;

                        if (next.Type != IniTokenType.Value)
                            throw new InvalidOperationException($"Invalid config: expected a value but got a {next.Value} after key '{token.Value}' at line {token.Line}.");

                        var entry = new IniEntry(token.Value, next.Value);
                        var list = section == null
                            ? config.Entries
                            : section.Entries;
                        list.Add(entry);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Invalid config: expected a value but got EOF after key '{token.Value}' at line {token.Line}.");
                    }

                    break;
                }
                case IniTokenType.Value:
                {
                    throw new InvalidOperationException($"Invalid config: encountered a value '{token.Value}' without a preceding key at line {token.Line}.");
                }
                case IniTokenType.Eof:
                {
                    break;
                }
                case IniTokenType.SectionName:
                case IniTokenType.SubsectionName:
                default:
                {
                    throw new InvalidOperationException($"Invalid syntax: unexpected token type '{token.Type}'.");
                }
            }
        }

        return config;
    }
}

file static class Extensions
{
    public static IniSection GetOrAdd(this List<IniSection> sections, string name)
    {
        var section = sections.FirstOrDefault(m => m.Name == name);
        if (section != null)
            return section;

        section = new IniSection(name);
        sections.Add(section);
        return section;
    }
}
