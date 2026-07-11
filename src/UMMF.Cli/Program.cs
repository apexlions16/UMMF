using System.Text.Json;
using System.Text.Json.Serialization;
using UMMF.Contracts;
using UMMF.Core;

namespace UMMF.Cli;

internal static class Program
{
    private const string PreviewVersion = "0.1.0-preview.1";

    private static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintHelp();
            return 0;
        }

        return args[0].ToLowerInvariant() switch
        {
            "info" or "--version" or "-v" => PrintInfo(),
            "validate" => ValidateManifest(args),
            "identity-demo" => RunIdentityDemo(),
            "match-demo" => RunMatchDemo(),
            "help" or "--help" or "-h" => PrintHelpAndReturn(),
            _ => UnknownCommand(args[0])
        };
    }

    private static int PrintInfo()
    {
        Console.WriteLine($"UMMF {PreviewVersion}");
        Console.WriteLine("Universal Media Mod Framework preview CLI");
        Console.WriteLine("Runtime Unity/BepInEx integration is not included in this preview yet.");
        return 0;
    }

    private static int ValidateManifest(string[] args)
    {
        if (args.Length != 2)
        {
            Console.Error.WriteLine("Usage: ummf validate <path-to-mod.json>");
            return 2;
        }

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            options.Converters.Add(new JsonStringEnumConverter());

            var json = File.ReadAllText(args[1]);
            var manifest = JsonSerializer.Deserialize<ModManifest>(json, options);
            if (manifest is null)
            {
                Console.Error.WriteLine("INVALID: the file did not contain a manifest object.");
                return 1;
            }

            var errors = ManifestValidator.Validate(manifest);
            if (errors.Count > 0)
            {
                Console.Error.WriteLine("INVALID");
                foreach (var error in errors)
                {
                    Console.Error.WriteLine($"- {error}");
                }

                return 1;
            }

            Console.WriteLine("VALID");
            Console.WriteLine($"Mod: {manifest.Name} ({manifest.Id})");
            Console.WriteLine($"Version: {manifest.Version}");
            Console.WriteLine($"Replacements: {manifest.Replacements.Count}");
            foreach (var group in manifest.Replacements.GroupBy(item => item.Kind).OrderBy(group => group.Key))
            {
                Console.WriteLine($"- {group.Key}: {group.Count()}");
            }

            return 0;
        }
        catch (FileNotFoundException)
        {
            Console.Error.WriteLine($"File not found: {args[1]}");
            return 2;
        }
        catch (IOException exception)
        {
            Console.Error.WriteLine($"Could not read manifest: {exception.Message}");
            return 2;
        }
        catch (JsonException exception)
        {
            Console.Error.WriteLine($"INVALID JSON: {exception.Message}");
            return 1;
        }
    }

    private static int RunIdentityDemo()
    {
        var fingerprint = new AssetFingerprint
        {
            Kind = MediaAssetKind.Subtitle,
            StableKey = "Dialogue/Intro/Line_001",
            UsagePath = "Canvas/Dialogue/SubtitleText",
            SourceText = "We need to leave now."
        };

        Console.WriteLine(AssetIdentity.CreateStableId(fingerprint));
        return 0;
    }

    private static int RunMatchDemo()
    {
        var expected = new AssetFingerprint
        {
            Kind = MediaAssetKind.Texture,
            Name = "MainMenuLogo",
            UsagePath = "Canvas/MainMenu/Header/Logo",
            Width = 2048,
            Height = 1024,
            ContentHash = "old-build-hash"
        };

        var updatedGameAsset = new AssetFingerprint
        {
            Kind = MediaAssetKind.Texture,
            Name = "MainMenuLogo",
            UsagePath = "Canvas/MainMenu/Header/Logo",
            Width = 2048,
            Height = 1024,
            ContentHash = "new-build-hash"
        };

        var result = AssetMatcher.Compare(expected, updatedGameAsset);
        Console.WriteLine($"Confidence: {result.Confidence:0.00}");
        Console.WriteLine($"Reasons: {string.Join(", ", result.Reasons)}");
        Console.WriteLine(result.Confidence >= 0.75 ? "Decision: compatible candidate" : "Decision: requires review");
        return 0;
    }

    private static int PrintHelpAndReturn()
    {
        PrintHelp();
        return 0;
    }

    private static int UnknownCommand(string command)
    {
        Console.Error.WriteLine($"Unknown command: {command}");
        PrintHelp();
        return 2;
    }

    private static void PrintHelp()
    {
        Console.WriteLine($"UMMF {PreviewVersion}");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  info                         Show preview information");
        Console.WriteLine("  validate <mod.json>          Validate a UMMF mod manifest");
        Console.WriteLine("  identity-demo                Generate a stable subtitle asset ID");
        Console.WriteLine("  match-demo                   Demonstrate update-resilient matching");
        Console.WriteLine("  help                         Show this help");
    }
}
