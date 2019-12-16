using System.Collections.Generic;

namespace LittleThings
{
    internal class Settings
    {
        public bool MoraleFixes = true;
        public bool UIFixes = true;
        public bool DisableTutorials = true;
        public bool InjuryFixes = true;
        public bool AdjustMechPartCost = true;
        public double AdjustMechPartCostMultiplier = 0.8;

        public bool ReserveEnable = true;
        public float ReserveBasePercentage = 25f;

        public bool DFAsRemoveEntrenched = true;
    }
}
