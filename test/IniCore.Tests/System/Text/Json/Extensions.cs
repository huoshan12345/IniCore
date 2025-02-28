namespace System.Text.Json;

public static class Extensions
{
    private static readonly Type _resolver = typeof(DefaultJsonTypeInfoResolver);
    private static readonly MethodInfo _getBuiltInConverter = _resolver.GetRequiredMethod("GetBuiltInConverter");
    private static readonly MethodInfo _createTypeInfoCore = _resolver.GetRequiredMethod("CreateTypeInfoCore");

    // this method will create actual converter for JsonConverterFactory.
    private static readonly MethodInfo _expandConverterFactory = typeof(JsonSerializerOptions).GetRequiredMethod("ExpandConverterFactory");
    
    public static JsonNode? ToJsonNode(this JsonElement element, JsonNodeOptions? options = null)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Array => JsonArray.Create(element, options),
            JsonValueKind.Object => JsonObject.Create(element, options),
            _ => JsonValue.Create(element, options),
        };
    }

    /// <summary>
    /// Get <see cref="JsonTypeInfo{T}" /> with built-in <see cref="JsonConverter{T}" /> for the type <typeparamref name="T"/> to convert.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="options"></param>
    /// <returns></returns>
    public static JsonTypeInfo<T> GetBuiltInJsonTypeInfo<T>(this JsonSerializerOptions options)
    {
        return (JsonTypeInfo<T>)options.GetBuiltInJsonTypeInfo(typeof(T));
    }

    /// <summary>
    /// Get <see cref="JsonTypeInfo" /> with built-in <see cref="JsonConverter" /> for the type to convert.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="typeToConvert"></param>
    /// <returns></returns>
    public static JsonTypeInfo GetBuiltInJsonTypeInfo(this JsonSerializerOptions options, Type typeToConvert)
    {
        var converter = _getBuiltInConverter.Invoke(null, [typeToConvert]);
        converter = _expandConverterFactory.Invoke(options, [converter, typeToConvert]);
        var typeInfo = _createTypeInfoCore.Invoke(null, [typeToConvert, converter, options]);
        return (JsonTypeInfo)typeInfo!;
    }
}