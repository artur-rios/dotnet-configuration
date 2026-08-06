---
title: Quickstart
weight: 20
description: >-
  Load .env files and appsettings for an environment, then read typed values.
---

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

Use `SettingsProvider` to read typed values from configuration:

```csharp
using ArturRios.Configuration.Providers;

var settings = new SettingsProvider(configuration);
var retries = settings.GetInt("Http:Retries");
var featureEnabled = settings.GetBool("Features:NewUX");
var apiKey = settings.GetString("Api:Key");
```

Read values from OS environment variables with `EnvironmentProvider`:

```csharp
using ArturRios.Configuration.Providers;

var env = new EnvironmentProvider();
var port = env.GetInt("PORT");
var loggingJson = env.GetString("LOGGING__JSON");
```
