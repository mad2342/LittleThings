using System.Collections.Generic;
using BattleTech;

namespace LittleThings
{
    public static class Utilities
    {
        public static int GetReadiedWeaponCount(List<Weapon> weapons, ICombatant target, bool log = false)
        {
            int readiedWeaponCount = 0;
            foreach (Weapon w in weapons)
            {
                bool willFireAtTarget = w.WillFireAtTarget(target);
                if (w.IsEnabled && willFireAtTarget)
                {
                    if (log)
                    {
                        Logger.LogLine($"[Utilities_GetReadiedWeaponCount] {w.Name} is enabled and will fire at target({target.DisplayName})!");
                    }
                    readiedWeaponCount++;
                }
                else
                {
                    if (log)
                    {
                        Logger.LogLine($"[Utilities_GetReadiedWeaponCount] {w.Name} is not ready!");
                    }
                }
            }
            return readiedWeaponCount;
        }

        public static int GetMaxAllowedWeaponCountForHeadshots(Pilot p, bool log = false)
        {
            int allowedWeaponCount = 1;
            int gunnery = p.Gunnery;

            if (log)
            {
                Logger.LogLine($"[Utilities_GetMaxAllowedWeaponCountForHeadshots] {p.Callsign} has gunnery: {gunnery}");
            }

            if (gunnery >= 7)
            {
                allowedWeaponCount += 1;
            }
            if (gunnery >= 10)
            {
                allowedWeaponCount += 2;
            }

            return allowedWeaponCount;
        }
    }
}