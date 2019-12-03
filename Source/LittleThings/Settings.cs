using System.Collections.Generic;

namespace LittleThings
{
    internal class Settings
    {
        public bool ReserveEnable = true;
        public float ReserveBasePercentage = 25f;

        public bool DFAsRemoveEntrenched = true;

        public bool MoraleFixes = true;

        public bool AddInventory = false;
        public List<string> AddInventoryMechs = new List<string>();
        public List<string> AddInventoryWeapons = new List<string>();
        public List<string> AddInventoryUpgrades = new List<string>();
        public List<string> AddInventoryHeatsinks = new List<string>();
        public List<string> AddInventoryAmmo = new List<string>();
        public int AddInventoryFunds = 0;
    }
}
