---
title: Extensibility
weight: 50
description: >-
  Add your own configuration sources and providers.
---

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
