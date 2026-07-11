namespace UMMF.Contracts;

public sealed class ModManifest
{
    public int SchemaVersion { get; set; } = 1;

    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Version { get; set; } = "0.1.0";

    public GameTarget Target { get; set; } = new GameTarget();

    public List<MediaReplacement> Replacements { get; set; } = new List<MediaReplacement>();
}

public sealed class GameTarget
{
    public string? GameId { get; set; }

    public string? ProductName { get; set; }

    public string? MinimumFrameworkVersion { get; set; }
}

public sealed class MediaReplacement
{
    public string Id { get; set; } = string.Empty;

    public MediaAssetKind Kind { get; set; }

    public AssetFingerprint Match { get; set; } = new AssetFingerprint();

    public string? Replacement { get; set; }

    public string? SubtitleText { get; set; }

    public string? Voice { get; set; }

    public bool StopVoiceWhenHidden { get; set; } = true;

    public int DelayMilliseconds { get; set; }

    public float Volume { get; set; } = 1.0f;
}
