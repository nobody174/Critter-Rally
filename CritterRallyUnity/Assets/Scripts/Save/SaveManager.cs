using System;
using System.IO;
using UnityEngine;
using CritterRally.Critters;

namespace CritterRally.Save
{
    /// <summary>
    /// Loads/saves PlayerData as JSON via JsonUtility. Critter.species is
    /// [NonSerialized] (ScriptableObject references don't round-trip through
    /// JsonUtility), so LoadOrCreatePlayer re-links each Critter's species
    /// from speciesId using the provided lookup after deserializing — a
    /// freshly-loaded Critter is unusable (CalculateStats() throws) until
    /// this relink happens.
    /// </summary>
    public class SaveManager
    {
        private readonly string savePath;

        public SaveManager(string savePathOverride = null)
        {
            savePath = savePathOverride ?? Path.Combine(Application.persistentDataPath, "game_data.json");
        }

        public PlayerData LoadOrCreatePlayer(CritterSpeciesLookup speciesLookup)
        {
            PlayerData data;

            if (File.Exists(savePath))
            {
                var json = File.ReadAllText(savePath);
                data = JsonUtility.FromJson<PlayerData>(json);
            }
            else
            {
                data = CreateStarterPlayer();
                SavePlayer(data);
            }

            RelinkSpecies(data, speciesLookup);
            return data;
        }

        public void SavePlayer(PlayerData data)
        {
            data.lastPlayTimeIso = DateTime.UtcNow.ToString("o");
            var json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(savePath, json);
        }

        private static PlayerData CreateStarterPlayer()
        {
            var data = new PlayerData();
            data.critters.Add(new Critter { id = 1, speciesId = "Fox", level = 1 });
            data.critters.Add(new Critter { id = 2, speciesId = "Frog", level = 1 });
            return data;
        }

        private static void RelinkSpecies(PlayerData data, CritterSpeciesLookup speciesLookup)
        {
            foreach (var critter in data.critters)
            {
                critter.species = speciesLookup.GetBySpeciesId(critter.speciesId);
                critter.CalculateStats();
            }
        }
    }
}
