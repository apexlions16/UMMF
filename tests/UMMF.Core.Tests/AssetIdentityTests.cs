using UMMF.Contracts;

namespace UMMF.Core.Tests;

public sealed class AssetIdentityTests
{
    [Fact]
    public void Stable_key_has_priority_over_runtime_metadata()
    {
        var first = new AssetFingerprint
        {
            Kind = MediaAssetKind.Subtitle,
            StableKey = "Dialogue/Main/Line_001",
            SourceText = "Original text"
        };
        var second = new AssetFingerprint
        {
            Kind = MediaAssetKind.Subtitle,
            StableKey = "dialogue\\main\\line_001",
            SourceText = "Changed by a game update"
        };

        Assert.Equal(AssetIdentity.CreateStableId(first), AssetIdentity.CreateStableId(second));
    }

    [Fact]
    public void Generated_identity_is_deterministic()
    {
        var fingerprint = new AssetFingerprint
        {
            Kind = MediaAssetKind.Texture,
            Name = "Main Menu Logo",
            Container = "ui_mainmenu",
            UsagePath = "Canvas/MainMenu/Logo",
            Width = 2048,
            Height = 1024
        };

        Assert.Equal(AssetIdentity.CreateStableId(fingerprint), AssetIdentity.CreateStableId(fingerprint));
    }
}
