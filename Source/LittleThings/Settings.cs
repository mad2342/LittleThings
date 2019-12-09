using System.Collections.Generic;

namespace LittleThings
{
    internal class Settings
    {
        public bool MoraleFixes = true;
        public bool TooltipFixes = true;
        public bool InjuryFixes = true;

        public bool ReserveEnable = true;
        public float ReserveBasePercentage = 25f;

        public bool DFAsRemoveEntrenched = true;

        public bool AddInventory = false;
        public bool AddInventoryMechs = false;
        public List<string> AddInventoryMechsList = new List<string>();
        public bool AddInventoryComponents = true;
        public int AddInventoryComponentCount = 10;
        public int AddInventoryFunds = 3000000;

        public bool LogComponentLists = true;
    }
}
