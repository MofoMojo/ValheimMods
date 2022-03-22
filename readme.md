# MofoMojo's Mods

## Releases
### ExploreOnShipRadius
https://github.com/MofoMojo/ValheimMods/tree/master/MMExploreOnShipRadius/bin/Release

### PlayerMiniMapMod
https://github.com/MofoMojo/ValheimMods/tree/master/MMPlayerMiniMapMod/bin/Release

### ShowPlayerStatsInRange
https://github.com/MofoMojo/ValheimMods/tree/master/MMShowPlayerStatsInRange/bin/Release

### Wishbone tweak
https://github.com/MofoMojo/ValheimMods/tree/master/MMWishboneTweak/bin/Release

### Personal tweaks
https://github.com/MofoMojo/ValheimMods/tree/master/MMPersonalTweaks/bin/Release

### GuardStone Tweaks
https://github.com/MofoMojo/ValheimMods/tree/master/MMGuardStoneMod/bin/Release
**Experimental - Creates a no monsters area with wards, but works a little sloppy on servers due to client/server behavior 

### MM's Prayers for Rain Mod
https://github.com/MofoMojo/ValheimMods/tree/master/MMWeatherMod/bin/Release

### MM's Random First Spawn Start Position
https://github.com/MofoMojo/ValheimMods/tree/master/MMRandomSpawnPoint/bin/Release

### MM's Drop Item using Camera Direction
https://github.com/MofoMojo/ValheimMods/tree/master/MMDropItemOnCameraDirection/bin/Release

### MM's Personal Recipe Tweaks
https://github.com/MofoMojo/ValheimMods/tree/master/MMRecipeTweaks/bin/Release

### MM's Personal Server Messages
https://github.com/MofoMojo/ValheimMods/tree/master/MMServerMessages/bin/Release

# Details
## MMRandomStartPosition (Updated for H&H/Frostcaves)
Randomizes a players first spawn location on every map started. Players will no longer (most likely) be starting in the center of the map and their spawn locations will be different from others (assuming they're running this mod too).

## MMWeatherMod
Allows the player to /pray in the chat window to ask Odin to bless them with clear weather. It does not always work, sometimes you will anger Odin, and you must be seated and outside (or on a boat). 

## ExploreOnShipRadius (Updated for H&H/Frostcaves)
Extends the ship radius when you're standing on a boat. I made this because an existing mod didn't work for players on the boat, just piloting the boat.

## PlayerMiniMapMod
Allows you to toggle the minimap and specify independent scaling of the minimap marker and the large map marker

## RecipeTweaks (Updated for H&H/Frostcaves)
FishingRodRecipe requires Wood and LinenThread. Yes, this is fairly late game and you probably already have one by then but thematically it makes sense. 
* Bait requires raw Neck meat
* Chains requires 4 iron
* LoxMeatSurprise requires CarrotSoup and LoxPie
* BronzeTweak gives you 3 bronze for 1 tin and 2 copper, or 15 for 5 tin and 10 copper
* Convert 3 LeatherScraps into 1 DeerHide
* Convert 1 DeerHide into 3 LeatherScraps

## ShowPlayerStatsInRange 
PlayerStats will show health data for all nearby players within a given range of you. It allows you to configure various health and healthpercentage levels with customizable levels of criticality. As for coloring, the most critical level wins. In other words, if your health is considered critical, but your health percentage is Medium, then stats are colored with critical coloring. if your health percentage is critical, but your health level indicates it should be normal then the coloring will be critical.

Default color levels are from Normal to Critical:

* Green
* White
* Yellow
* Red

## Wishbone tweak
Allows the wishbone to track on Copper and Tin

## Personal tweaks
Birds drop an additional 1-3 feathers
Fishing in the ocean biome provides 1-3 extra fish
Remember the IP address and port of the last server you connected to

## Guardstone Tweaks
This does several things. 

### WardActivationBehavior
Supports a *WardActivationBehavior* setting now with 4 different modes
* All
* OwnerAndPermitted
* OwnerOnly
* Original (Will revert to original OOTB behavior for permitted and activation)

**All**  
All players on the server can activate/deactivate wards and add/remove themselves as permitted players on wards  
**OwnerAndPermitted**  
Owners and Permitted Players can activate and deactivate the wards  
**OwnerOnly**  
Only owners can activate and deactivate the ward  

**Note:**
When using All or OwnerAndPermitted, a SHIFT key modifier is used to allow players to add/remove themselves from the permitted list when the ward is deactivated.

### WardBehavior
Ward Behavior allows you to configure additional behaviors
* Original
* NoMonsters

#### Original
Does not change the behavior of a ward
#### NoMonsters
Creates a NoMonsters EffectArea attached to the ward. 
No monsters should spawn in the NoMonster EffectArea and as of the latest implementation, should be able to enter and leave the NoMonster area but will not be able to target anything in the area. 

**Note:**
In a Server/Client relationship the server must also have the mod and it's recommended that all clients have the same settings due to behavior of spawns and client/server relationships.


### Toggle Keys
Use the values here: https://docs.unity3d.com/ScriptReference/KeyCode.html to determine what can be used/specified as a toggle key

**Note: The default settings are the settings that I use**

## Useful Tutorial and other Sites
https://valheim-modding.github.io/Jotunn/home/tutorials/overview.html  
https://www.youtube.com/watch?v=p_gsFASlvRw - The video that started this adventure for me  
https://harmony.pardeike.net/ - Harmony Documentation  
https://github.com/Valheim-Modding/Wiki/wiki - Valheim modding  
https://jotunnlib.github.io/jotunnlib/conceptual/prefabs/prefab-list.html  
https://morevalheim.com/  
https://github.com/Valheim-Modding/Wiki/wiki/Valheim-Unity-Project-Guide  
