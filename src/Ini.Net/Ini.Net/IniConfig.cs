namespace Ini.Net;

public class IniConfig
{
    /// <summary>
    /// Top-level key-value pairs.
    /// </summary>
    public List<IniEntry> Entries { get; set; } = [];
    public List<IniSection> Sections { get; set; } = [];
    public List<string> FullLineComments { get; set; } = [];

    public override string ToString()
    {
        return StringBuilderHelper.Build(Render);
    }

    public void Render(StringBuilder builder)
    {
        builder.AppendEntries(Entries);

        if (Entries.Count > 0)
            builder.AppendLine();

        foreach (var (_, section, _, isLast) in Sections.IndexEx())
        {
            section.Render(builder);

            if (isLast == false)
                builder.AppendLine();
        }
    }
}