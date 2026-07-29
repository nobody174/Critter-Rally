using UnityEngine;
using CritterRally.Race;

namespace CritterRally.Equipment
{
    /// <summary>
    /// Definition data for an equippable gadget ("Nature Tool"). Flat stat
    /// bonuses, some with tradeoffs, each favors a terrain type.
    /// </summary>
    [CreateAssetMenu(fileName = "NewEquipment", menuName = "CritterRally/Equipment")]
    public class Equipment : ScriptableObject
    {
        public string equipmentName;

        public int bonusSprint;
        public int bonusJump;
        public int bonusDig;
        public int bonusSwim;
        public int bonusBalance;

        public TerrainType primaryTerrainType;
    }
}
