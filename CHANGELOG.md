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

- Locked full 5-gadget balance sheet (Rocket Acorns, Leaf Gliders, Mud
  Skis, Berry Shields, Vine Whips) covering all 4 terrain types, and
  the win/loss XP + trophy reward curve — see ROADMAP.md.
- Phase 1 Week 3 (Equipment System & Progression) mostly complete:
  `Equipment` + `EquipmentLibrary` (all 5 gadgets),
  `Critter.EquipGadget()`/`UnequipGadget()` (enforces 2-gadget max,
  recalculates stats immediately), `Critter.AddExperience()` (handles
  multi-level jumps in one call), `Race/RaceFlow.cs` (applies race
  result to XP/trophies per the locked cozy no-negative-trophy rule).
  **Not yet done:** the actual gadget-select Canvas UI (equip logic
  exists, no screen to drive it yet) and a human playtest pass in the
  Editor — both intentionally left open in BACKLOG.md rather than
  claimed as finished.
- Verified in-editor (not just compiled), 2026-07-29:
  `ProgressionTestHarness` confirms equip-limit enforcement, immediate
  stat recalculation on equip/unequip, single- and multi-level XP
  jumps, and `RaceFlow` win/loss rewards matching the locked design —
  including that a loss never reduces trophies.

### Added — 2026-07-29 (Week 3.5, human-playable debug loop)
- `BiomeLibrary.Forest()` — first real multi-segment biome (Ground →
  Burrow → Tightrope → Ground), so a race screen has something
  concrete to run instead of only inline test-harness biomes.
- `GameFlow.GenerateOpponent()` — placeholder same-level,
  opposite-species opponent.
- **Bugfix:** `GameFlow.RunRace()` was calling `RaceSimulator` directly
  instead of `RaceFlow`, so Week 3's reward wiring never actually
  applied during normal play — only in the isolated test harness. Now
  correctly routes through `RaceFlow` and saves after every race.
- `DebugFlowUI` — bare Canvas UI (buttons/text, zero art/layout
  polish, explicitly disposable once real UI/UX design happens):
  MainMenu → CritterSelect → BiomeSelect → RaceResult, fully clickable
  in Play mode.
- Added `Debug.Log` calls throughout `GameFlow` so Play mode is
  observable via the Console even before any UI existed.
- **Real bug caught and fixed:** `com.unity.ugui` was missing from
  `Packages/manifest.json` — this Unity 6 project template ships
  UI Toolkit modules by default but not the legacy UGUI package that
  `Button`/`Text` require. Added `"com.unity.ugui": "2.0.0"`.
- Regression-verified: all Week 1/2/3 test harnesses still pass after
  the package change (run sequentially — two parallel Unity
  batch-mode instances against one project conflict on the project
  lock and fail immediately).

### Changed — 2026-07-29 (render pipeline)
- Switched from the deprecated Built-in Render Pipeline to URP
  (Universal Render Pipeline, `com.unity.render-pipelines.universal`),
  done now specifically because Phase 1 has zero rendering yet (no
  models/materials) — the cheapest possible point to convert, before
  any Phase 2 art work exists that would need re-conversion. New URP
  asset + renderer at `Assets/Settings/`, assigned as the default
  pipeline and across all 6 quality levels via `URPSetup.cs`
  (`CritterRally > Setup URP (one-time)`).
- Regression-verified: all Week 1/2/3 test harnesses still pass after
  the pipeline switch (logic-only tests, unaffected by rendering, but
  confirmed anyway since the package/settings churn touched
  `ProjectSettings`).

*(No tagged releases yet. v0.1.0 is intentionally NOT tagged — the
gadget-select UI and a human playtest pass in the Editor are still
open per BACKLOG.md, and "playable end-to-end" means a person can
actually click through it and confirm it feels right, not just that
the underlying systems pass headless tests.)*
