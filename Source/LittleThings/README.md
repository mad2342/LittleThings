# Little Things

[Battletech Mod][ModTek] Fix some little annoyances and change minor things.

## Fixes
- Bonuses display for Equipment had a missing comma added
- Fixed wrong getters for Resolve/Turn
- Replace hardcoded(!) tooltip descriptions for morale levels
    - Now the values are read from "BaselineAddFromSimGameValues" in CombatGameConstants.json
- Pilots health is now correctly displayed in Barracks and Hiring Halls (obsolete with BTG 1.8.1)
- Dead or incapacitated actors won't get inspired anymore
- Disabled "StockIcon" overlay in After Action Report 
- Right-Click on MWTray Portraits always works
- Fixes some annoyances with Coil damage preview

## Changes/Additions
- Enable Reserving for AI
    - Adjustable base chance
    - NO behaviour variables tweaked (You may want "BetterAI")
- DFA attacks remove entrenched state from enemy Mechs because it just makes sense
- Disable all additional tutorial slides in "Three years later" and the ones for Urban Warfare (Raven introduction)
- The cost for MechParts in Shops is now calculated depending on corresponding difficulty setting
    - Adjustable discount for MechParts to make them still worthwhile
- Respect "Heatsinks" in ChassisDefs
    - Requires Tag "chassis_heatsinks" on ChassisDef
- Shows number of injuries for targets when injured (!health, because you can't know it)



## Thanks
* Mpstark
* pardeike
* HBS
