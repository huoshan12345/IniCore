namespace IniCore;

public static class Extensions
{
    public static IniToken Make(this IniTokenType type, string value)
    {
        return new(type, value, 0, 0, 0);
    }

    public static TResult Pipe<T, TResult>(this T value, Func<T, TResult> func) => func(value);
}

public static class IniTokens
{
    public static IniToken[] Entry(string key, string value)
    {
        return [IniTokenType.Key.Make(key), IniTokenType.Value.Make(value)];
    }

    public static IniToken Comment(string value) => IniTokenType.Comment.Make(value);
    public static IniToken SectionName(string value) => IniTokenType.SectionName.Make(value);
}