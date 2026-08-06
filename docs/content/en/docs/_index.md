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
  - `EnvironmentProvider` for environment-specific logic (e.g., Development/Production).
  - `SettingsProvider` for layered settings.
- Source types and formats via enums: `ConfigurationSourceType`, `DataFormatType`, `EnvironmentType`, `OutputType`.
- Built on `Microsoft.Extensions.Configuration`, supports JSON, environment variables, .env files.
- Simple precedence model: later-added sources override earlier ones.
- Extensible: implement your own provider or source.
