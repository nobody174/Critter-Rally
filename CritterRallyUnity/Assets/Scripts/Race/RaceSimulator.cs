using CritterRally.Critters;

namespace CritterRally.Race
{
    /// <summary>
    /// Deterministic, seeded stat-vs-terrain race simulation. Bare math only —
    /// no physics/obstacles/rubber-banding (ROADMAP.md "Sim fidelity for
    /// Phase 1" decision, 2026-07-29). Both critters must have CalculateStats()
    /// already called before simulating.
    /// </summary>
    public class RaceSimulator
    {
        private const float SecondsPerFrame = 1f / 60f;
        private const int MaxFrames = 7200; // 2 minutes at 60 FPS

        public class RaceResult
        {
            public bool playerWon;
            public int frameCount;
            public float playerProgress;
            public float opponentProgress;

            // Reward calculation lives in RaceFlow (Week 3), not here —
            // RaceSimulator only reports what happened in the race itself.
        }

        public RaceResult SimulateRace(Critter player, Critter opponent, Biome biome)
        {
            float playerProgress = 0f;
            float opponentProgress = 0f;
            int frame = 0;

            while (frame < MaxFrames)
            {
                var playerTerrain = biome.GetTerrainAtProgress(playerProgress);
                var opponentTerrain = biome.GetTerrainAtProgress(opponentProgress);

                playerProgress += playerTerrain.GetMovementSpeed(player) * SecondsPerFrame;
                opponentProgress += opponentTerrain.GetMovementSpeed(opponent) * SecondsPerFrame;

                frame++;

                if (playerProgress >= biome.TotalLength || opponentProgress >= biome.TotalLength)
                    break;
            }

            return new RaceResult
            {
                playerWon = playerProgress >= opponentProgress,
                frameCount = frame,
                playerProgress = playerProgress,
                opponentProgress = opponentProgress
            };
        }
    }
}
