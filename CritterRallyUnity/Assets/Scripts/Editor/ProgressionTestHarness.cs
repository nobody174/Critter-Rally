using UnityEditor;
using UnityEngine;
using CritterRally.Critters;
using CritterRally.Equipment;
using CritterRally.Race;
using CritterRally.Save;

namespace CritterRally.EditorTools
{
    /// <summary>
    /// Week 3 deliverable: verifies equipment equip-limit + stat effects,
    /// and XP/leveling (including multi-level jumps and the cozy no-loss-
    /// penalty reward rule), headlessly.
    /// </summary>
    public static class ProgressionTestHarness
    {
        [MenuItem("CritterRally/Run Progression Tests")]
        public static void RunTests()
        {
            Debug.Log("=== Critter Rally: Progression Test Harness ===");

            RunEquipmentTests();
            RunLevelingTests();
            RunRaceFlowTests();

            Debug.Log("=== Progression test harness complete ===");
        }

        private static void RunEquipmentTests()
        {
            var fox = MakeFoxSpecies();
            var critter = new Critter { id = 1, speciesId = "Fox", level = 1, species = fox };
            critter.CalculateStats();
            int baseSprint = critter.Sprint;

            bool firstEquip = critter.EquipGadget(EquipmentLibrary.RocketAcorns());
            bool afterFirst = critter.Sprint == baseSprint + 15;

            bool secondEquip = critter.EquipGadget(EquipmentLibrary.BerryShields());
            bool thirdEquipRejected = !critter.EquipGadget(EquipmentLibrary.MudSkis());

            Debug.Log(firstEquip && afterFirst
                ? "[PASS] Equipping Rocket Acorns applies +15 Sprint immediately."
                : "[FAIL] Rocket Acorns did not apply expected Sprint bonus.");

            Debug.Log(secondEquip && thirdEquipRejected
                ? "[PASS] Equip limit enforced: 2 gadgets allowed, 3rd rejected."
                : "[FAIL] Equip limit was not enforced correctly.");
        }

        private static void RunLevelingTests()
        {
            var fox = MakeFoxSpecies();
            var critter = new Critter { id = 1, speciesId = "Fox", level = 1, species = fox };
            critter.CalculateStats();

            // Level 1 needs 100 XP. Award exactly enough to hit level 2.
            critter.AddExperience(100);
            bool singleLevelUp = critter.level == 2 && critter.experience == 0;

            // Level 2 needs 150 XP (100 + 1*50). Award enough to jump two levels in one call.
            critter.AddExperience(150 + 200); // exactly enough for level 2->3 (150) then 3->4 needs 200
            bool multiLevelUp = critter.level == 4 && critter.experience == 0;

            Debug.Log(singleLevelUp
                ? "[PASS] AddExperience(100) at level 1 triggers exactly one level-up, XP resets to 0."
                : $"[FAIL] Expected level 2 / 0 XP, got level {critter.level} / {critter.experience} XP.");

            Debug.Log(multiLevelUp
                ? "[PASS] A large XP award correctly triggers multiple level-ups in one call."
                : $"[FAIL] Expected level 4 / 0 XP, got level {critter.level} / {critter.experience} XP.");
        }

        private static void RunRaceFlowTests()
        {
            var fox = MakeFoxSpecies();
            var frog = MakeFrogSpecies();

            var player = new Critter { id = 1, speciesId = "Fox", level = 5, species = fox };
            var opponent = new Critter { id = 2, speciesId = "Frog", level = 5, species = frog };
            player.CalculateStats();
            opponent.CalculateStats();

            var playerData = new PlayerData();
            playerData.trophies = 10;

            // Ground favors Fox's Sprint -> player should win.
            var winBiome = MakeSingleTerrainBiome(TerrainType.Ground, 500f);
            var flow = new RaceFlow();
            var winResult = flow.RunRace(player, opponent, winBiome, playerData);

            bool winRewardsCorrect = winResult.raceResult.playerWon
                                      && winResult.xpEarned == 100
                                      && winResult.trophiesEarned == 5
                                      && playerData.trophies == 15;

            Debug.Log(winRewardsCorrect
                ? "[PASS] Win rewards: +100 XP, +5 trophies, trophies applied to PlayerData."
                : "[FAIL] Win rewards did not match the locked design.");

            // Reset player level for a clean loss test; Water favors Frog -> player should lose.
            var loser = new Critter { id = 3, speciesId = "Fox", level = 5, species = fox };
            loser.CalculateStats();
            var lossBiome = MakeSingleTerrainBiome(TerrainType.Water, 500f);
            var trophiesBefore = playerData.trophies;
            var lossResult = flow.RunRace(loser, opponent, lossBiome, playerData);

            bool lossRewardsCorrect = !lossResult.raceResult.playerWon
                                       && lossResult.xpEarned == 40
                                       && lossResult.trophiesEarned == 0
                                       && playerData.trophies == trophiesBefore;

            Debug.Log(lossRewardsCorrect
                ? "[PASS] Loss rewards: +40 XP (never 0), +0 trophies, no trophy loss — cozy rule holds."
                : "[FAIL] Loss rewards violated the no-negative-trophies rule or XP design.");
        }

        private static Biome MakeSingleTerrainBiome(TerrainType type, float length)
        {
            return new Biome
            {
                biomeName = $"Test-{type}",
                randomSeed = 1,
                segments = new System.Collections.Generic.List<BiomeTerrain>
                {
                    new BiomeTerrain { type = type, length = length, difficulty = 1 }
                }
            };
        }

        private static CritterSpecies MakeFoxSpecies()
        {
            var species = ScriptableObject.CreateInstance<CritterSpecies>();
            species.speciesName = "Fox";
            species.baseSprint = 55; species.growthSprint = 2.4f;
            species.baseJump = 40; species.growthJump = 1.6f;
            species.baseDig = 25; species.growthDig = 1.0f;
            species.baseSwim = 20; species.growthSwim = 0.8f;
            species.baseBalance = 45; species.growthBalance = 1.8f;
            return species;
        }

        private static CritterSpecies MakeFrogSpecies()
        {
            var species = ScriptableObject.CreateInstance<CritterSpecies>();
            species.speciesName = "Frog";
            species.baseSprint = 25; species.growthSprint = 1.0f;
            species.baseJump = 50; species.growthJump = 2.2f;
            species.baseDig = 20; species.growthDig = 0.8f;
            species.baseSwim = 55; species.growthSwim = 2.4f;
            species.baseBalance = 30; species.growthBalance = 1.2f;
            return species;
        }
    }
}
