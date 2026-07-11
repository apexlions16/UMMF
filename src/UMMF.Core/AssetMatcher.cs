using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using UMMF.Contracts;

namespace UMMF.Core;

public sealed class AssetMatchResult
{
    public AssetMatchResult(AssetFingerprint candidate, double confidence, IEnumerable<string> reasons)
    {
        Candidate = candidate ?? throw new ArgumentNullException(nameof(candidate));
        Confidence = Math.Max(0.0, Math.Min(1.0, confidence));
        Reasons = new ReadOnlyCollection<string>(reasons.ToList());
    }

    public AssetFingerprint Candidate { get; }

    public double Confidence { get; }

    public IReadOnlyList<string> Reasons { get; }
}

public static class AssetMatcher
{
    public static AssetMatchResult Compare(AssetFingerprint expected, AssetFingerprint candidate)
    {
        if (expected is null)
        {
            throw new ArgumentNullException(nameof(expected));
        }

        if (candidate is null)
        {
            throw new ArgumentNullException(nameof(candidate));
        }

        var reasons = new List<string>();
        if (expected.Kind != MediaAssetKind.Unknown &&
            candidate.Kind != MediaAssetKind.Unknown &&
            expected.Kind != candidate.Kind)
        {
            return new AssetMatchResult(candidate, 0.0, new[] { "asset-kind-mismatch" });
        }

        if (EqualNonEmpty(expected.ContentHash, candidate.ContentHash))
        {
            return new AssetMatchResult(candidate, 1.0, new[] { "exact-content-hash" });
        }

        var score = 0.0;
        AddExact(expected.StableKey, candidate.StableKey, 0.55, "stable-key", reasons, ref score, true);
        AddExact(expected.SemanticHash, candidate.SemanticHash, 0.25, "semantic-hash", reasons, ref score, false);
        AddExact(expected.UsagePath, candidate.UsagePath, 0.15, "usage-path", reasons, ref score, true);
        AddExact(expected.Container, candidate.Container, 0.08, "container", reasons, ref score, true);
        AddExact(expected.Name, candidate.Name, 0.10, "name", reasons, ref score, false);

        if (expected.Width.HasValue && expected.Height.HasValue &&
            expected.Width == candidate.Width && expected.Height == candidate.Height)
        {
            score += 0.07;
            reasons.Add("dimensions");
        }

        if (expected.SampleRate.HasValue && expected.SampleRate == candidate.SampleRate &&
            expected.Channels.HasValue && expected.Channels == candidate.Channels)
        {
            score += 0.05;
            reasons.Add("audio-shape");
        }

        if (expected.DurationSeconds.HasValue && candidate.DurationSeconds.HasValue)
        {
            var difference = Math.Abs(expected.DurationSeconds.Value - candidate.DurationSeconds.Value);
            var tolerance = Math.Max(0.1, expected.DurationSeconds.Value * 0.02);
            if (difference <= tolerance)
            {
                score += 0.05;
                reasons.Add("duration");
            }
        }

        var textSimilarity = TextSimilarity(expected.SourceText, candidate.SourceText);
        if (textSimilarity >= 0.50)
        {
            score += 0.20 * textSimilarity;
            reasons.Add("source-text-similarity");
        }

        return new AssetMatchResult(candidate, Math.Min(0.99, score), reasons);
    }

    private static void AddExact(
        string? left,
        string? right,
        double weight,
        string reason,
        ICollection<string> reasons,
        ref double score,
        bool normalizeAsPath)
    {
        var normalizedLeft = normalizeAsPath ? AssetIdentity.NormalizePath(left) : AssetIdentity.NormalizeText(left);
        var normalizedRight = normalizeAsPath ? AssetIdentity.NormalizePath(right) : AssetIdentity.NormalizeText(right);

        if (normalizedLeft.Length > 0 && string.Equals(normalizedLeft, normalizedRight, StringComparison.Ordinal))
        {
            score += weight;
            reasons.Add(reason);
        }
    }

    private static bool EqualNonEmpty(string? left, string? right)
    {
        return !string.IsNullOrWhiteSpace(left) &&
               !string.IsNullOrWhiteSpace(right) &&
               string.Equals(left.Trim(), right.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static double TextSimilarity(string? left, string? right)
    {
        var leftTokens = Tokenize(left);
        var rightTokens = Tokenize(right);
        if (leftTokens.Count == 0 || rightTokens.Count == 0)
        {
            return 0.0;
        }

        var intersection = leftTokens.Intersect(rightTokens).Count();
        var union = leftTokens.Union(rightTokens).Count();
        return union == 0 ? 0.0 : (double)intersection / union;
    }

    private static HashSet<string> Tokenize(string? value)
    {
        var normalized = AssetIdentity.NormalizeText(value);
        return new HashSet<string>(
            Regex.Split(normalized, @"[^\p{L}\p{N}]+")
                .Where(token => token.Length > 0),
            StringComparer.Ordinal);
    }
}
