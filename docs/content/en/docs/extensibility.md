---
title: Extensibility
weight: 50
description: >-
  Add your own configuration sources and providers.
---

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
