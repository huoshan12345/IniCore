namespace Ini.Net;

public static class ObjectExtensions
{
    public static void AppendList<T>(this StringBuilder builder, IEnumerable<T> enumerable, Func<T, string?>? printItem = null)
    {
        printItem ??= m => m?.ToString();

        builder.Append('[');
        foreach (var (_, m, _, isLast) in enumerable.IndexExt())
        {
            builder.Append(printItem(m));
            if (isLast == false)
                builder.Append(", ");
        }
        builder.Append(']');
    }
}