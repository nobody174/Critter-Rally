using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CritterRally.UI;

namespace CritterRally.EditorTools
{
    /// <summary>
    /// Loads Main.unity, calls GameFlow.Initialize() directly (equivalent to
    /// what Start() does in Play Mode), and asserts the resulting state.
    /// EditorApplication.isPlaying is not reliable to await synchronously in
    /// -batchmode, so this exercises the same code path GameFlow.Start()
    /// calls rather than entering actual Play Mode — still real behavior,
    /// not just a compile check.
    /// </summary>
    public static class GameFlowPlayModeCheck
    {
        [MenuItem("CritterRally/Verify GameFlow In Main Scene")]
        public static void Verify()
        {
            Debug.Log("=== Critter Rally: GameFlow Verification ===");

            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");

            var flowObject = GameObject.Find("GameFlow");
            if (flowObject == null)
            {
                Debug.LogError("[FAIL] No GameFlow object found in Main.unity.");
                return;
            }

            var gameFlow = flowObject.GetComponent<GameFlow>();
            gameFlow.Initialize();

            bool screenCorrect = gameFlow.CurrentScreen == GameScreen.MainMenu;
            bool playerDataLoaded = gameFlow.PlayerData != null && gameFlow.PlayerData.critters.Count == 2;
            bool speciesLinked = playerDataLoaded && gameFlow.PlayerData.critters[0].species != null;

            Debug.Log(screenCorrect
                ? "[PASS] GameFlow.Initialize() lands on MainMenu screen."
                : $"[FAIL] Expected MainMenu, got {gameFlow.CurrentScreen}.");

            Debug.Log(playerDataLoaded
                ? "[PASS] PlayerData loaded with 2 starter critters."
                : "[FAIL] PlayerData did not load as expected.");

            Debug.Log(speciesLinked
                ? "[PASS] Critter species correctly re-linked from CritterSpeciesLookup."
                : "[FAIL] Critter species was not linked (would throw on CalculateStats()).");

            var fox = gameFlow.PlayerData.critters[0];
            gameFlow.SelectCritter(fox);
            bool selectWorks = gameFlow.SelectedCritter == fox && gameFlow.CurrentScreen == GameScreen.BiomeSelect;

            Debug.Log(selectWorks
                ? "[PASS] SelectCritter() sets SelectedCritter and advances to BiomeSelect."
                : "[FAIL] SelectCritter() did not behave as expected.");

            Debug.Log("=== GameFlow verification complete ===");
        }
    }
}
