# Update resilience

Game updates can rename, move, recompress or rebuild media assets. UMMF therefore does not use a single hash as the permanent identity of an asset.

## Fingerprint hierarchy

Matching uses the strongest available signals:

1. localization, Addressables or game-provided stable key
2. exact content hash
3. semantic or perceptual hash
4. GameObject/component usage path
5. bundle, asset or container name
6. object name
7. texture dimensions or audio shape
8. subtitle text similarity

Exact content hashes identify unchanged assets. Stable keys and contextual metadata allow the same mod entry to survive content changes.

## Confidence policy

The runtime will eventually expose configurable thresholds, with these initial defaults:

- `0.95-1.00`: apply automatically
- `0.75-0.95`: apply and report
- `0.50-0.75`: require review
- below `0.50`: do not apply

The core matcher returns both a score and human-readable reasons so migrations can be audited.

## Version snapshots

A scanner will create a catalog for each detected game build. When the build changes, UMMF will compare the old and new catalogs instead of modifying the mod's media files. Confirmed remaps are stored as a small compatibility overlay.

```text
GameProfiles/
  base.json
  1.4.x.overlay.json
  1.5.0.overlay.json
```

## Failure behavior

A missing or ambiguous target must fail closed: the replacement is skipped and logged. UMMF must never apply a low-confidence texture, voice or subtitle to an unrelated asset simply to keep a mod active.
