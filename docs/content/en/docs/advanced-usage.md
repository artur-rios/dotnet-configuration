---
title: Advanced usage
weight: 30
description: >-
  Folder conventions, source precedence and binding configuration to POCOs.
---

## Folder conventions

Conventions used by the loader:

- `.env` files under `Environments/.env.<EnvironmentName>`, fallback to `Environments/.env.local`.
- `appsettings` JSON under `Settings/appsettings.<EnvironmentName>.json`, fallback to
  `Settings/appsettings.local.json`.

## Precedence

When building `IConfiguration`, sources are added in the order you call them on the same
`IConfigurationBuilder`. JSON files added later override earlier ones.

## Binding to POCOs

Binding works through Microsoft.Extensions.Configuration:

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
