namespace Ini.Net;

/// <summary>
/// Represents a single entry within an INI configuration file, consisting of a key-value pair.
/// Includes options for full-line comments and inline comments.
/// </summary>
[DebuggerDisplay("{Key}, {Value}")]
public class IniEntry : IRenderable
{
    /// <summary>
    /// Gets or sets the key of the INI entry.
    /// This is the identifier used to reference the entry's value.
    /// </summary>
    public string Key { get; set; } = "";

    /// <summary>
    /// Gets or sets the value associated with the <see cref="Key"/> in the INI entry.
    /// </summary>
    public string Value { get; set; } = "";

    /// <summary>
    /// Gets a list of comments that appear on lines preceding the entry.
    /// Each string in the list represents a full line of comment text.
    /// </summary>
    public List<string> FullLineComments { get; } = [];

    /// <summary>
    /// Gets or sets an inline comment, typically placed on the same line as the entry,
    /// following the key-value pair.
    /// </summary>
    public string InlineComment { get; set; } = "";

    /// <summary>
    /// Initializes a new instance of the <see cref="IniEntry"/> class with default values.
    /// </summary>
    public IniEntry() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IniEntry"/> class with the specified key and value.
    /// </summary>
    /// <param name="key">The key of the INI entry.</param>
    /// <param name="value">The value associated with the key.</param>
    public IniEntry(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public override string ToString()
    {
        return StringBuilderHelper.Build(Render);
    }

    public void Render(StringBuilder builder)
    {
        builder.Append(Key);
        builder.Append('=');
        builder.Append(Value);
    }

    public void Deconstruct(out string key, out string value)
    {
        key = Key;
        value = Value;
    }
}