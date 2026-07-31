using System.Text;
using UnityEngine;
using UnityEngine.UI;
using CritterRally.Critters;
using CritterRally.Equipment;
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
        [SerializeField] private GameObject equipmentSelectPanel;
        [SerializeField] private GameObject biomeSelectPanel;
        [SerializeField] private GameObject raceResultPanel;

        [Header("Main Menu")]
        [SerializeField] private Button startRaceButton;

        [Header("Critter Select")]
        [SerializeField] private Transform critterButtonContainer;
        [SerializeField] private Button critterButtonPrefab;

        [Header("Equipment Select")]
        [SerializeField] private Text equipmentSelectStatsText;
        [SerializeField] private Transform gadgetButtonContainer;
        [SerializeField] private Button gadgetButtonPrefab;
        [SerializeField] private Button equipmentContinueButton;

        [Header("Biome Select")]
        [SerializeField] private Text biomeSelectInfoText;
        [SerializeField] private Button raceForestButton;

        [Header("Race Result")]
        [SerializeField] private Text raceResultText;
        [SerializeField] private Button raceAgainButton;

        private GameScreen lastRefreshedScreen = (GameScreen)(-1); // force a refresh on the first frame
        private Equipment.Equipment[] allGadgets;

        private void Start()
        {
            // Built in Start(), not a field initializer — ScriptableObject.CreateInstance
            // (used by EquipmentLibrary) isn't allowed to run during Unity's
            // MonoBehaviour construction/serialization pass.
            allGadgets = new[]
            {
                EquipmentLibrary.RocketAcorns(),
                EquipmentLibrary.LeafGliders(),
                EquipmentLibrary.MudSkis(),
                EquipmentLibrary.BerryShields(),
                EquipmentLibrary.VineWhips(),
            };

            startRaceButton.onClick.AddListener(() => gameFlow.ShowScreen(GameScreen.CritterSelect));
            equipmentContinueButton.onClick.AddListener(() => gameFlow.ShowScreen(GameScreen.BiomeSelect));
            raceForestButton.onClick.AddListener(OnRaceForestClicked);
            raceAgainButton.onClick.AddListener(() => gameFlow.ShowScreen(GameScreen.MainMenu));

            RefreshForScreen(gameFlow.CurrentScreen);
        }

        private void Update()
        {
            // Only rebuild panels/buttons when the screen actually changes —
            // rebuilding critter buttons every frame destroyed and
            // re-instantiated them before a click's Down/Up events could
            // land on the same object, silently swallowing all clicks.
            if (gameFlow.CurrentScreen != lastRefreshedScreen)
                RefreshForScreen(gameFlow.CurrentScreen);
        }

        private void RefreshForScreen(GameScreen screen)
        {
            lastRefreshedScreen = screen;

            mainMenuPanel.SetActive(screen == GameScreen.MainMenu);
            critterSelectPanel.SetActive(screen == GameScreen.CritterSelect);
            equipmentSelectPanel.SetActive(screen == GameScreen.EquipmentSelect);
            biomeSelectPanel.SetActive(screen == GameScreen.BiomeSelect);
            raceResultPanel.SetActive(screen == GameScreen.RaceResult);

            if (screen == GameScreen.CritterSelect)
                PopulateCritterButtons();

            if (screen == GameScreen.EquipmentSelect)
                PopulateGadgetButtons();

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

        private void PopulateGadgetButtons()
        {
            foreach (Transform child in gadgetButtonContainer)
                Destroy(child.gameObject);

            foreach (var gadget in allGadgets)
            {
                var button = Instantiate(gadgetButtonPrefab, gadgetButtonContainer);
                var capturedGadget = gadget; // avoid closure-over-loop-variable bug
                button.onClick.AddListener(() => OnGadgetClicked(capturedGadget));
                button.gameObject.SetActive(true);
            }

            RefreshEquipmentSelectView();
        }

        private void OnGadgetClicked(Equipment.Equipment gadget)
        {
            var critter = gameFlow.SelectedCritter;
            if (critter.equippedGadgets.Contains(gadget))
                critter.UnequipGadget(gadget);
            else
                critter.EquipGadget(gadget); // no-ops (with a log) past the 2-gadget max

            RefreshEquipmentSelectView();
        }

        /// <summary>
        /// Updates gadget button labels and the stats readout in place —
        /// deliberately does not destroy/re-instantiate the buttons here,
        /// since that previously broke click detection (see the
        /// CritterSelect bugfix note above).
        /// </summary>
        private void RefreshEquipmentSelectView()
        {
            var critter = gameFlow.SelectedCritter;

            int i = 0;
            foreach (Transform child in gadgetButtonContainer)
            {
                var gadget = allGadgets[i++];
                bool equipped = critter.equippedGadgets.Contains(gadget);
                child.GetComponentInChildren<Text>().text =
                    $"{(equipped ? "[EQUIPPED] " : "")}{gadget.equipmentName} ({gadget.primaryTerrainType})\n" +
                    FormatBonus(gadget);
            }

            equipmentSelectStatsText.text =
                $"{critter.speciesId} (Lvl {critter.level}) — {critter.equippedGadgets.Count}/{Critter.MaxEquippedGadgets} gadgets equipped\n" +
                $"Spr {critter.Sprint} Jmp {critter.Jump} Dig {critter.Dig} Swm {critter.Swim} Bal {critter.Balance}";
        }

        private static string FormatBonus(Equipment.Equipment gadget)
        {
            var sb = new StringBuilder();
            AppendBonus(sb, "Spr", gadget.bonusSprint);
            AppendBonus(sb, "Jmp", gadget.bonusJump);
            AppendBonus(sb, "Dig", gadget.bonusDig);
            AppendBonus(sb, "Swm", gadget.bonusSwim);
            AppendBonus(sb, "Bal", gadget.bonusBalance);
            return sb.ToString();
        }

        private static void AppendBonus(StringBuilder sb, string label, int value)
        {
            if (value == 0)
                return;
            if (sb.Length > 0)
                sb.Append(" ");
            sb.Append(label).Append(value > 0 ? "+" : "").Append(value);
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
