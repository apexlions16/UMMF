using UMMF.Contracts;

namespace UMMF.Core.Tests;

public sealed class AssetMatcherTests
{
    [Fact]
    public void Exact_content_hash_is_a_certain_match()
    {
        var expected = new AssetFingerprint
        {
            Kind = MediaAssetKind.Audio,
            ContentHash = "ABC123"
        };
        var candidate = new AssetFingerprint
        {
            Kind = MediaAssetKind.Audio,
            ContentHash = "abc123"
        };

        var result = AssetMatcher.Compare(expected, candidate);

        Assert.Equal(1.0, result.Confidence);
        Assert.Contains("exact-content-hash", result.Reasons);
    }

    [Fact]
    public void Stable_metadata_survives_changed_content()
    {
        var expected = new AssetFingerprint
        {
            Kind = MediaAssetKind.Texture,
            StableKey = "ui/main-menu/logo",
            Name = "MainMenuLogo",
            UsagePath = "Canvas/MainMenu/Logo",
            Width = 2048,
            Height = 1024,
            ContentHash = "old"
        };
        var candidate = new AssetFingerprint
        {
            Kind = MediaAssetKind.Texture,
            StableKey = "UI/Main-Menu/Logo",
            Name = "MainMenuLogo",
            UsagePath = "canvas/mainmenu/logo",
            Width = 2048,
            Height = 1024,
            ContentHash = "new"
        };

        var result = AssetMatcher.Compare(expected, candidate);

        Assert.True(result.Confidence >= 0.85, $"Confidence was {result.Confidence}");
    }

    [Fact]
    public void Different_media_kinds_never_match()
    {
        var expected = new AssetFingerprint { Kind = MediaAssetKind.Audio, Name = "intro" };
        var candidate = new AssetFingerprint { Kind = MediaAssetKind.Texture, Name = "intro" };

        var result = AssetMatcher.Compare(expected, candidate);

        Assert.Equal(0.0, result.Confidence);
    }

    [Fact]
    public void Similar_subtitle_text_contributes_to_score()
    {
        var expected = new AssetFingerprint
        {
            Kind = MediaAssetKind.Subtitle,
            UsagePath = "UI/Dialogue/Subtitle",
            SourceText = "We need to leave this place now"
        };
        var candidate = new AssetFingerprint
        {
            Kind = MediaAssetKind.Subtitle,
            UsagePath = "UI/Dialogue/Subtitle",
            SourceText = "We need to leave now"
        };

        var result = AssetMatcher.Compare(expected, candidate);

        Assert.True(result.Confidence > 0.25, $"Confidence was {result.Confidence}");
        Assert.Contains("source-text-similarity", result.Reasons);
    }
}
