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
- Sanitize some trait descriptions
- Workaround an empty contract list after priority mission on Artru
- Fix position of Heraldry Screen Title
- Disable highlights of some interactables in Argo's rooms.

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
- Open up the possibility of using "MechDef.Description.Model" to reference the base mech (aka stock)
    - Useful for custom MechDefs with broken/changed inventory/locations to properly show the stock loadout in MechLab
- Shows number of injuries for targets when injured (!health, because you can't know it)

## Note
- All fixes and features can be turned on/off via settings



## Thanks
* Mpstark
* pardeike
* HBS
