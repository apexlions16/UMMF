using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UMMF.Contracts;

namespace UMMF.Core;

public static class AssetIdentity
{
    public static string CreateStableId(AssetFingerprint fingerprint)
    {
        if (fingerprint is null)
        {
            throw new ArgumentNullException(nameof(fingerprint));
        }

        var prefix = GetPrefix(fingerprint.Kind);
        if (!string.IsNullOrWhiteSpace(fingerprint.StableKey))
        {
            return $"{prefix}:key:{NormalizePath(fingerprint.StableKey)}";
        }

        var canonical = string.Join("|", new[]
        {
            fingerprint.Kind.ToString(),
            NormalizeText(fingerprint.Name),
            NormalizePath(fingerprint.Container),
            NormalizePath(fingerprint.UsagePath),
            fingerprint.Width?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            fingerprint.Height?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            fingerprint.DurationSeconds?.ToString("0.000", CultureInfo.InvariantCulture) ?? string.Empty,
            fingerprint.SampleRate?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            fingerprint.Channels?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            NormalizeText(fingerprint.SourceText)
        });

        using (var sha256 = SHA256.Create())
        {
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(canonical));
            var hex = BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            return $"{prefix}:auto:{hex.Substring(0, 16)}";
        }
    }

    internal static string NormalizePath(string? value)
    {
        return NormalizeText(value).Replace('\\', '/');
    }

    internal static string NormalizeText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var nonNullValue = value ?? string.Empty;
        return Regex.Replace(nonNullValue.Trim().ToLowerInvariant(), @"\s+", " ");
    }

    private static string GetPrefix(MediaAssetKind kind)
    {
        switch (kind)
        {
            case MediaAssetKind.Texture:
                return "tex";
            case MediaAssetKind.Sprite:
                return "sprite";
            case MediaAssetKind.Audio:
                return "audio";
            case MediaAssetKind.Subtitle:
                return "subtitle";
            default:
                return "asset";
        }
    }
}
