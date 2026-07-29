using System;
using System.Collections.Generic;
using CritterRally.Equipment;

namespace CritterRally.Critters
{
    /// <summary>
    /// Runtime state for one owned critter instance. Stats are derived from
    /// its CritterSpecies definition + level + equipped gadgets, never
    /// hand-authored per instance (see CLAUDE.md Rule 2).
    /// </summary>
    [Serializable]
    public class Critter
    {
        public int id;
        public string speciesId; // matches CritterSpecies.speciesName, used for save lookup
        public int level = 1;
        public int experience;

        [NonSerialized] public CritterSpecies species;
        [NonSerialized] public List<Equipment.Equipment> equippedGadgets = new();

        public int Sprint { get; private set; }
        public int Jump { get; private set; }
        public int Dig { get; private set; }
        public int Swim { get; private set; }
        public int Balance { get; private set; }

        public void CalculateStats()
        {
            if (species == null)
                throw new InvalidOperationException(
                    $"Critter {id} has no species assigned; cannot calculate stats.");

            Sprint = (int)(species.baseSprint + species.growthSprint * level);
            Jump = (int)(species.baseJump + species.growthJump * level);
            Dig = (int)(species.baseDig + species.growthDig * level);
            Swim = (int)(species.baseSwim + species.growthSwim * level);
            Balance = (int)(species.baseBalance + species.growthBalance * level);

            foreach (var gadget in equippedGadgets)
            {
                Sprint += gadget.bonusSprint;
                Jump += gadget.bonusJump;
                Dig += gadget.bonusDig;
                Swim += gadget.bonusSwim;
                Balance += gadget.bonusBalance;
            }
        }
    }
}
