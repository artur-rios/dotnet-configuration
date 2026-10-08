# ArturRios.Configuration

[![Docs](https://img.shields.io/badge/docs-website-blue)](https://artur-rios.github.io/dotnet-configuration)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/artur-rios/dotnet-configuration/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/ArturRios.Configuration.svg)](https://www.nuget.org/packages/ArturRios.Configuration)

Lightweight, composable configuration loader for .NET. Load settings from JSON files (including appsettings),
environment variables, .env files, and merge them with clear precedence. Built on Microsoft.Extensions.Configuration
with a simple, focused API.

- Targets: .NET 10.0 (net10.0)
- NuGet: ArturRios.Configuration
- Minimal dependencies: `ArturRios.Extensions`, `DotNetEnv`, `Microsoft.Extensions.*`.

## Features

- Unified loader: `ConfigurationLoader` to compose multiple sources.
- Providers:
  - `EnvironmentProvider` reads OS environment variables as bool, int, string or a JSON-deserialized object.
  - `SettingsProvider` reads the same typed values from any `IConfiguration`. `GetObject<T>` deserializes a value
    that is itself a JSON string; an appsettings section has no value of its own and reads as `null`, so bind
    sections with `configuration.GetSection(key).Get<T>()` instead.
- Shared enums: `ConfigurationSourceType`, `DataFormatType`, `DataSource`, `EnvironmentType`, `OutputType`.
- Built on `Microsoft.Extensions.Configuration`, supports JSON, environment variables, .env files.
- Simple precedence model: later-added sources override earlier ones.
- Extensible: implement your own provider or source.

## Installation

NuGet CLI:

```cmd
nuget install ArturRios.Configuration
```

Dotnet CLI:

```cmd
dotnet add package ArturRios.Configuration
```

PackageReference:

```xml
<ItemGroup>
  <PackageReference Include="ArturRios.Configuration" Version="x.y.z" />
</ItemGroup>
```

Git submodule (alternative):

```cmd
git submodule add https://github.com/artur-rios/dotnet-configuration.git external/dotnet-configuration
```

Then add a project reference:

```xml
<ItemGroup>
  <ProjectReference Include="external/dotnet-configuration/src/ArturRios.Configuration.csproj" />
</ItemGroup>
```

## Quickstart

Load environment variables from `.env` and appsettings JSON for a given environment:

```csharp
using ArturRios.Configuration.Loaders;
using Microsoft.Extensions.Configuration;

// Choose your environment name: Local, Development, Staging, Production
var environmentName = "Development";

// Load environment variables from Environments/.env.<environment> (falls back to .env.local)
var envLoader = new ConfigurationLoader(environmentName);
envLoader.LoadEnvironment();

// Build appsettings for the environment from Settings/appsettings.<environment>.json (falls back to appsettings.local.json)
var builder = new ConfigurationBuilder();
var settingsLoader = new ConfigurationLoader(builder, environmentName);
settingsLoader.LoadAppSettings();

var configuration = builder.Build();
var connectionString = configuration["ConnectionStrings:Default"];
```

Use SettingsProvider to read typed values from configuration:

```csharp
using ArturRios.Configuration.Providers;

var settings = new SettingsProvider(configuration);
var retries = settings.GetInt("Http:Retries");
var featureEnabled = settings.GetBool("Features:NewUX");
var apiKey = settings.GetString("Api:Key");
```

Read values from OS environment variables with EnvironmentProvider:

```csharp
using ArturRios.Configuration.Providers;

var env = new EnvironmentProvider();
var port = env.GetInt("PORT");
var loggingJson = env.GetString("LOGGING__JSON");
```

## Advanced usage

- Folder conventions used by the loader, relative to the application's base directory
  (`AppDomain.CurrentDomain.BaseDirectory`) unless you pass a `basePath` to the constructor:
  - `.env` files under `Environments/.env.<EnvironmentName>`, fallback to `Environments/.env.local`.
  - `appsettings` JSON under `Settings/appsettings.<EnvironmentName>.json`, fallback to
      `Settings/appsettings.local.json`.

  The files must therefore be copied to the build output (for example with `CopyToOutputDirectory`) when the
  default base directory is used.

File names are matched **without regard to case**, so `.env.Development` and `.env.development` — or
`appsettings.Local.json` and `appsettings.local.json` — are equally acceptable on every platform. Matching
on exact case only resolved on Windows and silently found nothing on a case-sensitive file system.

- Precedence: when building `IConfiguration`, sources are added in the order you call them on the same
  `IConfigurationBuilder`. JSON files added later override earlier ones.
- `.env` files never override the real environment: `LoadEnvironment()` loads one file (`.env.<EnvironmentName>`,
  else `.env.local`) and leaves every variable the process already has untouched, so values injected by the
  deployment win over a shipped `.env` file. Within the file, a repeated key keeps its first value, and `${VAR}`
  resolves to the value in effect. See the docs' Advanced usage page.
- Binding to POCOs via Microsoft.Extensions.Configuration:

```csharp
public sealed class AppSettings
{
    public string? Name { get; set; }
    public ConnectionStrings ConnectionStrings { get; set; } = new();
}

public sealed class ConnectionStrings
{
    public string? Default { get; set; }
}

var builder = new ConfigurationBuilder();
// add any other sources here; LoadAppSettings adds the environment-specific file
new ConfigurationLoader(builder, "Development").LoadAppSettings();

var configuration = builder.Build();
var settings = configuration.Get<AppSettings>();
```

## API overview

- `ConfigurationLoader` (in `src/Loaders/ConfigurationLoader.cs`): `LoadEnvironment()` loads the `.env` file into the
  process environment, and `LoadAppSettings()` adds `appsettings.<env>.json` to the `IConfigurationBuilder` passed to
  the constructor, both following the folder conventions.
- `EnvironmentProvider` (in `src/Providers/EnvironmentProvider.cs`): read and parse OS environment variables to
  bool/int/string/object.
- `SettingsProvider` (in `src/Providers/SettingsProvider.cs`): read and parse configuration values to
  bool/int/string/object. An object is deserialized from a JSON string value, not bound from a section.
- Enums (in `src/Enums/`):
  - `ConfigurationSourceType`, `DataFormatType`, `DataSource`, `EnvironmentType`, `OutputType`.

## Extensibility

Create a custom source or provider:

1. Define a new provider class that implements `IConfigurationProvider` (in `src/Providers/Interfaces/`).
2. Add further sources to the same `IConfigurationBuilder` you passed to `ConfigurationLoader`; the loader does not
   expose the builder, so extend the builder rather than the loader.
3. Respect precedence by adding sources in order.

Example sketch:

```csharp
public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddMySource(this IConfigurationBuilder builder, string endpoint)
    {
        // fetch data from endpoint and add it as a source, e.g. builder.AddInMemoryCollection(values)
        // ...
        return builder;
    }
}
```

## Acknowledgements

- Built on Microsoft.Extensions.Configuration
- Uses DotNetEnv for .env support
- Thanks to the .NET OSS community

## Changelog

Notable changes in each release are recorded in [CHANGELOG.md](https://github.com/artur-rios/dotnet-configuration/blob/main/CHANGELOG.md). Releases follow
[Semantic Versioning](https://semver.org/).

## Contributing

Building from source, running the tests, the branching model and the release process are described in
[CONTRIBUTING.md](https://github.com/artur-rios/dotnet-configuration/blob/main/CONTRIBUTING.md).

## Legal Details

This project is licensed under the [MIT License](https://en.wikipedia.org/wiki/MIT_License). A copy of the license is
available at [LICENSE](https://github.com/artur-rios/dotnet-configuration/blob/main/LICENSE) in the repository.
