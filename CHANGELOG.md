# Changelog

All notable changes to `ArturRios.Configuration` are recorded in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- **Behavior change:** `ConfigurationLoader.LoadEnvironment()` no longer overwrites environment variables the
  process already has. Real environment variables (injected by the container, orchestrator, shell or CI) now win
  over the `.env` file; before, the file won, so a `.env.local` shipped with the build overrode deployment-injected
  values even in Production, via the fallback for environments without their own file. Within the file, a repeated
  key now keeps its first value instead of its last, and `${VAR}` resolves to the value in effect. Which file is
  loaded is unchanged: `.env.<Environment>`, else `.env.local`, never both. To let the file win for a key, unset
  it before loading. (DotNetEnv `clobberExistingVars: false`.)
- `SettingsProvider` rejects a null `IConfiguration` with `ArgumentNullException` when it is constructed, like
  `ConfigurationLoader` does for its builder, instead of failing with `NullReferenceException` on the first read.

### Fixed

- The documentation of `SettingsProvider.GetObject<T>` states that it deserializes a JSON string value and that an
  appsettings section, which has no value of its own, reads as `null`.

## [1.2.0] - 2026-08-24

### Changed

- `ConfigurationLoader` matches `.env` and `appsettings` file names without regard to case, so the documented
  `Environments/.env.<Environment>` and `Settings/appsettings.local.json` fallback conventions resolve on a
  case-sensitive file system. They silently found nothing there before, so an application started unconfigured.
- Both `ConfigurationLoader` constructors reject a null or whitespace environment name, and the builder overload
  rejects a null builder, at construction rather than composing a path that quietly finds nothing.
- `ArturRios.Extensions` updated from 1.3.0 to 1.4.0.

### Fixed

- The fallback console logger no longer leaks one undisposed `LoggerFactory` per `ConfigurationLoader` instance; a
  single factory is created lazily for the process.

## [1.1.0] - 2026-08-19

### Changed

- `ArturRios.Extensions` updated from 1.0.1 to 1.3.0, so consumers no longer resolve an outdated transitive dependency.
- `Microsoft.Extensions.*` references updated from 10.0.1 to 10.0.11, which the newer `ArturRios.Extensions` requires.

## [1.0.0] - 2025-12-12

### Added

- `ConfigurationLoader` to load `.env` files from `Environments/` and `appsettings` JSON from `Settings/` for a named
  environment, with a local fallback for each.
- `EnvironmentProvider` and `SettingsProvider` to read OS environment variables and configuration values as
  bool, int, string or object.
- `IConfigurationProvider`, the interface both providers implement, for writing your own.
- `ConfigurationSourceType`, `DataFormatType`, `DataSource`, `EnvironmentType` and `OutputType` enums.

[Unreleased]: https://github.com/artur-rios/dotnet-configuration/compare/1.2.0...HEAD
[1.2.0]: https://github.com/artur-rios/dotnet-configuration/compare/v1.1.0...1.2.0
[1.1.0]: https://github.com/artur-rios/dotnet-configuration/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/artur-rios/dotnet-configuration/releases/tag/v1.0.0
