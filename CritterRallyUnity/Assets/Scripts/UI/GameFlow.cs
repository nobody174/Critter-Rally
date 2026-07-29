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
            ShowScreen(GameScreen.MainMenu);
        }

        public void ShowScreen(GameScreen screen)
        {
            CurrentScreen = screen;
        }

        public void SelectCritter(Critter critter)
        {
            SelectedCritter = critter;
            ShowScreen(GameScreen.BiomeSelect);
        }

        public RaceSimulator.RaceResult RunRace(Critter opponent, Biome biome)
        {
            var simulator = new RaceSimulator();
            var result = simulator.SimulateRace(SelectedCritter, opponent, biome);
            ShowScreen(GameScreen.RaceResult);
            return result;
        }

        public void SaveGame()
        {
            saveManager.SavePlayer(PlayerData);
        }
    }
}
