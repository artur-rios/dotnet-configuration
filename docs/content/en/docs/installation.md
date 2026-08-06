---
title: Installation
weight: 10
description: >-
  Add ArturRios.Configuration to your project via NuGet or as a Git submodule.
---

## NuGet CLI

```cmd
nuget install ArturRios.Configuration
```

## Dotnet CLI

```cmd
dotnet add package ArturRios.Configuration
```

## PackageReference

```xml
<ItemGroup>
  <PackageReference Include="ArturRios.Configuration" Version="x.y.z" />
</ItemGroup>
```

## Git submodule (alternative)

```cmd
git submodule add https://github.com/artur-rios/dotnet-configuration.git external/dotnet-configuration
```

Then add a project reference:

```xml
<ItemGroup>
  <ProjectReference Include="external/dotnet-configuration/src/ArturRios.Configuration.csproj" />
</ItemGroup>
```
