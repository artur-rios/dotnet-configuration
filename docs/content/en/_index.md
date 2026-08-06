---
title: Dotnet Configuration
linkTitle: Home
---

{{< blocks/cover title="Dotnet Configuration" height="auto" color="primary" >}}
<p class="lead mt-4">
Lightweight, composable configuration loader for .NET.
</p>
<a class="btn btn-lg btn-secondary me-3 mb-4" href="docs/">
  Documentation <i class="fas fa-arrow-alt-circle-right ms-2"></i>
</a>
<a class="btn btn-lg btn-secondary me-3 mb-4" href="https://github.com/artur-rios/dotnet-configuration">
  GitHub <i class="fab fa-github ms-2"></i>
</a>
{{< /blocks/cover >}}

{{% blocks/lead color="light" %}}
Load settings from JSON files (including appsettings), environment variables and `.env` files, and merge
them with clear precedence. Built on `Microsoft.Extensions.Configuration` with a simple, focused API.
{{% /blocks/lead %}}

{{< blocks/section color="white" type="row" >}}
{{% blocks/feature icon="fa-solid fa-layer-group" title="Unified loader" %}}
`ConfigurationLoader` composes multiple sources with a simple precedence model: later-added sources
override earlier ones.
{{% /blocks/feature %}}

{{% blocks/feature icon="fa-solid fa-plug" title="Focused providers" %}}
`EnvironmentProvider` reads and parses OS environment variables; `SettingsProvider` reads typed values
from any `IConfiguration`.
{{% /blocks/feature %}}

{{% blocks/feature icon="fa-solid fa-feather" title="Minimal dependencies" %}}
Targets .NET 10.0 and depends only on `ArturRios.Extensions`, `DotNetEnv` and `Microsoft.Extensions.*`.
{{% /blocks/feature %}}
{{< /blocks/section >}}
