---
title: API overview
weight: 40
description: >-
  The loader, the providers and the enums that make up the public API.
---

- `ConfigurationLoader` (in `src/Loaders/ConfigurationLoader.cs`): `LoadEnvironment()` loads the `.env` file into the
  process environment, and `LoadAppSettings()` adds `appsettings.<env>.json` to the `IConfigurationBuilder` passed to
  the constructor, both following the folder conventions.
- `EnvironmentProvider` (in `src/Providers/EnvironmentProvider.cs`): read and parse OS environment variables to
  bool/int/string/object.
- `SettingsProvider` (in `src/Providers/SettingsProvider.cs`): read and parse configuration values to
  bool/int/string/object. An object is deserialized from a JSON string value, not bound from a section; bind
  sections with `configuration.GetSection(key).Get<T>()`.
- Enums (in `src/Enums/`):
  - `ConfigurationSourceType`, `DataFormatType`, `DataSource`, `EnvironmentType`, `OutputType`.
