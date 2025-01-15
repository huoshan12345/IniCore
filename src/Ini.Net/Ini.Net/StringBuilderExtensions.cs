namespace Ini.Net;

internal static class StringBuilderExtensions
{
    public static StringBuilder AppendSectionName(this StringBuilder builder, string name)
    {
        builder.Append('[');
        builder.Append(name);
        builder.Append(']');
        return builder;
    }

    public static StringBuilder AppendEntries(this StringBuilder builder, List<IniEntry> entries)
    {
        foreach (var entry in entries)
        {
            entry.Render(builder);
            builder.AppendLine();
        }
        return builder;
    }
}