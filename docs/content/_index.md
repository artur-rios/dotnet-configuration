+++
title = 'Dotnet Configuration'
+++

# Documentation

Lightweight, composable configuration loader for .NET. Load settings from JSON files (including appsettings),
environment variables, .env files, and merge them with clear precedence. Built on Microsoft.Extensions.Configuration
with a simple, focused API.

- Targets: .NET 10.0 (net10.0)
- NuGet: ArturRios.Configuration
- Minimal dependencies: `ArturRios.Extensions`, `DotNetEnv`, `Microsoft.Extensions.*`.

## Features

- Unified loader: `ConfigurationLoader` to compose multiple sources.
- Providers:
    - `EnvironmentProvider` for environment-specific logic (e.g., Development/Production).
    - `SettingsProvider` for layered settings.
- Source types and formats via enums: `ConfigurationSourceType`, `DataFormatType`, `EnvironmentType`, `OutputType`.
- Built on `Microsoft.Extensions.Configuration`, supports JSON, environment variables, .env files.
- Simple precedence model: later-added sources override earlier ones.
- Extensible: implement your own provider or source.

## Installation

NuGet CLI:

```
nuget install ArturRios.Configuration
```

Dotnet CLI:

```
dotnet add package ArturRios.Configuration
```

PackageReference:

```
<ItemGroup>
  <PackageReference Include="ArturRios.Configuration" Version="x.y.z" />
</ItemGroup>
```

Git submodule (alternative):

```
git submodule add https://github.com/artur-rios/dotnet-configuration.git external/dotnet-configuration
```

Then add a project reference:

```
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

- Folder conventions used by the loader:
    - `.env` files under `Environments/.env.<EnvironmentName>`, fallback to `Environments/.env.local`.
    - `appsettings` JSON under `Settings/appsettings.<EnvironmentName>.json`, fallback to
      `Settings/appsettings.local.json`.
- Precedence: when building `IConfiguration`, sources are added in the order you call them on the same
  `IConfigurationBuilder`. JSON files added later override earlier ones.
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

var configuration = new ConfigurationBuilder()
    // if you have additional files, add them here; LoadAppSettings adds the environment-specific file
    .Build();

// After LoadAppSettings on the builder
var settings = configuration.Get<AppSettings>();
```

## API overview

- `ConfigurationLoader` (in `src/Loaders/ConfigurationLoader.cs`): fluent-style helpers to add `.env` and
  `appsettings.<env>.json` using folder conventions, backing `IConfigurationBuilder`.
- `EnvironmentProvider` (in `src/Providers/EnvironmentProvider.cs`): read and parse OS environment variables to
  bool/int/string/object.
- `SettingsProvider` (in `src/Providers/SettingsProvider.cs`): read and parse configuration values to
  bool/int/string/object.
- Enums (in `src/Enums/`):
    - `ConfigurationSourceType`, `DataFormatType`, `EnvironmentType`, `OutputType`.

## Extensibility

Create a custom source or provider:

1. Define a new provider class (see `src/Providers/Interfaces/` for guidance).
2. Extend your configuration loader or builder with additional sources as needed.
3. Respect precedence by adding sources in order.

Example sketch:

```csharp
public static class ConfigurationLoaderExtensions
{
    public static ConfigurationLoader AddMySource(this ConfigurationLoader loader, string endpoint)
    {
        // fetch data from endpoint, add it to the underlying IConfigurationBuilder
        // ...
        return loader;
    }
}
```

## Contributing

- Issues and PRs are welcome. If you plan a larger change, open an issue first with a short proposal.
- Coding style: follow existing conventions; keep APIs small and focused.

## Build, test and publish

Use the official [.NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/) to build, test and publish the project
and Git for source control.
If you want, optional helper toolsets I built to facilitate these tasks are available:

- [Dotnet Tools](https://github.com/artur-rios/dotnet-tools)
- [Python Dotnet Tools](https://github.com/artur-rios/python-dotnet-tools)

## Versioning

Semantic Versioning (SemVer). Breaking changes result in a new major version. New methods or non-breaking behavior
changes increment the minor version; fixes or tweaks increment the patch.

## Legal Details

This project is licensed under the [MIT License](https://en.wikipedia.org/wiki/MIT_License). A copy of the license is
available at [LICENSE](https://github.com/artur-rios/dotnet-configuration/blob/main/LICENSE) in the repository.

## Acknowledgements

- Built on Microsoft.Extensions.Configuration
- Uses DotNetEnv for .env support
- Thanks to the .NET OSS community
