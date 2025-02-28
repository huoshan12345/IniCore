namespace IniCore;

/// <summary>
/// Represents a complete INI configuration file, containing entries, sections, and comments.
/// </summary>
public class IniConfig
{
    /// <summary>
    /// Gets or sets the collection of top-level key-value entries that appear before any section in the INI file.
    /// </summary>
    public List<IniEntry> Entries { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of sections in the INI file, each containing its own entries and subsections.
    /// </summary>
    public List<IniSection> Sections { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of comments that appear at the top of the INI file, before any entries or sections.
    /// </summary>
    public List<string> FullLineComments { get; set; } = [];

    /// <summary>
    /// Converts the IniConfig instance to a string representation in INI format.
    /// </summary>
    /// <returns>A string containing the complete INI configuration.</returns>
    public override string ToString()
    {
        return StringBuilderHelper.Build(Render);
    }

    /// <summary>
    /// Renders the complete INI configuration to the provided StringBuilder.
    /// </summary>
    /// <param name="builder">The StringBuilder instance to append the INI content to.</param>
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