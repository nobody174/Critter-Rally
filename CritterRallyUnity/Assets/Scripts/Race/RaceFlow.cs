using CritterRally.Critters;
using CritterRally.Save;

namespace CritterRally.Race
{
    /// <summary>
    /// Applies race rewards to the player's critter and save data. Cozy
    /// rule (CLAUDE.md Rule 0): losing never removes trophies/progress,
    /// only slows it — see ROADMAP.md "XP & trophy rewards" (2026-07-29).
    /// </summary>
    public class RaceFlow
    {
        private const int WinXp = 100;
        private const int WinTrophies = 5;
        private const int LossXp = 40;
        private const int LossTrophies = 0;

        public class RaceRewardResult
        {
            public RaceSimulator.RaceResult raceResult;
            public int xpEarned;
            public int trophiesEarned;
            public int levelBefore;
            public int levelAfter;
        }

        public RaceRewardResult RunRace(Critter player, Critter opponent, Biome biome, PlayerData playerData)
        {
            var simulator = new RaceSimulator();
            var raceResult = simulator.SimulateRace(player, opponent, biome);

            int xp = raceResult.playerWon ? WinXp : LossXp;
            int trophies = raceResult.playerWon ? WinTrophies : LossTrophies;
            int levelBefore = player.level;

            player.AddExperience(xp);
            playerData.trophies += trophies;

            return new RaceRewardResult
            {
                raceResult = raceResult,
                xpEarned = xp,
                trophiesEarned = trophies,
                levelBefore = levelBefore,
                levelAfter = player.level
            };
        }
    }
}
