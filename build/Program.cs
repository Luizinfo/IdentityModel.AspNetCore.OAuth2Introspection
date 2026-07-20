using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace build
{
    internal static class Program
    {
        private const string PackOutput = "./artifacts";
        private const string EnvironmentVariableMissing = " environment variable is missing. Aborting.";

        private static readonly HashSet<string> ExecutedTargets = new(StringComparer.OrdinalIgnoreCase);

        private static class Targets
        {
            public const string Restore = "restore";
            public const string RestoreTools = "restore-tools";
            public const string CleanBuildOutput = "clean-build-output";
            public const string CleanPackOutput = "clean-pack-output";
            public const string Build = "build";
            public const string Test = "test";
            public const string Pack = "pack";
            public const string SignPackage = "sign-package";
            public const string Default = "default";
            public const string Sign = "sign";
        }

        internal static int Main(string[] args)
        {
            try
            {
                var requestedTargets = args.Length == 0 ? new[] { Targets.Default } : args;
                foreach (var target in requestedTargets)
                {
                    ExecuteTarget(target);
                }

                return 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
                return 1;
            }
        }

        private static void ExecuteTarget(string target)
        {
            if (ExecutedTargets.Contains(target))
            {
                return;
            }

            switch (target)
            {
                case Targets.Restore:
                    Restore();
                    break;
                case Targets.RestoreTools:
                    RunDotNet("tool", "restore");
                    break;
                case Targets.CleanBuildOutput:
                    RunDotNet("clean", "-c", "Release", "-v", "m", "--nologo");
                    break;
                case Targets.CleanPackOutput:
                    if (Directory.Exists(PackOutput))
                    {
                        Directory.Delete(PackOutput, true);
                    }
                    break;
                case Targets.Build:
                    ExecuteTarget(Targets.Restore);
                    ExecuteTarget(Targets.CleanBuildOutput);
                    RunDotNet("build", "-c", "Release", "--no-restore", "--nologo");
                    break;
                case Targets.Test:
                    ExecuteTarget(Targets.Build);
                    RunDotNet("test", "-c", "Release", "--no-build", "--no-restore", "--nologo");
                    break;
                case Targets.Pack:
                    ExecuteTarget(Targets.Build);
                    ExecuteTarget(Targets.CleanPackOutput);
                    RunDotNet(
                        "pack",
                        "./src/IdentityModel.AspNetCore.OAuth2Introspection.csproj",
                        "-c",
                        "Release",
                        "-o",
                        Directory.CreateDirectory(PackOutput).FullName,
                        "--no-build",
                        "--no-restore",
                        "--nologo");
                    break;
                case Targets.SignPackage:
                    ExecuteTarget(Targets.Pack);
                    ExecuteTarget(Targets.RestoreTools);
                    SignNuGet();
                    break;
                case Targets.Default:
                    ExecuteTarget(Targets.Test);
                    ExecuteTarget(Targets.Pack);
                    break;
                case Targets.Sign:
                    ExecuteTarget(Targets.Test);
                    ExecuteTarget(Targets.SignPackage);
                    break;
                default:
                    throw new ArgumentException($"Unknown build target '{target}'.", nameof(target));
            }

            ExecutedTargets.Add(target);
        }

        private static void Restore()
        {
            var arguments = new List<string>
            {
                "restore",
                "--nologo"
            };
            var nugetConfigFile = Environment.GetEnvironmentVariable("NUGET_CONFIG_FILE");

            if (!string.IsNullOrWhiteSpace(nugetConfigFile))
            {
                arguments.Add("--configfile");
                arguments.Add(nugetConfigFile);
            }

            RunDotNet(arguments.ToArray());
        }

        private static void SignNuGet()
        {
            var signClientSecret = Environment.GetEnvironmentVariable("SignClientSecret");

            if (string.IsNullOrWhiteSpace(signClientSecret))
            {
                throw new InvalidOperationException($"SignClientSecret{EnvironmentVariableMissing}");
            }

            foreach (var file in Directory.GetFiles(PackOutput, "*.nupkg", SearchOption.AllDirectories))
            {
                Console.WriteLine($"  Signing {file}");

                RunDotNet(
                    noEcho: true,
                    "NuGetKeyVaultSignTool",
                    "sign",
                    file,
                    "--file-digest",
                    "sha256",
                    "--timestamp-rfc3161",
                    "http://timestamp.digicert.com",
                    "--azure-key-vault-url",
                    "https://duendecodesigning.vault.azure.net/",
                    "--azure-key-vault-client-id",
                    "18e3de68-2556-4345-8076-a46fad79e474",
                    "--azure-key-vault-tenant-id",
                    "ed3089f0-5401-4758-90eb-066124e2d907",
                    "--azure-key-vault-client-secret",
                    signClientSecret,
                    "--azure-key-vault-certificate",
                    "CodeSigning");
            }
        }

        private static void RunDotNet(params string[] arguments)
        {
            RunDotNet(noEcho: false, arguments);
        }

        private static void RunDotNet(bool noEcho, params string[] arguments)
        {
            if (!noEcho)
            {
                Console.WriteLine($"> dotnet {string.Join(' ', arguments.Select(QuoteArgument))}");
            }

            var startInfo = new ProcessStartInfo("dotnet")
            {
                UseShellExecute = false
            };

            foreach (var argument in arguments)
            {
                startInfo.ArgumentList.Add(argument);
            }

            using var process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Unable to start dotnet.");
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"dotnet exited with code {process.ExitCode}.");
            }
        }

        private static string QuoteArgument(string argument)
        {
            return argument.Any(char.IsWhiteSpace) ? $"\"{argument}\"" : argument;
        }
    }
}
