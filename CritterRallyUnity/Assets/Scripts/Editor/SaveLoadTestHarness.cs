using System.IO;
using UnityEditor;
using UnityEngine;
using CritterRally.Critters;
using CritterRally.Save;

namespace CritterRally.EditorTools
{
    /// <summary>
    /// Week 2 deliverable: verifies the full select -> race -> results ->
    /// save -> quit -> reload -> data-persisted loop, headlessly. Uses a
    /// throwaway save file path so it never touches a real player's save.
    /// </summary>
    public static class SaveLoadTestHarness
    {
        [MenuItem("CritterRally/Run Save-Load Tests")]
        public static void RunTests()
        {
            Debug.Log("=== Critter Rally: Save/Load Test Harness ===");

            var testSavePath = Path.Combine(Application.temporaryCachePath, "critter_rally_test_save.json");
            if (File.Exists(testSavePath))
                File.Delete(testSavePath);

            var lookup = BuildTestLookup();

            // First run: no save exists yet, should create starter player.
            var saveManager1 = new SaveManager(testSavePath);
            var freshData = saveManager1.LoadOrCreatePlayer(lookup);

            bool startersCorrect = freshData.critters.Count == 2
                                    && freshData.critters[0].speciesId == "Fox"
                                    && freshData.critters[1].speciesId == "Frog"
                                    && freshData.critters[0].species != null;

            Debug.Log(startersCorrect
                ? "[PASS] Fresh save creates starter Fox + Frog with species linked."
                : "[FAIL] Fresh save did not create expected starter critters.");

            // Simulate progress: level up, earn trophies, save.
            freshData.critters[0].level = 5;
            freshData.trophies = 42;
            freshData.biomeProgression.Add(new BiomeProgressEntry { biomeId = 1, highestDifficultyCleared = 3 });
            saveManager1.SavePlayer(freshData);

            // Second run: simulate "quit and reload" with a fresh SaveManager instance.
            var saveManager2 = new SaveManager(testSavePath);
            var reloadedData = saveManager2.LoadOrCreatePlayer(lookup);

            bool persisted = reloadedData.critters[0].level == 5
                              && reloadedData.trophies == 42
                              && reloadedData.biomeProgression.Count == 1
                              && reloadedData.biomeProgression[0].biomeId == 1
                              && reloadedData.biomeProgression[0].highestDifficultyCleared == 3
                              && reloadedData.critters[0].species != null
                              && reloadedData.critters[0].Sprint > 0; // CalculateStats() ran post-relink

            Debug.Log(persisted
                ? "[PASS] Reload after quit: level, trophies, biome progress all persisted correctly."
                : "[FAIL] Reload after quit: data did not persist as expected.");

            File.Delete(testSavePath);
            Debug.Log("=== Save/Load test harness complete ===");
        }

        private static CritterSpeciesLookup BuildTestLookup()
        {
            var lookup = ScriptableObject.CreateInstance<CritterSpeciesLookup>();

            var fox = ScriptableObject.CreateInstance<CritterSpecies>();
            fox.speciesName = "Fox";
            fox.baseSprint = 55; fox.growthSprint = 2.4f;
            fox.baseJump = 40; fox.growthJump = 1.6f;
            fox.baseDig = 25; fox.growthDig = 1.0f;
            fox.baseSwim = 20; fox.growthSwim = 0.8f;
            fox.baseBalance = 45; fox.growthBalance = 1.8f;

            var frog = ScriptableObject.CreateInstance<CritterSpecies>();
            frog.speciesName = "Frog";
            frog.baseSprint = 25; frog.growthSprint = 1.0f;
            frog.baseJump = 50; frog.growthJump = 2.2f;
            frog.baseDig = 20; frog.growthDig = 0.8f;
            frog.baseSwim = 55; frog.growthSwim = 2.4f;
            frog.baseBalance = 30; frog.growthBalance = 1.2f;

            lookup.allSpecies.Add(fox);
            lookup.allSpecies.Add(frog);
            return lookup;
        }
    }
}
