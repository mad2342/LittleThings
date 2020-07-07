# Little Things

[Battletech Mod][ModTek] Fix some little annoyances and change minor things.

## Highlights
- Fix "Exit/Re-Enter Combat" bug for escort missions (Interleaved Dropouts)
- Fix/Expand Stat Tooltip for Firepower and Durability
    - Shows correct values for stability damage now
    - Respects special gear such as Optimized Capacitors and Ballistic Siege Compensators
    - Shows breakdown of damage depending on weapon type (Energy, Ballistic, Missile, Support)
- Replace hardcoded(!) tooltip descriptions for morale levels
    - Now the values are read from "BaselineAddFromSimGameValues" in CombatGameConstants.json
- Dead or incapacitated actors won't get inspired anymore
- Fixes some annoyances with Coil damage preview
- DFA attacks remove entrenched state from enemy Mechs because it just makes sense
- Disable all additional tutorial slides in "Three years later"
    - And the one for Urban Warfare (Raven introduction)
    - And the one for the added Starmap features
- Suppress Mechwarrior training notification if number of trainable pilots is below a certain limit
    - Vanilla: 2
    - LittleThings: Configurable in settings (default: 4)
- Randomize the 'Mechs fielded for missions "Training Day" and "B-Team"
    - Especially helpful under circumstances where these missions spawn with a very high difficulty
    - At least one Urbie is preserved of course
- Shrink the combat floaties for Structure/Armor-Damage

## Full list
### Fixes
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
- Fix/Expand Stat Tooltip for Firepower and Durability
    - Shows correct values for stability damage now
    - Respects special gear such as Optimized Capacitors and Ballistic Siege Compensators
    - Shows breakdown of damage depending on weapon type (Energy, Ballistic, Missile, Support)
- Fix "Exit/Re-Enter Combat" bug for escort missions (Interleaved Dropouts)

### Changes/Additions
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
- Shrink the combat floaties for Structure/Armor-Damage
- Unlock Alliance FPs at configurable reputation level
- Make Heavy Metal Campaign potentially repeatable
- Disable the useless "-1 INITIATIVE" floatie for dead actors
- Disable Career Mode Scoring
- Disable Heavy Metal Loot Popup

## Settings

| Setting                                            | Default | Description                                                                                                                                                                                                                                                                                   |
|----------------------------------------------------|---------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| MoraleFixes                                        | true    | Fixes wrong getters for Resolve/Turn (obsolete with BTG 1.9.0)  Replaces hardcoded(!) tooltip descriptions for morale levels  The values are now read from "BaselineAddFromSimGameValues" in CombatGameConstants.json                                                                         |
| InjuryFixes                                        | false   | Pilots health is now correctly displayed in Barracks and Hiring Halls (obsolete with BTG 1.8.1)                                                                                                                                                                                               |
| -                                                  | -       | -                                                                                                                                                                                                                                                                                             |
| FixUIInventoryItems                                | true    | Expands bonusText fields                                                                                                                                                                                                                                                                      |
| FixUIHeraldryScreen                                | true    | Fixes position of Heraldry Screen Title                                                                                                                                                                                                                                                       |
| FixUIEquipmentTooltips                             | true    | Adds a missing comma to bonuses display of equipment                                                                                                                                                                                                                                          |
| FixUIMainNavigation                                | true    | Prevents collapsing of submenu due to the 1px gap @1920 screen width                                                                                                                                                                                                                          |
| -                                                  | -       | -                                                                                                                                                                                                                                                                                             |
| FixInterleavedDropouts                             | true    | Works around the "Exit/Re-Enter Combat" bug for escort missions                                                                                                                                                                                                                               |
| FixInspire                                         | true    | Dead or incapacitated actors won't get inspired anymore                                                                                                                                                                                                                                       |
| FixIconOverlayAAR                                  | false   | Disables the "StockIcon" overlay in After Action Report (obsolete with BTG 1.9.0)                                                                                                                                                                                                             |
| FixCombatHUDPortraitRightClick                     | true    | Right-Click on MWTray Portraits will always work                                                                                                                                                                                                                                              |
| FixCoilPreviews                                    | true    | Fixes some annoyances with Coil damage preview during melee                                                                                                                                                                                                                                   |
| FixRepairNotification                              | true    | Suppresses repair notifications when more than four fieldable Mechs are ready                                                                                                                                                                                                                 |
| FixTaurianReputationPostCampaign                   | true    | Applies a reputation fix for the Taurian Concordat after campaign is concluded                                                                                                                                                                                                                |
| FixStatTooltipFirepower                            | true    | Firepower Tooltip now shows correct values for stability damage   Firepower Tooltip now respects special gear such as Optimized Capacitors and Ballistic Siege Compensators  Firepower Tooltip now shows a breakdown of damage depending on weapon type (Energy, Ballistic, Missile, Support) |
| FixStatTooltipDurability                           | true    | Durability Tooltip now respects special gear such as Ballistic Siege Compensators                                                                                                                                                                                                             |
| FixInitiativeFloatieForDeadActors                  | true    | Disables the useless "-1 INITIATIVE" floatie for dead actors                                                                                                                                                                                                                                  |
| -                                                  | -       | -                                                                                                                                                                                                                                                                                             |
| DisableTutorials                                   | true    | Disables all additional tutorial slides in "Three years later"  Disables the tutorial slide for Urban Warfare (Raven introduction)  Disables the tutorial slide for the added Starmap features                                                                                                |
| DisableSimGameCharHighlights                       | true    | Disables highlights of some interactables in Argo's rooms                                                                                                                                                                                                                                     |
| DisableCareerModeScoring                           | true    | Disables Countdown and Score Breakdown for Career Mode                                                                                                                                                                                                                                        |
| DisableHeavyMetalLootPopup                         | true    | No cheating anymore                                                                                                                                                                                                                                                                           |
| -                                                  | -       | -                                                                                                                                                                                                                                                                                             |
| EnableEnemyInjuryFloaties                          | true    | Shows number of injuries for targets when injured (!health, because you can't know it)                                                                                                                                                                                                        |
| EnableSmallCombatFloaties                          | true    | Shrinks the combat floaties for Structure/Armor-Damage                                                                                                                                                                                                                                        |
| EnableAbilityTooltips                              | true    | Mechwarrior skills now show trait descriptions                                                                                                                                                                                                                                                |
| EnableStockMechReferenceViaMechDefDescriptionModel | true    | Opens up the possibility of using "MechDef.Description.Model" to reference the base mech (aka stock)  Useful for custom MechDefs with broken/changed inventory/locations to properly show the stock loadout in MechLab                                                                        |
| EnableChassisHeatsinks                             | false   | Respect "Heatsinks" in ChassisDefs (obsolete with BTG 1.9.0)  Requires Tag "chassis_heatsinks" on ChassisDef                                                                                                                                                                                  |
| EnableSpawnProtection                              | false   | Mechs are now guarded when entering combat                                                                                                                                                                                                                                                    |
| EnableLanceConfigurationByTags                     | true    | Opens up unit-selection by tags in contract.jsons (can be used for all contracts as a modders resource)                                                                                                                                                                                       |
| EnableContractsTakeTime                            | false   | Taking contracts will pass one day                                                                                                                                                                                                                                                            |
| EnableDFAsRemoveEntrenched                         | true    | DFA attacks remove entrenched state from Target Mechs because it just makes sense                                                                                                                                                                                                             |
| EnableRepeatableHeavyMetalCampaign                 | false   | Makes the Heavy Metal Campaign potentially repeatable                                                                                                                                                                                                                                         |
| -                                                  | -       | -                                                                                                                                                                                                                                                                                             |
| EnableAIReserve                                    | false   | Enables Reserving for AI  NO behaviour variables tweaked (You may want "BetterAI")                                                                                                                                                                                                            |
| EnableAIReserveBasePercentage                      | 25      | Base Chance for the AI to reserve                                                                                                                                                                                                                                                             |
| -                                                  | -       | -                                                                                                                                                                                                                                                                                             |
| EnableAdjustedMechPartCost                         | true    | The cost for MechParts in Shops is now calculated depending on corresponding difficulty setting                                                                                                                                                                                               |
| EnableAdjustedMechPartCostMultiplier               | 0.8     | Adjustable discount for MechParts to make them still worthwhile                                                                                                                                                                                                                               |
| -                                                  | -       | -                                                                                                                                                                                                                                                                                             |
| EnableTrainingNotification                         | true    | Suppresses Mechwarrior training notification if number of trainable pilots is below a certain limit                                                                                                                                                                                           |
| EnableTrainingNotificationLimit                    | 4       | The limit described above (Vanilla: 2)                                                                                                                                                                                                                                                        |
| -                                                  | -       | -                                                                                                                                                                                                                                                                                             |
| EnableAllianceFlashpoints                          | true    | Unlocks Alliance FPs at configurable reputation level without the need to actually be allied with the Faction                                                                                                                                                                                 |
| EnableAllianceFlashpointsAtReputation              | 100     | The required reputation to have with a faction to unlock their alliance FP                                                                                                                                                                                                                    |

## Note
In addition to the above the following things are addressed by json overrides:

- Sanitize some trait descriptions
- Workaround an empty contract list after priority mission on Artru
- Randomize the 'Mechs fielded for missions "Training Day" and "B-Team"
    - Especially helpful under circumstances where these missions spawn with a very high difficulty
    - At least one Urbie is preserved of course


## Thanks
* Mpstark
* KMiSSioN
* Redferne
* pardeike
* HBS
