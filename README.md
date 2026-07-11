# UMMF

Universal Media Mod Framework is an update-resilient modding framework for Unity games.

UMMF focuses on three media surfaces:

- textures and sprites
- audio clips and subtitle-triggered voice playback
- subtitles and localization text

The first runtime target will be Windows x64 Unity Mono games through BepInEx 5. Mono remains the stable starting point while BepInEx 6 and IL2CPP support are developed as separate adapters.

## Current status

The repository currently contains the runtime-independent foundation and a testable preview CLI:

- a versioned mod manifest model
- stable and generated media asset identities
- confidence-based rematching after game updates
- manifest validation
- a JSON Schema and example media mod
- a Windows x64 preview command-line application
- automated build, test and release packaging

No game files are modified in place. The preview CLI does not inject into Unity yet. Runtime adapters will discover and replace assets while the game is running in later previews.

## Download and test

Download the latest Windows archive from [GitHub Releases](https://github.com/apexlions16/UMMF/releases), extract it, and open PowerShell in the extracted directory.

```powershell
./ummf.exe info
./ummf.exe validate ./samples/ExampleMediaMod/mod.json
./ummf.exe identity-demo
./ummf.exe match-demo
```

Expected results:

- `info` prints the installed preview version.
- `validate` prints `VALID` for the bundled example manifest.
- `identity-demo` prints a deterministic subtitle asset ID.
- `match-demo` reports a compatible candidate after a simulated game asset hash change.

To test validation failure, edit a copy of `samples/ExampleMediaMod/mod.json`, remove its top-level `id`, and run `validate` against the edited file.

## Build

```bash
dotnet restore UMMF.sln
dotnet build UMMF.sln --configuration Release
dotnet test UMMF.sln --configuration Release
```

.NET 8 SDK or newer is recommended for development. Runtime-facing libraries target `netstandard2.0` for compatibility with older Unity Mono environments.

## Releases

The repository root contains a `VERSION` file. Changing it on `main` triggers the release workflow, which:

1. restores, builds and tests the solution
2. publishes a self-contained Windows x64 preview
3. packages source and SHA-256 checksums
4. creates the matching Git tag and GitHub Release

Preview versions contain a suffix such as `-preview.1` and are published as prereleases.

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
