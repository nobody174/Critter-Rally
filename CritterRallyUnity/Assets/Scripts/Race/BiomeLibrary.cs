using System.Collections.Generic;

namespace CritterRally.Race
{
    /// <summary>
    /// Hardcoded starter biome(s) for the Phase 1 playtest loop. Real biome
    /// authoring (ScriptableObject-based, multiple biomes/difficulties) is a
    /// Phase 2+ concern per ROADMAP.md — this exists only so BiomeSelect has
    /// something real to race, not another inline test-only construction.
    /// </summary>
    public static class BiomeLibrary
    {
        public static Biome Forest()
        {
            return new Biome
            {
                biomeName = "Forest",
                randomSeed = 1,
                segments = new List<BiomeTerrain>
                {
                    new BiomeTerrain { type = TerrainType.Ground, length = 200f, difficulty = 1 },
                    new BiomeTerrain { type = TerrainType.Burrow, length = 100f, difficulty = 1 },
                    new BiomeTerrain { type = TerrainType.Tightrope, length = 100f, difficulty = 1 },
                    new BiomeTerrain { type = TerrainType.Ground, length = 150f, difficulty = 1 },
                }
            };
        }
    }
}
