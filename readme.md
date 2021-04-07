# MofoMojo's Mods

##Releases
###ExploreOnShipRadius
https://stuartp.visualstudio.com/_git/MofoMojoValheimMods?path=%2FMMExploreOnShipRadius%2Fbin%2FRelease

###PlayerMiniMapMod
https://stuartp.visualstudio.com/_git/MofoMojoValheimMods?path=%2FMMPlayerMiniMapMod%2Fbin%2FRelease

###ShowPlayerStatsInRange 
https://stuartp.visualstudio.com/_git/MofoMojoValheimMods?path=%2FMMShowPlayerStatsInRange%2Fbin%2FRelease

###Wishbone tweak
https://stuartp.visualstudio.com/_git/MofoMojoValheimMods?path=%2FMMWishboneTweak%2Fbin%2FRelease

###Personal tweaks
https://stuartp.visualstudio.com/_git/MofoMojoValheimMods?path=%2FMMPersonalTweaks%2Fbin%2FRelease

###GuardStone Tweaks
https://stuartp.visualstudio.com/_git/MofoMojoValheimMods?path=%2FMMGuardStoneMod%2Fbin%2FRelease

**Experimental - Creates a no monsters area with wards, but works a little sloppy on servers due to client/server behavior for AI/Spawns**


#Details
##ExploreOnShipRadius
Extends the ship radius when you're standing on a boat. I made this because an existing mod didn't work for players on the boat, just piloting the boat.

##PlayerMiniMapMod
Allows you to toggle the minimap and specify independent scaling of the minimap marker and the large map marker

##RecipeTweaks
FishingRodRecipe requires Wood and LinenThread. Yes, this is fairly late game and you probably already have one by then but thematically it makes sense. 
* Bait requires raw Neck meat
* Chains requires 4 iron
* LoxMeatSurprise requires CarrotSoup and LoxPie
* BronzeTweak gives you 3 bronze for 1 tin and 2 copper, or 15 for 5 tin and 10 copper

##ShowPlayerStatsInRange 
PlayerStats will show health data for all nearby players within a given range of you. It allows you to configure various health and healthpercentage levels with customizable levels of criticality. As for coloring, the most critical level wins. In other words, if your health is considered critical, but your health percentage is Medium, then stats are colored with critical coloring. if your health percentage is critical, but your health level indicates it should be normal then the coloring will be critical.

Default color levels are from Normal to Critical:

* Green
* White
* Yellow
* Red

##Wishbone tweak
Allows the wishbone to track on Copper and Tin

##Personal tweaks
Birds drop an additional 1-3 feathers
Fishing in the ocean biome provides 1-3 extra fish
Remember the IP address and port of the last server you connected to

##Guardstone Tweaks
This does several things. 

###WardActivationBehavior
Supports a *WardActivationBehavior* setting now with 4 different modes
* All
* OwnerAndPermitted
* OwnerOnly
* Original (Will revert to original OOTB behavior for permitted and activation)

####All
All players on the server can activate/deactivate wards and add/remove themselves as permitted players on wards
####OwnerAndPermitted
Owners and Permitted Players can activate and deactivate the wards
####OwnerOnly
Only owners can activate and deactivate the ward

**Note:**
When using All or OwnerAndPermitted, a SHIFT key modifier is used to allow players to add/remove themselves from the permitted list when the ward is deactivated.

###WardBehavior
Ward Behavior allows you to configure additional behaviors
* Original
* NoMonsters

####Original
Does not change the behavior of a ward
####NoMonsters
Creates a NoMonsters EffectArea attached to the ward. You cannot build when the ward is active. 
No monsters should spawn in the NoMonster EffectArea however any monsters in the radius, when activated, will STAY in the sphere and won't leave.
This ONLY prevents monsters from targetting something inside the area from outside the area, and thus they won't move into it, however swarms of monsters will PUSH others into the area and once in, they will not leave.

**Note:**
In a Server/Client relationship the server must also have the mod and it's recommended that all clients have the same settings due to behavior of spawns and client/server relationships.


### Toggle Keys
Use the values here: https://docs.unity3d.com/ScriptReference/KeyCode.html to determine what can be used/specified as a toggle key

**Note: The default settings are the settings that I use**