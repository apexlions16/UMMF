# Architecture

UMMF separates game-specific runtime access from the stable media-mod model.

## Layers

### UMMF.Contracts

Contains dependency-free data contracts shared by scanners, mod tools and runtime adapters:

- media asset kinds
- update-resilient fingerprints
- mod manifests
- replacement instructions

The assembly targets `netstandard2.0` so it can be consumed by older Unity Mono runtimes.

### UMMF.Core

Contains runtime-independent behavior:

- deterministic asset identity generation
- candidate scoring and rematching after game updates
- manifest validation

It must not reference Unity, BepInEx, Harmony, FMOD or Wwise.

### Runtime adapters (next phase)

Planned adapters discover and replace game objects while translating them to the shared contracts:

- `UMMF.BepInEx.Mono`
- `UMMF.BepInEx.IL2CPP`
- `UMMF.Backends.UnityAudio`
- `UMMF.Backends.FMOD`
- `UMMF.Backends.Wwise`
- `UMMF.Textures.Unity`
- `UMMF.Subtitles.TextMeshPro`
- `UMMF.Subtitles.UnityUI`

## Runtime flow

1. Detect the game, Unity version and scripting backend.
2. Load installed mod manifests.
3. Discover active textures, audio clips and subtitle sources.
4. Convert discoveries into `AssetFingerprint` values.
5. Match replacements with confidence scores.
6. Apply only matches above the configured safety threshold.
7. Record unresolved or ambiguous matches for review.

## Safety boundaries

UMMF will not provide anti-cheat bypasses, multiplayer manipulation or executable patch distribution. The first supported environment is offline/single-player Unity Mono games on Windows x64.
