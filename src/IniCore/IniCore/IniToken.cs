namespace IniCore;

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
