# ZinfoFramework IdentityModel ASP.NET Core OAuth 2.0 Introspection

> [!IMPORTANT]
> This project is an independent continuation of the historical `IdentityModel.AspNetCore.OAuth2Introspection` project. It is maintained by Luiz Antonio under the ZinfoFramework package identity and is not affiliated with or maintained by the original project authors or Duende Software.

An ASP.NET Core authentication handler that validates access tokens through OAuth 2.0 token introspection, with optional response caching and extensibility events.

## Package transition

| Historical package | Independent continuation |
| --- | --- |
| `IdentityModel.AspNetCore.OAuth2Introspection` | `ZinfoFramework.IdentityModel.AspNetCore.OAuth2Introspection` |

Install version 10.0.0:

```shell
dotnet add package ZinfoFramework.IdentityModel.AspNetCore.OAuth2Introspection --version 10.0.0
```

Version 10.0.0 targets only `net10.0`. The assembly name and the `IdentityModel.AspNetCore.OAuth2Introspection` and `IdentityModel.Client` namespaces remain unchanged so existing source-level names continue to be recognizable.

## Breaking changes and migration

The NuGet package ID changed, and the package now has an exact dependency on `ZinfoFramework.IdentityModel` 10.0.0. Because public members expose types supplied by that dependency, switching from an earlier Duende-based release is a binary and source breaking change: target `net10.0`, replace the old package references, restore the ZinfoFramework packages, and recompile the application.

The following affected public API remains in the preserved namespaces but its referenced types now come from `ZinfoFramework.IdentityModel`:

- `OAuth2IntrospectionOptions.ClientCredentialStyle`
- `OAuth2IntrospectionOptions.AuthorizationHeaderStyle`
- `OAuth2IntrospectionOptions.DiscoveryPolicy`
- `SendingRequestContext.TokenIntrospectionRequest`
- `UpdateClientAssertionContext.ClientAssertion`

Typical migration:

1. Remove `IdentityModel.AspNetCore.OAuth2Introspection`, `Duende.AspNetCore.Authentication.OAuth2Introspection`, and old IdentityModel dependency references as applicable.
2. Add `ZinfoFramework.IdentityModel.AspNetCore.OAuth2Introspection` version 10.0.0.
3. Target .NET 10 and keep application imports for `IdentityModel.AspNetCore.OAuth2Introspection` and `IdentityModel.Client`.
4. Recompile and test code that configures client credentials, discovery, request events, or client assertions.

## Current capabilities

- OAuth 2.0 token introspection authentication for ASP.NET Core.
- Discovery-based or explicit introspection endpoints.
- Client secret and client assertion authentication options.
- Optional introspection response caching.
- Events for request customization, validation, and authentication failures.
- Configurable token retrieval and claim mapping.

## Project information

- [Changelog](https://github.com/Luizinfo/IdentityModel.AspNetCore.OAuth2Introspection/blob/main/CHANGELOG.md)
- [Contributing](https://github.com/Luizinfo/IdentityModel.AspNetCore.OAuth2Introspection/blob/main/CONTRIBUTING.md)
- [Security policy](https://github.com/Luizinfo/IdentityModel.AspNetCore.OAuth2Introspection/blob/main/SECURITY.md)
- [Support](https://github.com/Luizinfo/IdentityModel.AspNetCore.OAuth2Introspection/blob/main/SUPPORT.md)
- [License](https://github.com/Luizinfo/IdentityModel.AspNetCore.OAuth2Introspection/blob/main/LICENSE)
- [Notice and attribution](https://github.com/Luizinfo/IdentityModel.AspNetCore.OAuth2Introspection/blob/main/NOTICE.md)

The historical upstream is available at [IdentityModel/IdentityModel.AspNetCore.OAuth2Introspection](https://github.com/IdentityModel/IdentityModel.AspNetCore.OAuth2Introspection). The separate Duende successor may be useful to users who prefer that product line, but it is not the operational home of this independent continuation.

## Build and pull-request validation

Install the exact .NET SDK **10.0.401** selected by `global.json`. Run `./build.ps1` on Windows or `./build.sh` on Linux/macOS from the repository root. The default build compiles the solution, runs its tests, and creates local validation packages. CI installs the same SDK from `global.json` and checks Windows, Linux, and macOS. It runs for pull requests and correction/release branches without publishing packages.

Unreleased build changes do not replace the existing NuGet release. A future publication requires a new package version and validation of the final artifacts.
