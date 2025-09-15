# IniCore [![LICENSE](https://img.shields.io/github/license/mashape/apistatus.svg)](LICENSE.TXT) [![Build](https://github.com/huoshan12345/IniCore/actions/workflows/build.yml/badge.svg)](https://github.com/huoshan12345/IniCore/actions/workflows/build.yml)

A .NET library for parsing and generating INI configuration files with support for nested sections and JSON conversion.

## Features

- Parse and generate INI-formatted content
- Support nested sections using dot notation (e.g., `[parent.child]`)
- Handle relative section nesting (e.g., `[.child]` for relative hierarchy)
- Convert between INI structure and JSON objects
- Preserve entry order and section hierarchy
- Handle entries before any section declarations

||TargetFramework(s)|Package|
|----|----|----|
|IniCore|![netstandard2.0](https://img.shields.io/badge/netstandard-2.0-30a14e.svg) ![net8.0](https://img.shields.io/badge/net-8.0-30a14e.svg) ![net9.0](https://img.shields.io/badge/net-9.0-30a14e.svg) |[![](https://img.shields.io/nuget/v/IniCore?logo=nuget&label=nuget)](https://www.nuget.org/packages/IniCore)|

## Installation

```bash
dotnet add package IniCore
```

## Usage

### Parsing INI Content

```csharp
string iniContent = @"
[server.database]
host = localhost
port = 1433";

IniConfig config = IniParser.Parse(iniContent);

// Get database host value
string host = config.Sections
    .First(s => s.Name == "server")
    .SubSections.First(s => s.Name == "database")
    .Entries.First(e => e.Key == "host").Value;
```

### Creating Configurations Programmatically

```csharp
var config = new IniConfig
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

// Generate INI configuration string
string outputConfig = config.ToString();
Console.WriteLine(outputConfig);
```

### Deserialize UCI Configuration to custom config type using json

```ini
username = admin

[server]
timeout = 30

[server.database]
host = localhost
port = 1433

[.cache]
enabled = true
```

```csharp
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
    [JsonConverter(typeof(BooleanJsonConverter))] // use custom json converter
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }
}

IniConfig config = IniParser.Parse(iniContent);
// convert UciConfig to JsonObject that is able to deserialize to custom type.
JsonObject jsonObject = config.ToSerializableJsonObject();
JsonSerializerOptions serializerOptions = new() { NumberHandling = JsonNumberHandling.AllowReadingFromString };
CustomConfig? customConfig = jsonObject.Deserialize<CustomConfig>(serializerOptions);
```

## TODO

- [ ] Add support for reading and writing comments (current parser just ignores comments)

## License

MIT License

## Contributing

Contributions are welcome! Please open an issue or submit a pull request for any improvements or bug fixes.