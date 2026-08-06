---
title: API overview
weight: 40
description: >-
  The loader, the providers and the enums that make up the public API.
---

- `ConfigurationLoader` (in `src/Loaders/ConfigurationLoader.cs`): fluent-style helpers to add `.env` and
  `appsettings.<env>.json` using folder conventions, backing `IConfigurationBuilder`.
- `EnvironmentProvider` (in `src/Providers/EnvironmentProvider.cs`): read and parse OS environment variables to
  bool/int/string/object.
- `SettingsProvider` (in `src/Providers/SettingsProvider.cs`): read and parse configuration values to
  bool/int/string/object.
- Enums (in `src/Enums/`):
  - `ConfigurationSourceType`, `DataFormatType`, `EnvironmentType`, `OutputType`.
