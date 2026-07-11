namespace UMMF.Contracts;

public sealed class AssetFingerprint
{
    public MediaAssetKind Kind { get; set; }

    public string? StableKey { get; set; }

    public string? Name { get; set; }

    public string? Container { get; set; }

    public string? UsagePath { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public double? DurationSeconds { get; set; }

    public int? SampleRate { get; set; }

    public int? Channels { get; set; }

    public string? ContentHash { get; set; }

    public string? SemanticHash { get; set; }

    public string? SourceText { get; set; }
}
