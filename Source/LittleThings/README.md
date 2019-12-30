# Little Things

[Battletech Mod][ModTek] Fix some little annoyances and change minor things.

## Fixes
- Bonuses display for Equipment had a missing comma added
- Fixed wrong getters for Resolve/Turn
- Replace hardcoded(!) tooltip descriptions for morale levels
    - Now the values are read from "BaselineAddFromSimGameValues" in CombatGameConstants.json
- Pilots health is now correctly displayed in Barracks and Hiring Halls (obsolete with BTG 1.8.1)
- Dead or incapacitated actors won't get inspired anymore

## Changes/Additions
- Enable Reserving for AI
    - Adjustable base chance
    - NO behaviour variables tweaked (You may want "BetterAI")
- DFA attacks remove entrenched state from enemy Mechs because it just makes sense
- Disable all additional tutorial slides in "Three years later" and the ones for Urban Warfare (Raven introduction)
- The cost for MechParts in Shops is now calculated depending on corresponding difficulty setting
    - Adjustable discount for MechParts to make them still worthwhile

## Thanks
* Mpstark
* pardeike
* HBS
