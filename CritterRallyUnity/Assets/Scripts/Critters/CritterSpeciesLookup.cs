using System.Collections.Generic;
using UnityEngine;

namespace CritterRally.Critters
{
    /// <summary>
    /// Registry of all known CritterSpecies assets, keyed by speciesName.
    /// Used by SaveManager to re-link a loaded Critter's species reference,
    /// since ScriptableObject references don't survive JsonUtility round-trips.
    /// </summary>
    [CreateAssetMenu(fileName = "CritterSpeciesLookup", menuName = "CritterRally/Critter Species Lookup")]
    public class CritterSpeciesLookup : ScriptableObject
    {
        public List<CritterSpecies> allSpecies = new();

        public CritterSpecies GetBySpeciesId(string speciesId)
        {
            foreach (var species in allSpecies)
            {
                if (species.speciesName == speciesId)
                    return species;
            }

            Debug.LogError($"CritterSpeciesLookup: no species found for id '{speciesId}'.");
            return null;
        }
    }
}
