namespace System;

public static class StringExtensions
{
    public static string LfToCrLf(this string input)
    {
        return input.Replace("\n", "\r\n");
    }

    public static string CrLfToLf(this string input)
    {
        return input.Replace("\r\n", "\n");
    }
}
