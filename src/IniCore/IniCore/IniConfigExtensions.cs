namespace IniCore;

public static class IniConfigExtensions
{
    /// <summary>
    /// Converts an IniConfig object to a serializable JsonObject.
    /// </summary>
    /// <param name="config">The IniConfig instance to convert.</param>
    /// <returns>A JsonObject representation of the INI configuration.</returns>
    public static JsonObject ToSerializableJsonObject(this IniConfig config)
    {
        return CreateJsonObject(config.Entries, config.Sections);
    }

    private static JsonObject CreateJsonObject(List<IniEntry> entries, List<IniSection> sections)
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
            jsonObject[section.Name] = CreateJsonObject(section.Entries, section.SubSections);
        }
        return jsonObject;
    }

    private static (List<IniEntry>, List<IniSection>) CreatePartialSection(JsonObject jsonObject)
    {
        var entries = new List<IniEntry>();
        var sections = new List<IniSection>();

        foreach (var pair in jsonObject)
        {
            var (key, value) = (pair.Key, pair.Value);

            switch (value)
            {
                case JsonObject obj:
                {
                    var (subEntries, subSections) = CreatePartialSection(obj);
                    var section = new IniSection(key) { Entries = subEntries, SubSections = subSections };
                    sections.Add(section);
                    break;
                }
                case JsonArray array:
                {
                    entries.AddRange(array.Select(v => new IniEntry(key, GetValue(v))));
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
        return token switch
        {
            null => "",
            JsonValue value when value.TryGetValue<string>(out var str) => str,
            _ => token.ToJsonString()
        };
    }

    /// <summary>
    /// Converts a JsonObject to an IniConfig object.
    /// </summary>
    /// <param name="jsonObject">The JsonObject to convert to INI configuration.</param>
    /// <returns>An IniConfig instance representing the JSON structure.</returns>
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