namespace IniCore;

/// <summary>
/// Represents a section within an INI configuration file. Each section contains a name,
/// a collection of key-value entries, optional subsections, and optional comments.
/// </summary>
[DebuggerDisplay("{Name}")]
public class IniSection
{
    /// <summary>
    /// Gets or sets the name of the INI section. This appears in square brackets in an INI file.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the list of key-value pairs (entries) within the section.
    /// </summary>
    public List<IniEntry> Entries { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of subsections within this section. 
    /// Subsections can represent a hierarchical structure of INI configuration.
    /// </summary>
    public List<IniSection> SubSections { get; set; } = [];

    /// <summary>
    /// Gets or sets an inline comment, which can be placed on the same line as the section name.
    /// </summary>
    public string InlineComment { get; set; } = "";

    /// <summary>
    /// Gets or sets a list of full-line comments, appearing before the section declaration in the INI file.
    /// </summary>
    public List<string> FullLineComments { get; set; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="IniSection"/> class with default values.
    /// </summary>
    public IniSection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IniSection"/> class with a specified name and optional entries.
    /// </summary>
    /// <param name="name">The name of the section.</param>
    /// <param name="entries">Optional key-value pairs (entries) for the section.</param>
    public IniSection(string name, params IniEntry[] entries)
    {
        Name = name;
        Entries.AddRange(entries);
    }

    public override string ToString()
    {
        return StringBuilderHelper.Build(Render);
    }

    public void Render(StringBuilder builder)
    {
        var queue = new Queue<(string, List<IniEntry>, List<IniSection>)>();
        queue.Enqueue((Name, Entries, SubSections));

        while (queue.Count > 0)
        {
            var (name, entries, sections) = queue.Dequeue();

            builder.AppendSectionName(name);
            builder.AppendLine();
            builder.AppendEntries(entries);

            foreach (var section in sections)
            {
                var fullName = StringBuilderHelper.Build(m => m.Append(name).Append('.').Append(section.Name));
                queue.Enqueue((fullName, section.Entries, section.SubSections));
            }
        }
    }
}