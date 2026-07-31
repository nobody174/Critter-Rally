using UnityEngine;
using CritterRally.Critters;
using CritterRally.Race;
using CritterRally.Save;

namespace CritterRally.UI
{
    public enum GameScreen
    {
        MainMenu,
        CritterSelect,
        EquipmentSelect,
        BiomeSelect,
        RaceResult
    }

    /// <summary>
    /// Minimal screen state machine driving the Phase 1 core loop:
    /// MainMenu -> CritterSelect -> BiomeSelect -> RaceResult. Placeholder
    /// UI only (Rule: no 3D models/art until Phase 2) — this owns navigation
    /// and wiring, not visual presentation.
    /// </summary>
    public class GameFlow : MonoBehaviour
    {
        [SerializeField] private CritterSpeciesLookup speciesLookup;

        public GameScreen CurrentScreen { get; private set; }
        public PlayerData PlayerData { get; private set; }
        public Critter SelectedCritter { get; private set; }

        private SaveManager saveManager;

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Split out from Start() so PlayMode tests can drive this
        /// deterministically without waiting on Unity's MonoBehaviour
        /// lifecycle timing.
        /// </summary>
        public void Initialize()
        {
            saveManager = new SaveManager();
            PlayerData = saveManager.LoadOrCreatePlayer(speciesLookup);

            Debug.Log($"[GameFlow] Save loaded. {PlayerData.critters.Count} critters, {PlayerData.trophies} trophies.");
            foreach (var critter in PlayerData.critters)
            {
                Debug.Log($"[GameFlow]   - {critter.speciesId} (Lvl {critter.level}): " +
                          $"Sprint {critter.Sprint}, Jump {critter.Jump}, Dig {critter.Dig}, " +
                          $"Swim {critter.Swim}, Balance {critter.Balance}");
            }

            ShowScreen(GameScreen.MainMenu);
        }

        public void ShowScreen(GameScreen screen)
        {
            CurrentScreen = screen;
            Debug.Log($"[GameFlow] Screen -> {screen}");
        }

        public void SelectCritter(Critter critter)
        {
            SelectedCritter = critter;
            Debug.Log($"[GameFlow] Selected critter: {critter.speciesId} (Lvl {critter.level})");
            ShowScreen(GameScreen.EquipmentSelect);
        }

        /// <summary>
        /// Builds a same-level opponent of a different species than the
        /// player's selection, so the race isn't a mirror match. Phase 1
        /// placeholder — real opponent selection (difficulty scaling,
        /// distinct opponent roster) is a later concern.
        /// </summary>
        public Critter GenerateOpponent()
        {
            var opponentSpeciesId = SelectedCritter.speciesId == "Fox" ? "Frog" : "Fox";
            var opponentSpecies = speciesLookup.GetBySpeciesId(opponentSpeciesId);

            var opponent = new Critter
            {
                id = -1,
                speciesId = opponentSpeciesId,
                level = SelectedCritter.level,
                species = opponentSpecies
            };
            opponent.CalculateStats();
            return opponent;
        }

        public RaceFlow.RaceRewardResult RunRace(Critter opponent, Biome biome)
        {
            var raceFlow = new RaceFlow();
            var reward = raceFlow.RunRace(SelectedCritter, opponent, biome, PlayerData);

            Debug.Log($"[GameFlow] Race on {biome.biomeName}: " +
                      $"{(reward.raceResult.playerWon ? "WON" : "lost")} " +
                      $"(you: {reward.raceResult.playerProgress:F1}, opponent: {reward.raceResult.opponentProgress:F1}, " +
                      $"{reward.raceResult.frameCount} frames) " +
                      $"+{reward.xpEarned} XP, +{reward.trophiesEarned} trophies" +
                      (reward.levelAfter > reward.levelBefore ? $", LEVEL UP to {reward.levelAfter}!" : ""));

            SaveGame();
            ShowScreen(GameScreen.RaceResult);
            return reward;
        }

        public void SaveGame()
        {
            saveManager.SavePlayer(PlayerData);
            Debug.Log("[GameFlow] Game saved.");
        }
    }
}
