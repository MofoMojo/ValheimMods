﻿MofoMojo's Random Spawn/Respawn Mod v2.0

2.0 Release!!!

Note: I have renamed the plugin binary!!! Please take special note of this if you are upgrading!!!

Are you interested in a potentially real challenge?
Care if you spawn on the middle of a small island in the map?
Care if you spawn sandwiched between a Black Forest, the Meadows and a Draugr village? What about in one?
Want to wake up in a new location EVERY... TIME... YOU ... DIE (only an option)
Tired of spawning in the dead center of the map and wish you'd wake up in some other location?

If you answered yes to that last question, this is for you!!!

:: FEATURES ::
• Spawn in new and horrifying conditions
• Instantly die over and over because it placed you in a Draugr village
• Run away for your life and immediately out of stamina and die to a pack of boar!!!
• You do NOT have to run this on a dedicated server to take advantage of this system on a dedicated server. Just install, activate, join the dedicated server and profit!!!
• Specify a Biome type that you'd like to spawn in on start of a new game or your death
• Specify whether you should respawn in a new location on every death
• Specify whether you should respawn (on death) in a new location ONLY if you don't have a bed
• Specify whether you should only spawn in random location at the start of the game 
	(um... this is done by just having RandomSpawnOnDeath and RandomSpawnOnDeathIfNoBed settings to their defaults of false)
• Disable the Valkrie intro ride if you just want to pop into a game as quick as possible.
• Capability to reset your homepoint (death respawn) to the StartTemple if you wish to respawn normally before uninstalling the mod


The mod supports specifying a minimum/maximum distance from center of the map along the X and Z axis and will
look for locations anywhere within that area. Some biomes are constrained within a certain distance from the center of the map
so the value is clamped to this distance, however you can override this check if you wish. 

You can specify a BiomeAreaType to have better control over the "zone" area you're starting in
You can specify a Biome to override Meadows. Good luck with this! May be a good idea for a seasoned character who got tired of retirement.

This mod randomizes YOUR first spawn into a map and sets your respawn point there. It does NOT mark this location on the map. 
You can use this mod to force a random re-spawn location on Death if you don't have a bed, or for harder games, on each death.

Congratulations it's your death day!!!

:: NOTES ::
BiomeAreaType let's you specify the "area type" of the Zone you'll be spawning into. There are 3 types

Median - Means you're more likely to be surrounded  by this biome area 
Edge - Means you're more likely to spawn near the edges and borders of other biomes or other places
Everything - Means you're ok with whatever the RNG gods decide...

Biome setting let's you specify the type of Biome you prefer. Want to go truly random? Set it to None or BiomesMax and it'll place you in the first matching BiomeAreaType it finds. Why? Why do you want to do this?

:: REQUIREMENTS ::
• BepInEx - 

:: INSTALLATION ::
Place the MofoMojo.MMRandomSpawnPoint.dll in your \BepinEx\Plugins folder
Start Valheim and hold on to your pants
Modify the \BepInEx\Config\MofoMojo.MMRandomStartPosition.cfg file to enable the mod.
Play Valheim
Profit... or die.

:: UPGRADING ::
If upgrading from 1.x, remove the MofoMojo.MMRandomStartPosition.DLL and backup the .CFG file (if you want to preserve settings) from the original version of the mod before running this mod
You'll need to at least start the game to get the new .CFG file. If you want to preserve settings, you'll need to manually copy them

:: UNINSTALLATION ::
If you want to reset your home point (where you spawn) back to the StartTemple, issue the following in chat (not console)
/resethomepoint

Then, remove the MofoMojo.MMRandomSpawnPoint.dll and MofoMojo.MMRandomSpawnPoint.CFG file from the \Plugins and \Config folders respectively.

::  KNOWN ISSUES ::
• Can put you in hostile situations
• Sometimes it will put you in another biome because it's VERY close to the one you asked for. You know... you've been sailing in the water and 
	seen instances where you've passed through a biome that just wasn't there... yeah... like that. Only, I try to keep you dry. 
• World Generation is a fickle beast. I try to ensure that the location it puts you is a little above the ZoneSystem.instance.m_waterLevel
	to try and ensure you're not just placed in the water somewhere. I can't feasibly test this out very extensively but seems to be working! :D

:: VERSIONS ::
2.0 A sort of re-write, and renamed the plugin. Sorry for the confusion there. 
	Added /resethomepoint
	Added RandomSpawnOnDeath option - will trigger a random respawn location when you die, bed set or not. If true, this setting takes precedence over RandomSpawnOnDeathIfNoBed
	Added RandomSpawnOnDeathIfNoBed option - will trigger a random respawn location when you die IF you don't have an active bed location
	At suggestion of Nibelung44, added "Hugin takes pity on you..." message if you end up getting spawned in the center of the map because no suitable spawn point could be found.
1.7 At request of emulegs, added IgnoreWaterDepthCheck parameter. 
	Mod previously attempted to always spawn you on terrain higher than the base water depth. This ignores that if set to true. 
1.6 At request of Redjparasite and backed up by emulegs, added RandomSpawnOnDeath config parameter. 
	Setting this to true will cause you to randomly respawn whenever you die. Enjoy the added difficulty you masochists. 
1.5 Reissue 1.4 (Nexus code wasn't updated)
1.4 Calling SetHomePoint after initial spawn point is identified
	Calling GetHomePoint death after game is reloaded. This combo should ensure no random relocations after death. Odin was not pleased with 1.3 and prior
1.3 Added BiomeArea type. Supported values are Median, Edge, Everything
	With the extension of supporting other biomes and biomearea types, there's a chance we won't find anything
	So added loop check. If we iterate through more than MaxSpawnPointChecks without finding a suitable place, default to the original behavior. 
	This gets reset with each new start

1.2 Fixed a random range bug I introduced so on 1.1 all locations were south west of 0,0
1.1 Added minimum ranges, renamed XDistance and ZDistance to MaxXDistance and MaxZDistance
1.0 Initial Release

::  CREDITS Template::
♦ https://www.youtube.com/watch?v=p_gsFASlvRw
♦ https://harmony.pardeike.net/ - Harmony Documentation
♦ https://github.com/Valheim-Modding/Wiki/wiki - Valheim modding

Sample Settings:
## Settings file was created by plugin MMRandomStartPosition v1.6
## Plugin GUID: MofoMojo.MMRandomStartPosition

[LoggingLevel]

## Supported values are None, Normal, Verbose
# Setting type: LoggingLevel
# Default value: None
# Acceptable values: None, Normal, Verbose
PluginLoggingLevel = Verbose

[MMRandomStartPosition]

## Enables MMRandomStartPosition mod
# Setting type: Boolean
# Default value: true
MMRandomStartPositionEnabled = true

## Disables the ride in on the Valkyrie
# Setting type: Boolean
# Default value: false
DisableValkryieRide = true

## Constrain X axis search from center of map. This is clamped between 0 and 10000
# Setting type: Single
# Default value: 5000
MaxXDistance = 5000

## Constrain Z axis search from center of map. This is clamped between 0 and 10000
# Setting type: Single
# Default value: 5000
MaxZDistance = 5000

## Constrain X axis search from center of map. This is the minimum distance you wish to be away from center X/0 position
# Setting type: Single
# Default value: 500
MinXDistance = 500

## Constrain Z axis search from center of map. This is the minimum distance you wish to be away from center z/0 position
# Setting type: Single
# Default value: 500
MinZDistance = 500

## Attempts to spawns you into this type of Biome
# Setting type: Biome
# Default value: Meadows
# Acceptable values: None, Meadows, Swamp, Mountain, BlackForest, Plains, AshLands, DeepNorth, Ocean, Mistlands, BiomesMax
Biome = Meadows

## Attempts to spawn you into this type of BiomeArea
# Setting type: BiomeArea
# Default value: Median
# Acceptable values: Edge, Median, Everything
BiomeAreaType = Median

## The maximum number of times mod will search for a good spawnpoint before handing off to normal spawn code
# Setting type: Int32
# Default value: 1000
MaxSpawnPointChecks = 1000

## The mod attempts to set biome search constraints based on values in WorldGenerator. Set this to true to disable these constraints.
# Setting type: Boolean
# Default value: false
IgnoreWorldGeneratorConstraints = false

## If true, You will generate a new respawn point on every death
# Setting type: Boolean
# Default value: false
RandomSpawnOnDeath = false