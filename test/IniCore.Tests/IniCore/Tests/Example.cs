// ReSharper disable SuggestVarOrType_SimpleTypes

using System.Text.Json.Nodes;

namespace IniCore.Tests;

// custom config type
public class CustomConfig
{
    [JsonPropertyName("username")]
    public string? UserName { get; set; }

    [JsonPropertyName("server")]
    public Server? Server { get; set; }
}

public class Server
{
    [JsonPropertyName("timeout")]
    public int Timeout { get; set; }

    [JsonPropertyName("database")]
    public ServerDatabase? Database { get; set; }
}

public class ServerDatabase
{
    [JsonPropertyName("host")]
    public string? Host { get; set; }
    [JsonPropertyName("port")]
    public int Port { get; set; }

    [JsonPropertyName("cache")]
    public ServerDatabaseCache? Cache { get; set; }
}

public class ServerDatabaseCache
{
    [JsonConverter(typeof(BooleanJsonConverter))]
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }
}

public class Example
{
    [Fact]
    public void Test()
    {
        var config2 = new IniConfig
        {
            Entries =
            [
                new IniEntry("debug", "true"),
            ],
            Sections =
            [
                new IniSection("server")
                {
                    SubSections =
                    [
                        new IniSection("database")
                        {
                            Entries =
                            [
                                new IniEntry("host", "localhost"),
                                new IniEntry("port", "1433")
                            ],
                        },
                    ],
                },
            ],
        };


        // ReSharper disable once UseRawString
        const string iniContent = @"
username = admin

[server]
timeout = 30

[server.database]
host = localhost
port = 1433

[.cache]
enabled = true";


        IniConfig config = IniParser.Parse(iniContent);
        // convert UciConfig to JsonObject that is able to deserialize to custom type.
        JsonObject jsonObject = config.ToSerializableJsonObject();
        JsonSerializerOptions serializerOptions = new() { NumberHandling = JsonNumberHandling.AllowReadingFromString };
        CustomConfig? customConfig = jsonObject.Deserialize<CustomConfig>(serializerOptions);

        Assert.NotNull(customConfig);
        Assert.Equal("admin", customConfig.UserName);

        Server? server = customConfig.Server;

        Assert.NotNull(server);
        Assert.Equal(30, server.Timeout);

        Assert.NotNull(server.Database);
        Assert.Equal("localhost", server.Database.Host);
        Assert.Equal(1433, server.Database.Port);

        Assert.NotNull(server.Database.Cache);
        Assert.True(server.Database.Cache.Enabled);
    }
}