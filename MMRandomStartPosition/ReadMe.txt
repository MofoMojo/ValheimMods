MofoMojo's Random First Spawn Start Position v1.1

Are you interested in a potentially real challenge?
Care if you spawn on the middle of a small island in the map?
Care if you spawn sandwiched between a Black Forest, the Meadows and a Draugr village? What about in one?
Tired of spawning in the dead center of the map and wish you'd wake up in some Meadows elsewhere?

If you answered yes to that last question, this is for you!!!

The mod supports specifying a maximum distance from center of the map along the X and Z axis and will
look for Meadows locations anywhere within that area. Meadows are constrained within 5000 distance from the center of the map
so the value is clamped to this distance, however if you're using a mod that alters the WorldGenerator.meadowsMaxDistance
then it will honor that distance

You can now specify a minimum distance to spawn from 0,0 in the .cfg file

You can also disable the Valkyrie intro ride if you just want to pop up as quick as possible in your new homeland. 

This mod ONLY randomizes YOUR first spawn into a map and sets your respawn point there. 

If you die, you'll respawn there. Congratulations it's your death day!!!


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
• Sometimes it will put you in another biome because it's VERY close to the Meadows. You know... you've been sailing in the water and 
seen instances where you've passed through a biome that just wasn't there... yeah... like that. Only, I try to keep you dry. 
• World Generation is a fickle beast. I try to ensure that the location it puts you is a little below the minimum height
for the mountain start (if you use a mod which alters that, it should hopefully pick it up) and above the ZoneSystem.instance.m_waterLevel
to try and ensure you're not just placed in the water somewhere. I can't feasibly test this out very extensively but seems to be working! :D

:: VERSIONS ::
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

## Disables the ride in with Hugin
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