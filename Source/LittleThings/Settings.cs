using System.Collections.Generic;

namespace LittleThings
{
    internal class Settings
    {
        public bool ReserveEnable = true;
        public float ReserveBasePercentage = 25f;

        public bool DFAsRemoveEntrenched = true;

        public bool MoraleFixes = true;

        public bool TooltipFixes = true;

        public bool LogComponentLists = true;
        public bool AddInventory = true;
        public bool AddInventoryMechs = false;
        public bool AddInventoryComponents = true;
        public int AddInventoryComponentCount = 10;
        public int AddInventoryFunds = 3000000;
    }
}
