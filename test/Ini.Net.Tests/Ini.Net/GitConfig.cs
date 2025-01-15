namespace Ini.Net;

[JsonConverter(typeof(GitConfigJsonConverter))]
public class GitConfig
{
    [JsonPropertyName("core")]
    public GitConfigCore? Core { get; set; }

    [JsonPropertyName("diff")]
    public GitConfigDiff? Diff { get; set; }

    [JsonPropertyName("include")]
    public GitConfigInclude? Include { get; set; }

    [JsonIgnore]
    public GitConfigBranch[]? Branches { get; set; }

    [JsonIgnore]
    public GitConfigIncludeIf[]? IncludeIfs { get; set; }

    [JsonIgnore]
    public GitConfigRemote[]? Remotes { get; set; }
}

public class GitConfigBranch
{
    [JsonIgnore]
    public string? Branch { get; set; }

    [JsonPropertyName("remote")]
    public string? Remote { get; set; }

    [JsonPropertyName("merge")]
    public string? Merge { get; set; }
}

public class GitConfigCore
{
    [JsonPropertyName("filemode")]
    public bool FileMode { get; set; }

    [JsonPropertyName("gitProxy")]
    public string[]? GitProxy { get; set; }
}

public class GitConfigDiff
{
    [JsonPropertyName("external")]
    public string? External { get; set; }

    [JsonPropertyName("renames")]
    public bool Renames { get; set; }
}

public class GitConfigInclude
{
    [JsonPropertyName("path")]
    public string[]? Path { get; set; }
}

public class GitConfigIncludeIf
{
    [JsonIgnore]
    public string? Branch { get; set; }

    [JsonConverter(typeof(ReadAsArrayJsonConverter))]
    [JsonPropertyName("path")]
    public string[]? Path { get; set; }
}

public class GitConfigRemote
{
    [JsonIgnore]
    public string? Branch { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

public class GitConfigJsonConverter : JsonConverter<GitConfig>
{
    private static readonly Regex _regMemberWithBranch = new("^(\\w+) \\\"(\\S+)\\\"$", RegexOptions.Compiled);


    public override GitConfig? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;
 
        var token = reader.ReadElement();
        var typeInfo = options.GetBuiltInJsonTypeInfo<GitConfig>();
        var config = token.Deserialize(typeInfo)!;

        List<GitConfigBranch>? branches = null;
        List<GitConfigIncludeIf>? includeIfs = null;
        List<GitConfigRemote>? remotes = null;

        foreach (var (key, value) in token.EnumerateObject())
        {
            if (value.ValueKind == JsonValueKind.Null)
                continue;

            if (!_regMemberWithBranch.TryMatch(key, out var match))
                continue;

            var member = match.Groups[1].Value;
            var branch = match.Groups[2].Value;

            switch (member)
            {
                case "branch":
                {
                    var item = value.Deserialize<GitConfigBranch>(options)!;
                    item.Branch = branch;
                    branches ??= [];
                    branches.Add(item);
                    break;
                }
                case "includeIf":
                {
                    var item = value.Deserialize<GitConfigIncludeIf>(options)!;
                    item.Branch = branch;
                    includeIfs ??= [];
                    includeIfs.Add(item);
                    break;
                }
                case "remote":
                {
                    var item = value.Deserialize<GitConfigRemote>(options)!;
                    item.Branch = branch;
                    remotes ??= [];
                    remotes.Add(item);
                    break;
                }
            }
        }

        config.Branches = branches?.AsSpan().ToArray();
        config.IncludeIfs = includeIfs?.AsSpan().ToArray();
        config.Remotes = remotes?.AsSpan().ToArray();

        return config;
    }

    public override void Write(Utf8JsonWriter writer, GitConfig value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
