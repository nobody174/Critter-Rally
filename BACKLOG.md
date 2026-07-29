# Critter Rally — Open Backlog

Open/unresolved items only — features to design, bugs to fix, ideas
needing a design pass before implementation. Once an item ships
(confirmed working in-editor, not just compiling), its write-up moves
to [CHANGELOG.md](CHANGELOG.md) — the sole historical record — and is
removed from here.

Ordering within each week matters — items depend on the ones above
them. See [ROADMAP.md](ROADMAP.md) for the full phase context and
process decisions behind these choices.

---

## Phase 1, Week 1 — Race Simulation Engine

- [x] **Design pass:** confirm base stat formulas per critter species
      (Fox/Frog to start) and level-scaling curve — Technical Designer
      hat, write-up before code. Locked 2026-07-29: per-species base +
      per-level growth rate (linear), full 5-species table in
      ROADMAP.md. Flagged as first-pass, expect tweaks after real
      race results.
- [x] `Critter` class (species, level, 5 Instincts, equipped gadgets
      list, `CalculateStats()`) — `Assets/Scripts/Critters/Critter.cs`,
      backed by `CritterSpecies` ScriptableObject definitions.
- [x] `BiomeTerrain` class (terrain type enum: Ground/Water/Tightrope/
      Burrow, length, difficulty, `GetMovementSpeed(Critter)`) —
      `Assets/Scripts/Race/BiomeTerrain.cs` + `TerrainType.cs`, plus a
      `Biome.cs` course wrapper (ordered terrain segments).
- [x] `RaceSimulator` (deterministic, seeded, frame-stepped progress
      race — bare math version, no physics/obstacles per the
      2026-07-29 sim-fidelity decision in ROADMAP.md) —
      `Assets/Scripts/Race/RaceSimulator.cs`.
- [x] Console/editor-script test harness that runs a race and prints
      the result — no graphics required —
      `Assets/Scripts/Editor/RaceSimulatorTestHarness.cs`, runnable via
      `CritterRally > Run Race Simulator Tests` menu, also runs
      headless via `-executeMethod`.
- [x] Playtest checks (verified 2026-07-29 via headless batch-mode run,
      log-confirmed, not just compiled):
  - [x] Does a higher-Sprint critter reliably win on Ground terrain?
        Yes — Fox (Sprint-specialist) won Ground 500.3 vs Frog's 224.0.
  - [x] Is the same seed always producing the same result
        (determinism)? Yes — [PASS] in test harness output.
  - [x] Do different terrain types meaningfully favor different
        critters (balance sanity check)? Yes — Frog (Swim-specialist)
        won Water 500.5 vs Fox's 179.3, flipping the Ground result.

## Phase 1, Week 2 — UI & Data Persistence

- [x] **Design pass:** finalize `PlayerData` shape — must not contain
      any `Dictionary<TKey,TValue>` field (JsonUtility limitation, see
      CLAUDE.md). `biomeProgression` becomes
      `List<BiomeProgressEntry>` where `BiomeProgressEntry` is a
      `[System.Serializable]` struct/class with `biomeId` and
      `highestDifficultyCleared`. Implemented in
      `Assets/Scripts/Save/PlayerData.cs` +
      `BiomeProgressEntry.cs`. Also caught: `DateTime` doesn't
      round-trip through `JsonUtility` either — stored as an ISO-8601
      string (`lastPlayTimeIso`) instead.
- [x] `SaveManager` (`LoadOrCreatePlayer`, `SavePlayer`, using
      `JsonUtility` + `Application.persistentDataPath`) —
      `Assets/Scripts/Save/SaveManager.cs`. Also handles re-linking
      each loaded `Critter.species` from `CritterSpeciesLookup`, since
      `[NonSerialized]` ScriptableObject references don't survive a
      JsonUtility round-trip and `CalculateStats()` would otherwise
      throw on a freshly-loaded critter.
- [x] `GameFlow` screen state machine: MainMenu → CritterSelect →
      BiomeSelect → RaceResult (placeholder UI, cubes for critters are
      fine, no 3D models yet) — `Assets/Scripts/UI/GameFlow.cs`. Screen
      enum renamed `GameScreen` after a real compile error against
      `UnityEngine.Screen` (`CS0104` ambiguous reference) — worth
      remembering for any future enum named `Screen`, `Input`,
      `Object`, etc. that shadows a UnityEngine type.
- [x] Wire starter save: new player gets Fox (Lvl 1) and Frog (Lvl 1) —
      done in `SaveManager.CreateStarterPlayer()`.
- [x] End-to-end test: select critter → pick biome → race → see
      results → save → quit → reload → critter/progress persisted
      correctly. Verified 2026-07-29 two ways, both headless and
      log-confirmed (no PlayMode Test Framework package added yet —
      kept dependency footprint minimal per the "dead simple" stack
      goal):
      - `SaveLoadTestHarness.cs`: fresh save creates linked starters;
        after simulated level-up/trophies/biome-progress + save +
        fresh `SaveManager` instance (quit/reload equivalent), all
        three persisted correctly.
      - `GameFlowPlayModeCheck.cs`: loads the real `Main.unity` scene,
        calls `GameFlow.Initialize()` (same code path as `Start()`),
        confirms it lands on `MainMenu`, loads `PlayerData` with 2
        linked starters, and `SelectCritter()` correctly advances to
        `BiomeSelect`.

## Phase 1, Week 3 — Equipment System & Progression

- [x] **Design pass:** finalize effects for Berry Shields and Vine
      Whips (currently placeholder/TBD in ROADMAP.md) and confirm the
      full 5-gadget balance sheet (each gadget's primary terrain +
      stat tradeoffs) before implementation. Locked 2026-07-29: Berry
      Shields (Tightrope: +15 Balance, -5 Sprint), Vine Whips (Burrow:
      +15 Dig, -5 Balance) — full table in ROADMAP.md. All 4 terrain
      types now have gadget coverage across the 5 launch gadgets.
- [x] `Equipment` class + `EquipmentLibrary` (Rocket Acorns, Leaf
      Gliders, Mud Skis confirmed from draft; Berry Shields/Vine Whips
      pending design pass above) —
      `Assets/Scripts/Equipment/Equipment.cs` +
      `EquipmentLibrary.cs` (factory for all 5).
- [ ] **Equipment-select UI: choose up to 2 gadgets before a race —
      still open.** `Critter.EquipGadget()`/`UnequipGadget()` enforce
      the 2-gadget max and recalculate stats correctly (verified), but
      no Canvas screen exists yet for a player to actually pick
      gadgets — that's a UI/UX Programmer hat task, deferred since it
      needs real screen-layout decisions, not just logic.
- [x] XP/level-up wiring: race result → `Critter.Experience` →
      level-up check → stat recalculation — `Critter.AddExperience()`
      (handles multi-level jumps in one call) +
      `Assets/Scripts/Race/RaceFlow.cs` (applies race result to XP/
      trophies).
- [x] **Design pass:** XP curve and trophy reward values — confirm win
      XP/trophies and loss XP/trophies match the cozy no-negative-loss
      rule (CLAUDE.md Rule 0) before wiring rewards into `RaceFlow`.
      Locked 2026-07-29: win +100 XP/+5 trophies, loss +40 XP/+0
      trophies (never negative), level threshold
      `100 + (level-1)*50` — full detail in ROADMAP.md.
- [ ] Playtest checklist — **needs a human in the Unity Editor,
      not just automated tests; still open:**
  - [ ] Can you tell which gadgets make you faster on which terrain?
  - [ ] Does winning and earning XP feel rewarding?
  - [ ] Does losing feel fair rather than punishing (no trophy loss)?
  - [ ] Does the full loop feel repetitive yet, or still fresh?

**Verified 2026-07-29 via `ProgressionTestHarness.cs`, headless and
log-confirmed:** equip-limit enforcement (2 max, 3rd rejected),
immediate stat recalculation on equip, single- and multi-level XP
jumps in one `AddExperience()` call, and `RaceFlow` win/loss rewards
matching the locked design exactly (including confirming loss never
reduces trophies).

---

*(Phase 2+ items are intentionally not itemized here yet — see the
"Maybe Later" section in ROADMAP.md. They get moved into this file
only once Phase 1 has shipped and a specific Phase 2 item is scoped.)*
