namespace System;

public static class TypeExtensions
{
    public static MethodInfo GetRequiredMethod(this Type type, string name)
    {
        return type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
               ?? throw new InvalidOperationException($"Cannot find method '{name}' in type '{type.FullName}'");
    }
}