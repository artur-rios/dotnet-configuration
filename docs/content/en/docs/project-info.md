---
title: Project info
weight: 60
description: >-
  Build, test and publish workflow, versioning policy and legal details.
---

## Acknowledgements

- Built on Microsoft.Extensions.Configuration
- Uses DotNetEnv for .env support
- Thanks to the .NET OSS community

## Testing

The test suite is xUnit, and every test is named with the Given / When / Then pattern. Every test class
carries a `Category` trait, so the two kinds can be run — and reported — separately:

```bash
dotnet test src/ArturRios.Configuration.sln --filter "Category=Unit"
dotnet test src/ArturRios.Configuration.sln --filter "Category=Functional"
```

Unit tests exercise the code in isolation against test doubles.
Functional tests drive the loader over real .env and appsettings files written to a temporary directory on disk.
CI runs the two as separate jobs, and both must pass before a pull request can be merged.

## Versioning

Semantic Versioning (SemVer). Breaking changes result in a new major version. New methods or non-breaking behavior
changes increment the minor version; fixes or tweaks increment the patch.

## Build, test and publish

Use the official [.NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/) to build, test and publish the project
and Git for source control.
If you want, optional helper toolsets I built to facilitate these tasks are available:

- [Dotnet Tools](https://github.com/artur-rios/dotnet-tools)
- [Python Dotnet Tools](https://github.com/artur-rios/python-dotnet-tools)

## Legal Details

This project is licensed under the [MIT License](https://en.wikipedia.org/wiki/MIT_License). A copy of the license is
available at [LICENSE](https://github.com/artur-rios/dotnet-configuration/blob/main/LICENSE) in the repository.
