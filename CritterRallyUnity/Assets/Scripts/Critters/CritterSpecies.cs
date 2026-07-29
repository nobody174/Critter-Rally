using UnityEngine;

namespace CritterRally.Critters
{
    /// <summary>
    /// Definition data for a critter species: base stats and per-level growth.
    /// Numbers per ROADMAP.md "Species stat identity" (locked 2026-07-29, first pass).
    /// </summary>
    [CreateAssetMenu(fileName = "NewCritterSpecies", menuName = "CritterRally/Critter Species")]
    public class CritterSpecies : ScriptableObject
    {
        public string speciesName;

        [Header("Base stats (level 1)")]
        public float baseSprint;
        public float baseJump;
        public float baseDig;
        public float baseSwim;
        public float baseBalance;

        [Header("Growth per level")]
        public float growthSprint;
        public float growthJump;
        public float growthDig;
        public float growthSwim;
        public float growthBalance;
    }
}
