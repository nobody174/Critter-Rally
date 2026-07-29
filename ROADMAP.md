# Critter Rally — Roadmap & Ideas

Open/forward-looking planning only. Once something is actually
released/shipped, its write-up lives in [CHANGELOG.md](CHANGELOG.md) —
the sole historical record — and is removed from here.

## Core concept (from initial pitch, 2026-07-28)

**Theme:** Cute animals with quirky abilities racing through nature.
**Core fantasy:** Train adorable critters, equip goofy gadgets, traverse
varied terrain.
**Visual/UX inspiration:** *Pocket Champs* — bright, chunky, readable
color palette, UI, and HUD style. No art exists yet in Phase 1, but
every future UI/UX pass should be checked against this reference.

**Stats ("Instincts")** — 0–100 scale, scale with critter level
- Sprint – forward speed on Ground terrain
- Jump – vertical movement / air control
- Dig – underground shortcuts (Burrow terrain)
- Swim – water traversal
- Balance – tightrope/branch running

**Gadgets ("Nature Tools")** — equip up to 2 before a race, flat stat
bonuses (some with tradeoffs), each favors a terrain type
- Rocket Acorns – Ground: +Sprint, +Jump
- Leaf Gliders – Ground: +Jump, -Sprint (glide-focused, slow on the
  ground)
- Mud Skis – Water: +Swim, -Jump
- Berry Shields – temporary protection (effect TBD — needs a Technical
  Designer pass before Week 3)
- Vine Whips – swing across obstacles (effect TBD — needs a Technical
  Designer pass before Week 3)

**Progression**
- Unlock new critters (fox, frog, mole, squirrel, hedgehog, more later)
- Explore biomes: forest, swamp, desert, tundra
- Seasonal events: "Harvest Dash," "Snowy Sprint," "Bloom Festival"

### Species stat identity (locked 2026-07-29, first pass — see note)

Each species has a base value at level 1 plus a flat per-level growth
rate, per stat — so specialization holds throughout leveling, not just
at level 1. `stat = base + level * growthPerLevel`. Values below are a
first pass to unblock Week 1 coding; **not final** — see the note at
the end of this section.

| Species   | Sprint        | Jump          | Dig           | Swim          | Balance       | Identity |
|-----------|---------------|---------------|---------------|---------------|---------------|----------|
| Fox       | 55 (+2.4/lvl) | 40 (+1.6/lvl) | 25 (+1.0/lvl) | 20 (+0.8/lvl) | 45 (+1.8/lvl) | Ground speedster — great Sprint/Balance, terrible Swim/Dig |
| Frog      | 25 (+1.0/lvl) | 50 (+2.2/lvl) | 20 (+0.8/lvl) | 55 (+2.4/lvl) | 30 (+1.2/lvl) | Water/air specialist — great Jump/Swim, weak Sprint/Dig |
| Mole      | 20 (+0.8/lvl) | 15 (+0.6/lvl) | 60 (+2.6/lvl) | 25 (+1.0/lvl) | 35 (+1.4/lvl) | Burrow specialist — dominant Dig, weak everywhere airborne |
| Squirrel  | 35 (+1.4/lvl) | 45 (+1.8/lvl) | 30 (+1.2/lvl) | 20 (+0.8/lvl) | 55 (+2.4/lvl) | Balance/agility — great Balance/Jump, weak Swim |
| Hedgehog  | 30 (+1.2/lvl) | 20 (+0.8/lvl) | 40 (+1.6/lvl) | 35 (+1.4/lvl) | 40 (+1.6/lvl) | Generalist/tank — no big weakness or strength, forgiving starter |

Roughly equal total "power budget" per species (~185 base, ~9.4/lvl
combined growth) except Hedgehog, deliberately flatter as the
forgiving starter pick alongside Fox. Ties directly into terrain
(Ground→Sprint, Water→Swim, Tightrope→Balance, Burrow→Dig) so critter
choice and biome choice are meant to interact — e.g. Mole should
dominate Burrow shortcuts but struggle everywhere else.

**Only Fox and Frog are in-scope for Week 1** (per BACKLOG.md); Mole,
Squirrel, Hedgehog numbers are here now so the full roster's identity
is decided in one pass, but they don't need implementing until their
species is actually unlocked in a later phase.

**Note — first pass, expect tweaks:** these numbers are a starting
hypothesis, not a balance guarantee. Per Rule 0/user decision
2026-07-29, we're deliberately not pre-tweaking further — run actual
races (Week 1 test harness) and rebalance from real results, not
theory.

**Design constraint:** cozy, friendly, casual-market tone — deliberate,
not incidental (see CLAUDE.md Rule 0). Losing a race never removes
trophies/progress, only slows it.

## Process/communication decisions (so they aren't re-decided later)

- **2026-07-29 — Role structure:** Claude wears three hats under one
  "Senior Gameplay Programmer/Systems Designer" identity rather than
  three separate personas: Game Systems Programmer (~60%, the
  simulation/save/progression math), Technical Game Designer (~25%,
  written specs/balance before code), UI/UX Programmer (~15%,
  Canvas/navigation/HUD structure, not visual art). See CLAUDE.md.
- **2026-07-29 — Save serializer:** Staying on Unity's built-in
  `JsonUtility` rather than adding Newtonsoft/Json.NET, to keep the
  stack dependency-free as originally scoped. Consequence: no
  `Dictionary<TKey,TValue>` in any persisted data structure, ever —
  use `List<T>` of a serializable struct instead. This overrides an
  early draft (`PlayerData.biomeProgression` as
  `Dictionary<int,int>`) which would have silently failed to save.
- **2026-07-29 — Loss rewards:** Losing a race gives reduced/zero
  trophies and small consolation XP, never negative trophies. Overrides
  an early draft's -2 trophies-on-loss rule as inconsistent with Rule 0
  (STAY COZY).
- **2026-07-29 — Sim fidelity for Phase 1:** Week 1's RaceSimulator
  ships as the bare deterministic stat-vs-terrain math race (progress
  accumulation per frame, first to cross the line wins, no physics/
  obstacles/rubber-banding). Texture (mid-race events, randomness
  beyond the seed, catch-up mechanics) is deliberately deferred until
  after the core loop is proven fun in this simplest form.
- **2026-07-29 — GitHub workflow setup:** Public repo created
  (github.com/nobody174/Critter-Rally), Unity `.gitignore` committed
  first before any real code (Library/, Temp/, Obj/, Build/, .vs/,
  *.csproj, *.sln, UserSettings/, etc. — see repo's `.gitignore`).
  **Git LFS deliberately skipped for now** — GitHub's free LFS tier is
  only 1GB storage/bandwidth per month and this is a no-budget solo
  project, so binary assets (art/audio, once Phase 2 starts) will be
  committed as regular git files instead. Audio is low-priority/minimal
  for this game already, which helps keep repo size down. Revisit LFS
  (or alternatives: itch.io asset hosting, Google Drive for source
  files, or just watching `git count-objects -vH`) only if repo size
  actually becomes a problem — don't set it up preemptively.
  **CI (GitHub Actions/GameCI headless Test Runner) deliberately
  deferred until after Phase 1 Week 1** — needs a real local Unity
  install to generate a license-activation file first, and there's no
  code to test yet. No build/release automation planned at all for
  now; releases stay fully manual (build in-editor, tag `vX.Y.Z`,
  upload to a GitHub Release or itch.io by hand) until the project is
  further along.

## Planned — Phase 1 (Weeks 1–3): Prove the core loop

See [BACKLOG.md](BACKLOG.md) for the scoped, checkable task list.
Summary of what each week delivers:

- **Week 1 — Race Simulation Engine:** `Critter`, `BiomeTerrain`,
  `RaceSimulator` as plain C# (no Unity scene needed yet). Deliverable
  is a console/editor-script harness that runs races and prints
  results — no graphics. Proves stat-vs-terrain math produces sensible,
  deterministic, terrain-differentiated outcomes.
- **Week 2 — UI & Data Persistence:** Minimal `GameFlow` screen
  state machine (MainMenu → CritterSelect → BiomeSelect → RaceResult),
  `PlayerData` + `SaveManager` using `JsonUtility`-safe structures
  (List, not Dictionary). Placeholder cubes are fine — no 3D models.
  Deliverable is a full select → race → results → save → reload loop.
- **Week 3 — Equipment System & Progression:** `Equipment` /
  `EquipmentLibrary`, gadget-equip screen (max 2 gadgets), XP/level-up
  wired into race results. Deliverable is the complete mechanical loop:
  select critter, equip gadgets, race, earn XP/trophies, level up.

## Phase 1 → Phase 2 transition

Once Phase 1 ships, the game is mechanically complete but has no real
art. Phase 2 is purely visual — the simulation underneath does not
change:
- 3D critter models (Blender and/or Sketchfab) replacing placeholder
  cubes
- Animations (run/jump/land/celebrate) synced to race results via
  Mecanim
- Biome art (forest trees/bridges, swamp mud/water, etc.)
- VFX (dust, splashes, speed trails)
- UI polish matching the Pocket Champs-inspired look: transitions, stat
  icons, equipment previews
- Visual design pipeline (Figma/Photoshop vs. AI-assisted vs. other) —
  not yet decided, needs its own discussion before Phase 2 starts

## Maybe Later — Phase 2+ backlog (post v1.0)

Held back deliberately: v1.0 must be a rock-solid single-player loop
before multiplayer/backend complexity is justified, and post-launch
content (events, new critters) is what keeps players returning once
they've finished the base game.

**High priority (very rewarding, moderate effort)**
- Multiplayer matchmaking (asynchronous races vs. leaderboard players)
- Leaderboards (global + friends)
- Seasonal events ("Harvest Dash," limited-time exclusive critters)
- Critter breeding (combine Fox + Frog → hybrid)
- 3 more biomes (Desert, Tundra, Sky)
- 3 more critters (Mole, Hedgehog, Bunny)

**Medium priority (nice-to-have, more work)**
- Procedural biome generation (infinite variations)
- Cosmetics shop (colored skins, hats, accessories)
- Challenge modes (speedrun, no-gadget, survival)
- Critter personalities (stat variation per individual, not just species)
- Photo mode (screenshot races, share replays)
- Story campaign (escape-the-research-facility narrative)

**Low priority (passion projects, save for later)**
- Mobile app (iOS/Android, if PC version succeeds)
- Mod support (community-made critters/biomes)
- Trading system (swap equipment with friends)
- Cosmetic customization (dye critters different colors)
- Difficulty modes (relaxed, hard, impossible)
- Cross-platform play
