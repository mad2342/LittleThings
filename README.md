# Little Things

[Battletech Mod][ModTek] Fix some little annoyances and change minor things.

## Fixes
- Bonuses display for Equipment had a missing comma added
- Fixed wrong getters for Resolve/Turn (obsolete with BTG 1.9.0)
- Replace hardcoded(!) tooltip descriptions for morale levels
    - Now the values are read from "BaselineAddFromSimGameValues" in CombatGameConstants.json
- Pilots health is now correctly displayed in Barracks and Hiring Halls (obsolete with BTG 1.8.1)
- Dead or incapacitated actors won't get inspired anymore
- Disabled "StockIcon" overlay in After Action Report (obsolete with BTG 1.9.0)
- Right-Click on MWTray Portraits always works
- Fixes some annoyances with Coil damage preview
- Sanitize some trait descriptions
- Workaround an empty contract list after priority mission on Artru
- Fix position of Heraldry Screen Title
- Disable highlights of some interactables in Argo's rooms
- Works around the annoying "Undeletable Saves" Bug for GOG Galaxy users (obsolete with BTG 1.9.0)
    - Builds path from users homedir + "GalaxySaveGameRelativeRootPath" and deletes files there if requested to do so
    - Option to override with a full, absolute path to savegamedir added
- Fix/Expand Stat Tooltip for Firepower
    - Shows correct values for stability damage now
    - Respects special gear such as Optimized Capacitors and Ballistic Siege Compensators
    - Shows breakdown of damage depending on weapon type (Energy, Ballistic, Missile, Support)
- Shrink the combat floaties for Structure/Armor-Damage

## Changes/Additions
- Enable Reserving for AI
    - Adjustable base chance
    - NO behaviour variables tweaked (You may want "BetterAI")
- DFA attacks remove entrenched state from enemy Mechs because it just makes sense
- Disable all additional tutorial slides in "Three years later"
    - And the one for Urban Warfare (Raven introduction)
    - And the one for the added Starmap features
- The cost for MechParts in Shops is now calculated depending on corresponding difficulty setting
    - Adjustable discount for MechParts to make them still worthwhile
- Respect "Heatsinks" in ChassisDefs (obsolete with BTG 1.9.0)
    - Requires Tag "chassis_heatsinks" on ChassisDef
- Open up the possibility of using "MechDef.Description.Model" to reference the base mech (aka stock)
    - Useful for custom MechDefs with broken/changed inventory/locations to properly show the stock loadout in MechLab
- Shows number of injuries for targets when injured (!health, because you can't know it)
- Suppress Mechwarrior training notification if number of trainable pilots is below a certain limit
    - Vanilla: 2
    - LittleThings: Configurable in settings (default: 4)
- Randomize the 'Mechs fielded for missions "Training Day" and "B-Team"
    - Especially helpful under circumstances where these missions spawn with a very high difficulty
    - At least one Urbie is preserved of course
    - On a side note: This is realized by opening up unit-selection by tags in contract.jsons (can be used for all contracts as a modders resource)

## Note
- Most fixes and features can be turned on/off via settings



## Thanks
* Mpstark
* KMiSSioN
* Redferne
* pardeike
* HBS
