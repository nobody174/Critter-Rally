# Changelog

All notable changes to Critter Rally are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/).
Version numbers follow [Semantic Versioning](https://semver.org/): MAJOR.MINOR.PATCH

## How to version commits

- `fix:` → bug fix → bumps PATCH (1.0.0 → 1.0.1)
- `feat:` → new feature → bumps MINOR (1.0.0 → 1.1.0)
- `BREAKING CHANGE:` → save-format break or major rework → bumps MAJOR (1.0.0 → 2.0.0)
- `chore:` → maintenance, refactoring, no gameplay change
- `docs:` → README or documentation only

When you bump the version, update it in `ProjectSettings` (or wherever
the project's version constant lives once that's established) and add
a dated entry below.

---

## [Unreleased]

### Added — 2026-07-29
- Unity 6 (6000.5.5f1) project created at `CritterRallyUnity/`, modular
  folder structure (`Scripts/Critters`, `Scripts/Race`,
  `Scripts/Equipment`, `Scripts/Save`, `Scripts/UI`,
  `Scripts/Editor`, `ScriptableObjects/`).
- Phase 1 Week 1 (Race Simulation Engine) complete: `Critter`,
  `CritterSpecies` (ScriptableObject), `TerrainType`, `BiomeTerrain`,
  `Biome`, `RaceSimulator`. Deterministic, seeded, stat-vs-terrain math
  race — no physics/obstacles by design (see ROADMAP.md sim-fidelity
  decision).
- `RaceSimulatorTestHarness` editor tool
  (`CritterRally > Run Race Simulator Tests`), confirmed working
  headless via Unity batch mode:
  - Determinism check: PASS.
  - Ground terrain: Fox (Sprint-specialist) beat Frog, 500.3 vs 224.0
    progress.
  - Water terrain: Frog (Swim-specialist) beat Fox, 500.5 vs 179.3
    progress — confirms terrain/species matchups behave as designed.
- Locked full 5-species stat identity table (Fox, Frog, Mole,
  Squirrel, Hedgehog) — see ROADMAP.md, flagged as first-pass numbers
  pending real playtest rebalancing.
- Phase 1 Week 2 (UI & Data Persistence) complete: `PlayerData`,
  `BiomeProgressEntry` (List-based, JsonUtility-safe), `SaveManager`
  (load/save + species re-linking after deserialization),
  `CritterSpeciesLookup` registry, `GameFlow` screen state machine
  (`GameScreen` enum — renamed from `Screen` after a real
  `UnityEngine.Screen` naming collision), and a placeholder `Main.unity`
  scene wiring it all together with Fox/Frog `CritterSpecies` assets.
- Verified in-editor (not just compiled), 2026-07-29: `SaveLoadTestHarness`
  confirms fresh-save creation and a full save → new-instance-reload
  round-trip preserves level, trophies, and biome progress;
  `GameFlowPlayModeCheck` loads the real scene and confirms
  `Initialize()` reaches `MainMenu` with linked starter critters, and
  `SelectCritter()` correctly advances to `BiomeSelect`.

*(No tagged releases yet — v0.1.0 is planned once the full Phase 1
loop, Weeks 1-3, is playable end-to-end.)*
