MofoMojo's Random First Spawn Start Position v1.4

Please update to 1.4 (See Known Issues)

Are you interested in a potentially real challenge?
Care if you spawn on the middle of a small island in the map?
Care if you spawn sandwiched between a Black Forest, the Meadows and a Draugr village? What about in one?
Tired of spawning in the dead center of the map and wish you'd wake up in some other location?

If you answered yes to that last question, this is for you!!!

The mod now supports specifying a Biome type that you'd like to spawn in. 

The mod supports specifying a minimum/maximum distance from center of the map along the X and Z axis and will
look for locations anywhere within that area. Some biomes are constrained within a certain distance from the center of the map
so the value is clamped to this distance, however you can override this check if you wish. 

You can also disable the Valkyrie intro ride if you just want to pop up as quick as possible in your new homeland. 

You can now specify a BiomeAreaType to have better control over the "zone" area you're starting in
You can now specify a Biome to override Meadows. Good luck with this! May be a good idea for a seasoned character who got tired of retirement.

This mod ONLY randomizes YOUR first spawn into a map and sets your respawn point there. It does NOT mark this location on the map. 

If you die, you'll respawn there. Congratulations it's your death day!!!

:: NOTES ::
BiomeAreaType let's you specify the "area type" of the Zone you'll be spawning into. There are 3 types

Median - Means you're more likely to be surrounded  by this biome area 
Edge - Means you're more likely to spawn near the edges and borders of other biomes or other places
Everything - Means you're ok with whatever the RNG gods decide...

Biome setting let's you specify the type of Biome you prefer. Want to go truly random? Set it to None or BiomesMax and it'll place you in the first matching BiomeAreaType it finds. Why? Why do you want to do this?

:: REQUIREMENTS ::
• BepInEx - 

:: INSTALLATION ::
Place the MofoMojo.MMRandomStartPosition.dll in your \BepinEx\Plugins folder
Start Valheim and hold on to your pants
Modify the \BepInEx\Config\MofoMojo.MMRandomStartPosition.cfg file to enable the mod.
Play Valheim
Profit... or die.

:: UNINSTALLATION ::
Remove the .DLL and the .CFG file from the \Plugins and \Config folders respectively.

:: FEATURES ::
• Spawn in new and horrifying conditions
• Instantly die over and over because it placed you in a Draugr village
• Run away for your life and immediately out of stamina and die to a pack of boar!!!
• Disable the Valkyrie ride for an even quicker death
• You do NOT have to run this on a dedicated server to take advantage of this system on a dedicated server. Just install, activate, join the dedicated server and profit!!!

::  KNOWN ISSUES ::
• Can put you in hostile situations
• Sometimes it will put you in another biome because it's VERY close to the one you asked for. You know... you've been sailing in the water and 
	seen instances where you've passed through a biome that just wasn't there... yeah... like that. Only, I try to keep you dry. 
• World Generation is a fickle beast. I try to ensure that the location it puts you is a little above the ZoneSystem.instance.m_waterLevel
	to try and ensure you're not just placed in the water somewhere. I can't feasibly test this out very extensively but seems to be working! :D
• Versions 1.3 and prior didn't set the HomePoint permanently for your character. This creates an issue where if you log out and back on and THEN die you might get teleported elsewhere. Should be fixed in 1.4 and later
	however characters started prior to 1.4 will likely encounter this issue. Sorry!!!!

:: VERSIONS ::
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
﻿## Settings file was created by plugin MMRandomStartPosition v1.1
## Plugin GUID: MofoMojo.MMRandomStartPosition

[LoggingLevel]

## Supported values are None, Normal, Verbose
# Setting type: LoggingLevel
# Default value: None
# Acceptable values: None, Normal, Verbose
PluginLoggingLevel = Normal

[MMRandomStartPosition]

## Enables MMRandomStartPosition mod
# Setting type: Boolean
# Default value: true
MMRandomStartPositionEnabled = true

## Disables the ride in with Valkryie
# Setting type: Boolean
# Default value: false
DisableValkryieRide = true

## Constrain X axis search from center of map. This is clamped between 0 and WorldGenerator.meadowsMaxDistance = 5000
# Setting type: Single
# Default value: 5000
MaxXDistance = 5000

## Constrain Z axis search from center of map. This is clamped between 0 and WorldGenerator.meadowsMaxDistance = 5000
# Setting type: Single
# Default value: 5000
MaxZDistance = 5000

## Constrain X axis search from center of map. This is the minimum distance you wish to be away from center X/0 position
# Setting type: Single
# Default value: 100
MinXDistance = 2000

## Constrain Z axis search from center of map. This is the minimum distance you wish to be away from center z/0 position
# Setting type: Single
# Default value: 100
MinZDistance = 2000