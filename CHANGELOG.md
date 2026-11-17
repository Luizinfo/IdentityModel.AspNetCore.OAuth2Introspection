# Changelog

## Unreleased

- Pin local builds and CI to .NET SDK 10.0.401 through global.json.
- Validate pull requests on Windows, Linux, and macOS without package publication.
- Use the patched SourceLink implementation supplied by the SDK instead of the vulnerable Microsoft.Build.Tasks.Git 8.0.0 dependency.

All notable changes to this independent continuation are documented here.

## 10.0.0

- Published as `ZinfoFramework.IdentityModel.AspNetCore.OAuth2Introspection`.
- Targeted .NET 10 exclusively.
- Replaced the internal package dependency with the exact `ZinfoFramework.IdentityModel` version 10.0.0.
- Preserved the historical assembly name and public namespaces.
- Added current repository, attribution, support, security, and package documentation metadata.

See the [repository releases](https://github.com/Luizinfo/IdentityModel.AspNetCore.OAuth2Introspection/releases) for release artifacts and additional notes.
