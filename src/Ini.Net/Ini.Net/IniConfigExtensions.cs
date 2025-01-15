namespace Ini.Net;

public static class IniConfigExtensions
{
    public static JsonObject ToStructuredJsonObject(this IniConfig config)
    {
        return CreateStructuredJsonObject(config.Entries, config.Sections);
    }

    private static JsonObject CreateStructuredJsonObject(List<IniEntry> entries, List<IniSection> sections)
    {
        var jsonObject = new JsonObject();
        foreach (var (key, value) in entries)
        {
            if (jsonObject.TryGetPropertyValue(key, out var token))
            {
                if (token is JsonArray array)
                {
                    array.Add(value);
                }
                else
                {
                    jsonObject.Remove(key);
                    jsonObject[key] = new JsonArray(token, value);
                }
            }
            else
            {
                jsonObject[key] = value;
            }
        }

        foreach (var section in sections)
        {
            jsonObject[section.Name] = CreateStructuredJsonObject(section.Entries, section.SubSections);
        }
        return jsonObject;
    }

    private static (List<IniEntry>, List<IniSection>) CreatePartialSection(JsonObject jsonObject)
    {
        var entries = new List<IniEntry>();
        var sections = new List<IniSection>();

        foreach (var (key, value) in jsonObject)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (value?.GetValueKind())
            {
                case JsonValueKind.Object:
                {
                    var (subEntries, subSections) = CreatePartialSection((JsonObject)value);
                    var section = new IniSection(key) { Entries = subEntries, SubSections = subSections };
                    sections.Add(section);
                    break;
                }
                case JsonValueKind.Array:
                {
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var v in (JsonArray)value)
                    {
                        var entry = new IniEntry(key, GetValue(v));
                        entries.Add(entry);
                    }
                    break;
                }
                default:
                {
                    var entry = new IniEntry(key, GetValue(value));
                    entries.Add(entry);
                    break;
                }
            }
        }
        return (entries, sections);
    }

    private static string GetValue(JsonNode? token)
    {
        if (token is null)
            return "";

        var kind = token.GetValueKind();
        return kind switch
        {
            JsonValueKind.Null => "",
            JsonValueKind.String => token.GetValue<string>() ?? "",
            _ => token.ToJsonString(),
        };
    }

    public static IniConfig ToIniConfig(this JsonObject jsonObject)
    {
        var (entries, sections) = CreatePartialSection(jsonObject);
        return new IniConfig
        {
            Entries = entries,
            Sections = sections,
        };
    }
}