using System.Text;
using UnityEngine;
using UnityEngine.UI;
using CritterRally.Critters;
using CritterRally.Race;

namespace CritterRally.UI
{
    /// <summary>
    /// Bare-bones clickable UI for the Phase 1 playtest loop — no art, no
    /// layout polish, just enough Canvas UI to click through MainMenu ->
    /// CritterSelect -> BiomeSelect -> Race -> Results with a mouse instead
    /// of reading Console logs. Rebuilt/replaced entirely once real UI/UX
    /// design happens (Hat 3, Phase 2) — this is not meant to survive.
    /// </summary>
    public class DebugFlowUI : MonoBehaviour
    {
        [SerializeField] private GameFlow gameFlow;

        [Header("Panels (one per GameScreen)")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject critterSelectPanel;
        [SerializeField] private GameObject biomeSelectPanel;
        [SerializeField] private GameObject raceResultPanel;

        [Header("Main Menu")]
        [SerializeField] private Button startRaceButton;

        [Header("Critter Select")]
        [SerializeField] private Transform critterButtonContainer;
        [SerializeField] private Button critterButtonPrefab;

        [Header("Biome Select")]
        [SerializeField] private Text biomeSelectInfoText;
        [SerializeField] private Button raceForestButton;

        [Header("Race Result")]
        [SerializeField] private Text raceResultText;
        [SerializeField] private Button raceAgainButton;

        private void Start()
        {
            startRaceButton.onClick.AddListener(() => gameFlow.ShowScreen(GameScreen.CritterSelect));
            raceForestButton.onClick.AddListener(OnRaceForestClicked);
            raceAgainButton.onClick.AddListener(() => gameFlow.ShowScreen(GameScreen.MainMenu));

            RefreshForScreen(gameFlow.CurrentScreen);
        }

        private void Update()
        {
            // Simple polling refresh — fine for a debug-only UI at this scale;
            // an event-driven refresh isn't worth the complexity here.
            RefreshForScreen(gameFlow.CurrentScreen);
        }

        private void RefreshForScreen(GameScreen screen)
        {
            mainMenuPanel.SetActive(screen == GameScreen.MainMenu);
            critterSelectPanel.SetActive(screen == GameScreen.CritterSelect);
            biomeSelectPanel.SetActive(screen == GameScreen.BiomeSelect);
            raceResultPanel.SetActive(screen == GameScreen.RaceResult);

            if (screen == GameScreen.CritterSelect)
                PopulateCritterButtons();

            if (screen == GameScreen.BiomeSelect)
                UpdateBiomeSelectInfo();
        }

        private void PopulateCritterButtons()
        {
            foreach (Transform child in critterButtonContainer)
                Destroy(child.gameObject);

            foreach (var critter in gameFlow.PlayerData.critters)
            {
                var button = Instantiate(critterButtonPrefab, critterButtonContainer);
                button.GetComponentInChildren<Text>().text =
                    $"{critter.speciesId} (Lvl {critter.level})\n" +
                    $"Spr {critter.Sprint} Jmp {critter.Jump} Dig {critter.Dig} Swm {critter.Swim} Bal {critter.Balance}";

                var capturedCritter = critter; // avoid closure-over-loop-variable bug
                button.onClick.AddListener(() => gameFlow.SelectCritter(capturedCritter));
                button.gameObject.SetActive(true);
            }
        }

        private void UpdateBiomeSelectInfo()
        {
            var selected = gameFlow.SelectedCritter;
            biomeSelectInfoText.text = selected == null
                ? "No critter selected."
                : $"Racing as: {selected.speciesId} (Lvl {selected.level})";
        }

        private void OnRaceForestClicked()
        {
            var opponent = gameFlow.GenerateOpponent();
            var biome = BiomeLibrary.Forest();
            var reward = gameFlow.RunRace(opponent, biome);

            var sb = new StringBuilder();
            sb.AppendLine(reward.raceResult.playerWon ? "YOU WON!" : "You lost.");
            sb.AppendLine($"Biome: {biome.biomeName}");
            sb.AppendLine($"Your progress: {reward.raceResult.playerProgress:F1}");
            sb.AppendLine($"Opponent progress: {reward.raceResult.opponentProgress:F1}");
            sb.AppendLine($"+{reward.xpEarned} XP, +{reward.trophiesEarned} trophies");
            if (reward.levelAfter > reward.levelBefore)
                sb.AppendLine($"LEVEL UP! {reward.levelBefore} -> {reward.levelAfter}");

            raceResultText.text = sb.ToString();
        }
    }
}
