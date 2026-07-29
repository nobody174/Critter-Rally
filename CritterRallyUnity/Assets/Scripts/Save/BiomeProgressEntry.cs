using System;

namespace CritterRally.Save
{
    /// <summary>
    /// List-of-structs replacement for Dictionary&lt;int,int&gt; — JsonUtility
    /// cannot serialize Dictionary (silently produces empty data). See
    /// CLAUDE.md "JsonUtility constraint" and ROADMAP.md 2026-07-29 decision.
    /// </summary>
    [Serializable]
    public class BiomeProgressEntry
    {
        public int biomeId;
        public int highestDifficultyCleared;
    }
}
