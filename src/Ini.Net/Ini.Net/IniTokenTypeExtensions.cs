namespace Ini.Net;

public static class IniTokenTypeExtensions
{
    public static bool IsIdentifier(this IniTokenType type)
    {
        return type is IniTokenType.Key || type.IsSectionNamePart();
    }

    public static bool IsSectionNamePart(this IniTokenType type)
    {
        return type is IniTokenType.SectionName or IniTokenType.SubsectionName;
    }
}