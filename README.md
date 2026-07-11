# UMMF

Universal Media Mod Framework is an update-resilient modding framework for Unity games.

UMMF focuses on three media surfaces:

- textures and sprites
- audio clips and subtitle-triggered voice playback
- subtitles and localization text

The first runtime target will be Windows x64 Unity Mono games through BepInEx 5. Mono remains the stable starting point while BepInEx 6 and IL2CPP support are developed as separate adapters.

## Current status

The repository currently contains the runtime-independent foundation:

- a versioned mod manifest model
- stable and generated media asset identities
- confidence-based rematching after game updates
- manifest validation
- a JSON Schema and example media mod
- automated build and unit tests

No game files are modified in place. Runtime adapters will discover and replace assets while the game is running.

## Build

```bash
dotnet restore UMMF.sln
dotnet build UMMF.sln --configuration Release
dotnet test UMMF.sln --configuration Release
```

.NET 8 SDK or newer is recommended for development. Runtime-facing libraries target `netstandard2.0` for compatibility with older Unity Mono environments.

## Roadmap

1. Complete manifest serialization and migration support.
2. Add a Unity/BepInEx Mono host.
3. Discover and replace `Texture2D`, `Sprite` and UI textures.
4. Capture TextMeshPro and Unity UI subtitle sources.
5. Attach mod-provided voice clips to subtitle IDs.
6. Add runtime catalogs and game-update migration reports.
7. Add IL2CPP, Addressables, FMOD and Wwise adapters.

## Scope and safety

UMMF is intended for lawful, offline and single-player modding. Anti-cheat bypasses, multiplayer manipulation and redistribution of copyrighted game assets are outside the project scope.

## License

MIT
