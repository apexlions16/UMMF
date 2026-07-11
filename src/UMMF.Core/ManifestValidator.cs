using UMMF.Contracts;

namespace UMMF.Core;

public static class ManifestValidator
{
    public static IReadOnlyList<string> Validate(ModManifest manifest)
    {
        if (manifest is null)
        {
            throw new ArgumentNullException(nameof(manifest));
        }

        var errors = new List<string>();
        if (manifest.SchemaVersion != 1)
        {
            errors.Add("Only schemaVersion 1 is supported.");
        }

        if (string.IsNullOrWhiteSpace(manifest.Id))
        {
            errors.Add("The mod id is required.");
        }

        if (string.IsNullOrWhiteSpace(manifest.Name))
        {
            errors.Add("The mod name is required.");
        }

        if (string.IsNullOrWhiteSpace(manifest.Version))
        {
            errors.Add("The mod version is required.");
        }

        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var replacement in manifest.Replacements)
        {
            if (string.IsNullOrWhiteSpace(replacement.Id))
            {
                errors.Add("Every replacement requires an id.");
            }
            else if (!ids.Add(replacement.Id))
            {
                errors.Add($"Duplicate replacement id: {replacement.Id}");
            }

            if (replacement.Kind == MediaAssetKind.Unknown)
            {
                errors.Add($"Replacement '{replacement.Id}' must declare a media kind.");
            }

            if (replacement.Match is null)
            {
                errors.Add($"Replacement '{replacement.Id}' requires a match fingerprint.");
            }

            if (string.IsNullOrWhiteSpace(replacement.Replacement) &&
                string.IsNullOrWhiteSpace(replacement.SubtitleText) &&
                string.IsNullOrWhiteSpace(replacement.Voice))
            {
                errors.Add($"Replacement '{replacement.Id}' does not change any media.");
            }

            if (replacement.DelayMilliseconds < 0)
            {
                errors.Add($"Replacement '{replacement.Id}' has a negative delay.");
            }

            if (replacement.Volume < 0.0f || replacement.Volume > 2.0f)
            {
                errors.Add($"Replacement '{replacement.Id}' volume must be between 0 and 2.");
            }
        }

        return errors;
    }
}
