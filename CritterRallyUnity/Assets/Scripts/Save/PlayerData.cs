using System;
using System.Collections.Generic;
using CritterRally.Critters;

namespace CritterRally.Save
{
    /// <summary>
    /// Root save-file shape. JsonUtility-safe: no Dictionary fields (see
    /// BiomeProgressEntry), no raw DateTime (stored as ISO-8601 string).
    /// Every field here needs a safe default so an older save doesn't throw
    /// on load after a future update (CLAUDE.md Rule 4).
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public List<Critter> critters = new();
        public int trophies;
        public string lastPlayTimeIso = string.Empty;
        public List<BiomeProgressEntry> biomeProgression = new();
    }
}
