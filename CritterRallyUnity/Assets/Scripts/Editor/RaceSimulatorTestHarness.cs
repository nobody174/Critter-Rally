using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CritterRally.Critters;
using CritterRally.Race;

namespace CritterRally.EditorTools
{
    /// <summary>
    /// Week 1 deliverable: runs races and prints results, no graphics.
    /// Verifies species identity + terrain matchups behave per the locked
    /// design (ROADMAP.md "Species stat identity", 2026-07-29).
    /// </summary>
    public static class RaceSimulatorTestHarness
    {
        [MenuItem("CritterRally/Run Race Simulator Tests")]
        public static void RunTests()
        {
            var fox = MakeSpecies("Fox", 55, 40, 25, 20, 45, 2.4f, 1.6f, 1.0f, 0.8f, 1.8f);
            var frog = MakeSpecies("Frog", 25, 50, 20, 55, 30, 1.0f, 2.2f, 0.8f, 2.4f, 1.2f);

            Debug.Log("=== Critter Rally: Race Simulator Test Harness ===");

            RunDeterminismCheck(fox, frog);
            RunTerrainMatchup("Ground (Fox should win)", TerrainType.Ground, fox, frog);
            RunTerrainMatchup("Water (Frog should win)", TerrainType.Water, fox, frog);

            Debug.Log("=== Test harness complete ===");
        }

        private static void RunDeterminismCheck(CritterSpecies fox, CritterSpecies frog)
        {
            var biome = MakeSingleTerrainBiome(TerrainType.Ground, 500f, seed: 12345);
            var result1 = RunOneRace(fox, frog, biome, level: 5);
            var result2 = RunOneRace(fox, frog, biome, level: 5);

            bool deterministic = result1.playerWon == result2.playerWon
                                  && Mathf.Approximately(result1.frameCount, result2.frameCount);

            Debug.Log(deterministic
                ? "[PASS] Determinism check: same inputs produced identical results."
                : "[FAIL] Determinism check: results differed between runs.");
        }

        private static void RunTerrainMatchup(string label, TerrainType terrain, CritterSpecies fox, CritterSpecies frog)
        {
            var biome = MakeSingleTerrainBiome(terrain, 500f, seed: 999);
            var result = RunOneRace(fox, frog, biome, level: 5);

            Debug.Log($"[{label}] Fox {(result.playerWon ? "WON" : "lost")} " +
                      $"in {result.frameCount} frames " +
                      $"(Fox progress: {result.playerProgress:F1}, Frog progress: {result.opponentProgress:F1})");
        }

        private static RaceSimulator.RaceResult RunOneRace(
            CritterSpecies fox, CritterSpecies frog, Biome biome, int level)
        {
            var player = new Critter { id = 1, speciesId = fox.speciesName, level = level, species = fox };
            var opponent = new Critter { id = 2, speciesId = frog.speciesName, level = level, species = frog };
            player.CalculateStats();
            opponent.CalculateStats();

            var simulator = new RaceSimulator();
            return simulator.SimulateRace(player, opponent, biome);
        }

        private static Biome MakeSingleTerrainBiome(TerrainType type, float length, int seed)
        {
            return new Biome
            {
                biomeName = $"Test-{type}",
                randomSeed = seed,
                segments = new List<BiomeTerrain>
                {
                    new BiomeTerrain { type = type, length = length, difficulty = 1 }
                }
            };
        }

        private static CritterSpecies MakeSpecies(
            string name,
            float baseSprint, float baseJump, float baseDig, float baseSwim, float baseBalance,
            float growthSprint, float growthJump, float growthDig, float growthSwim, float growthBalance)
        {
            var species = ScriptableObject.CreateInstance<CritterSpecies>();
            species.speciesName = name;
            species.baseSprint = baseSprint;
            species.baseJump = baseJump;
            species.baseDig = baseDig;
            species.baseSwim = baseSwim;
            species.baseBalance = baseBalance;
            species.growthSprint = growthSprint;
            species.growthJump = growthJump;
            species.growthDig = growthDig;
            species.growthSwim = growthSwim;
            species.growthBalance = growthBalance;
            return species;
        }
    }
}
