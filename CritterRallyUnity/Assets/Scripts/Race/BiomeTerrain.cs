using System;
using CritterRally.Critters;

namespace CritterRally.Race
{
    /// <summary>
    /// One terrain segment within a biome. Movement speed on a segment is
    /// driven entirely by the matching Instinct stat.
    /// </summary>
    [Serializable]
    public class BiomeTerrain
    {
        public TerrainType type;
        public float length;
        public int difficulty; // 1-5, reserved for opponent-strength scaling

        public float GetMovementSpeed(Critter critter)
        {
            return type switch
            {
                TerrainType.Ground => critter.Sprint * 1.0f,
                TerrainType.Water => critter.Swim * 0.9f,
                TerrainType.Tightrope => critter.Balance * 1.2f,
                TerrainType.Burrow => critter.Dig * 1.1f,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
