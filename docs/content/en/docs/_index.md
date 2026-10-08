---
title: Documentation
linkTitle: Documentation
weight: 20
description: >-
  Lightweight, composable configuration loader for .NET.
---

Lightweight, composable configuration loader for .NET. Load settings from JSON files (including appsettings),
environment variables, .env files, and merge them with clear precedence. Built on
`Microsoft.Extensions.Configuration` with a simple, focused API.

- Targets: .NET 10.0 (net10.0)
- NuGet: ArturRios.Configuration
- Minimal dependencies: `ArturRios.Extensions`, `DotNetEnv`, `Microsoft.Extensions.*`.

## Features

- Unified loader: `ConfigurationLoader` to compose multiple sources.
- Providers:
  - `EnvironmentProvider` reads OS environment variables as bool, int, string or a JSON-deserialized object.
  - `SettingsProvider` reads the same typed values from any `IConfiguration`.
- Shared enums: `ConfigurationSourceType`, `DataFormatType`, `DataSource`, `EnvironmentType`, `OutputType`.
- Built on `Microsoft.Extensions.Configuration`, supports JSON, environment variables, .env files.
- Simple precedence model: later-added sources override earlier ones.
- Extensible: implement your own provider or source.

## Acknowledgements

- Built on Microsoft.Extensions.Configuration
- Uses DotNetEnv for .env support
- Thanks to the .NET OSS community

## Legal Details

This project is licensed under the [MIT License](https://en.wikipedia.org/wiki/MIT_License). A copy of the license is
available at [LICENSE](https://github.com/artur-rios/dotnet-configuration/blob/main/LICENSE) in the repository.
