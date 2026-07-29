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

- [ ] **Design pass:** confirm base stat formulas per critter species
      (Fox/Frog to start) and level-scaling curve — Technical Designer
      hat, write-up before code.
- [ ] `Critter` class (species, level, 5 Instincts, equipped gadgets
      list, `CalculateStats()`)
- [ ] `BiomeTerrain` class (terrain type enum: Ground/Water/Tightrope/
      Burrow, length, difficulty, `GetMovementSpeed(Critter)`)
- [ ] `RaceSimulator` (deterministic, seeded, frame-stepped progress
      race — bare math version, no physics/obstacles per the
      2026-07-29 sim-fidelity decision in ROADMAP.md)
- [ ] Console/editor-script test harness that runs a race and prints
      the result — no graphics required
- [ ] Playtest checks:
  - [ ] Does a higher-Sprint critter reliably win on Ground terrain?
  - [ ] Is the same seed always producing the same result
        (determinism)?
  - [ ] Do different terrain types meaningfully favor different
        critters (balance sanity check)?

## Phase 1, Week 2 — UI & Data Persistence

- [ ] **Design pass:** finalize `PlayerData` shape — must not contain
      any `Dictionary<TKey,TValue>` field (JsonUtility limitation, see
      CLAUDE.md). `biomeProgression` becomes
      `List<BiomeProgressEntry>` where `BiomeProgressEntry` is a
      `[System.Serializable]` struct/class with `biomeId` and
      `highestDifficultyCleared`.
- [ ] `SaveManager` (`LoadOrCreatePlayer`, `SavePlayer`, using
      `JsonUtility` + `Application.persistentDataPath`)
- [ ] `GameFlow` screen state machine: MainMenu → CritterSelect →
      BiomeSelect → RaceResult (placeholder UI, cubes for critters are
      fine, no 3D models yet)
- [ ] Wire starter save: new player gets Fox (Lvl 1) and Frog (Lvl 1)
- [ ] End-to-end test: select critter → pick biome → race → see
      results → save → quit → reload → critter/progress persisted
      correctly

## Phase 1, Week 3 — Equipment System & Progression

- [ ] **Design pass:** finalize effects for Berry Shields and Vine
      Whips (currently placeholder/TBD in ROADMAP.md) and confirm the
      full 5-gadget balance sheet (each gadget's primary terrain +
      stat tradeoffs) before implementation.
- [ ] `Equipment` class + `EquipmentLibrary` (Rocket Acorns, Leaf
      Gliders, Mud Skis confirmed from draft; Berry Shields/Vine Whips
      pending design pass above)
- [ ] Equipment-select UI: choose up to 2 gadgets before a race
- [ ] XP/level-up wiring: race result → `Critter.Experience` →
      level-up check → stat recalculation
- [ ] **Design pass:** XP curve and trophy reward values — confirm win
      XP/trophies and loss XP/trophies match the cozy no-negative-loss
      rule (CLAUDE.md Rule 0) before wiring rewards into `RaceFlow`
- [ ] Playtest checklist:
  - [ ] Can you tell which gadgets make you faster on which terrain?
  - [ ] Does winning and earning XP feel rewarding?
  - [ ] Does losing feel fair rather than punishing (no trophy loss)?
  - [ ] Does the full loop feel repetitive yet, or still fresh?

---

*(Phase 2+ items are intentionally not itemized here yet — see the
"Maybe Later" section in ROADMAP.md. They get moved into this file
only once Phase 1 has shipped and a specific Phase 2 item is scoped.)*
