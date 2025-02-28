namespace IniCore;

/// <summary>
/// Represents a token in the lexical analysis of INI content, containing type, value, and position information.
/// </summary>
[DebuggerDisplay("{Type}, {Value}")]
public record IniToken(
    IniTokenType Type,
    string Value,
    int Position,
    int Line,
    int Column)
{
    public override string ToString()
    {
        return $"{nameof(IniToken)}({Type}, {Value})";
    }
}
