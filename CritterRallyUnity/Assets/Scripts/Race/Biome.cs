using System;
using System.Collections.Generic;

namespace CritterRally.Race
{
    /// <summary>
    /// An ordered sequence of terrain segments forming one race course.
    /// </summary>
    [Serializable]
    public class Biome
    {
        public string biomeName;
        public int randomSeed;
        public List<BiomeTerrain> segments = new();

        public float TotalLength
        {
            get
            {
                float total = 0f;
                foreach (var segment in segments)
                    total += segment.length;
                return total;
            }
        }

        /// <summary>
        /// Returns the terrain segment covering the given distance-into-course.
        /// Clamps to the final segment once progress exceeds the course length,
        /// so a critter that finishes doesn't throw on a trailing lookup.
        /// </summary>
        public BiomeTerrain GetTerrainAtProgress(float progress)
        {
            float accumulated = 0f;
            foreach (var segment in segments)
            {
                accumulated += segment.length;
                if (progress < accumulated)
                    return segment;
            }
            return segments.Count > 0 ? segments[^1] : null;
        }
    }
}
