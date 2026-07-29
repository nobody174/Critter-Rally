using UnityEngine;
using CritterRally.Race;

namespace CritterRally.Equipment
{
    /// <summary>
    /// Factory for the 5 launch gadgets. Numbers per ROADMAP.md "Gadgets"
    /// section (locked 2026-07-29, first pass). Returns fresh
    /// ScriptableObject instances rather than shared assets, so callers
    /// (e.g. test harnesses) don't need an AssetDatabase reference — actual
    /// gameplay should prefer wiring real .asset files via
    /// CreateAssetMenu once a gadget-select UI exists.
    /// </summary>
    public static class EquipmentLibrary
    {
        public static Equipment RocketAcorns() => Make(
            "Rocket Acorns", TerrainType.Ground, sprint: 15, jump: 5);

        public static Equipment LeafGliders() => Make(
            "Leaf Gliders", TerrainType.Ground, jump: 20, sprint: -5);

        public static Equipment MudSkis() => Make(
            "Mud Skis", TerrainType.Water, swim: 18, jump: -10);

        public static Equipment BerryShields() => Make(
            "Berry Shields", TerrainType.Tightrope, balance: 15, sprint: -5);

        public static Equipment VineWhips() => Make(
            "Vine Whips", TerrainType.Burrow, dig: 15, balance: -5);

        private static Equipment Make(
            string name, TerrainType terrain,
            int sprint = 0, int jump = 0, int dig = 0, int swim = 0, int balance = 0)
        {
            var equipment = ScriptableObject.CreateInstance<Equipment>();
            equipment.equipmentName = name;
            equipment.primaryTerrainType = terrain;
            equipment.bonusSprint = sprint;
            equipment.bonusJump = jump;
            equipment.bonusDig = dig;
            equipment.bonusSwim = swim;
            equipment.bonusBalance = balance;
            return equipment;
        }
    }
}
