---
title: Advanced usage
weight: 30
description: >-
  Folder conventions, source precedence and binding configuration to POCOs.
---

## Folder conventions

Conventions used by the loader, relative to the application's base directory
(`AppDomain.CurrentDomain.BaseDirectory`) unless you pass a `basePath` to the constructor:

- `.env` files under `Environments/.env.<EnvironmentName>`, fallback to `Environments/.env.local`.
- `appsettings` JSON under `Settings/appsettings.<EnvironmentName>.json`, fallback to
  `Settings/appsettings.local.json`.

The files must therefore be copied to the build output (for example with `CopyToOutputDirectory`) when the
default base directory is used.

File names are matched **without regard to case**, so `.env.Development` and `.env.development` — or
`appsettings.Local.json` and `appsettings.local.json` — are equally acceptable on every platform. Matching
on exact case only resolved on Windows and silently found nothing on a case-sensitive file system.


## Precedence

When building `IConfiguration`, sources are added in the order you call them on the same
`IConfigurationBuilder`. JSON files added later override earlier ones.

### `.env` files and the real environment

`LoadEnvironment()` loads **one** file: `Environments/.env.<EnvironmentName>` when it exists, otherwise
`Environments/.env.local`. The two are never layered, so a key that is only in `.env.local` is not loaded when
the environment has its own file.

**Variables the process already has win over the file** (since 1.3.0). A key that is already set — by the
container, the orchestrator, the shell, CI — is left alone, so a `.env` file shipped with the build can't
override what the deployment injected. That matters most for the `.env.local` fallback, which is loaded for any
environment that has no file of its own, Production included. Two consequences of this "never overwrite" rule
(DotNetEnv's `clobberExistingVars: false`):

- A key repeated within one file keeps its **first** value: once the first line has set it, the later ones find
  it already set. Before 1.3.0 the last one won. Don't repeat keys.
- A `${VAR}` reference in the file resolves to the value actually in effect: with `DB_HOST` injected by the
  deployment and `DB_URL=postgres://${DB_HOST}/app` in the file, `DB_URL` points at the injected host.

To let the file win for a key, unset it in the environment before calling `LoadEnvironment()`.

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

var builder = new ConfigurationBuilder();
// add any other sources here; LoadAppSettings adds the environment-specific file
new ConfigurationLoader(builder, "Development").LoadAppSettings();

var configuration = builder.Build();
var settings = configuration.Get<AppSettings>();
```
