MofoMojo's Extended Wishbone Tweaks 1.2
This mod provides the ability for the Wishbone to find several new items:

* Copper
* Tin
* Deathquitos (thanks to RaidoRadh for suggestion)!

Yes, it's pretty easy to find these... and you don't get a wishbone until well after you find copper and tin
but it was requested by a server mate. 

You can also configure their distances, and the distances for Silver, Mud Piles, and Buried Chests. 

Copper defaults to 60
Tin to 25
Deathsquitos 60

Silver, Mud Piles, and Buried Chests remain at their defaults of 20. 

To disable detection, set value to 0. 

:: REQUIREMENTS ::
• BepInEx - 

:: INSTALLATION ::
Place the MofoMojo.MMWishboneTweak.dll in your \BepinEx\Plugins folder
Start Valheim Once and then exit. 
Modify the \BepInEx\Config\MofoMojo.MMWishboneTweak.cfg as you see fit.
Play Valheim

:: UNINSTALLATION ::
Remove the .DLL and the .CFG file from the \Plugins and \Config folders respectively.

:: UPGRADING ::
Just replace the binary

:: FEATURES ::
• It does the things

::  KNOWN ISSUES ::
• Not aware of any for a change...oh, it doesn't make coffee.

:: VERSIONS :: 
• 1.2 By suggestion from Andrey059918
Added ability to adjust Silver, Mud Pile and Buried Treasure distances
• 1.1 By suggestion from RaidoRadh 
Added ability to detect Deathsquitos
• 1.0 Initial Release

::  CREDITS ::
♦ https://www.youtube.com/watch?v=p_gsFASlvRw
♦ https://harmony.pardeike.net/ - Harmony Documentation
♦ https://github.com/Valheim-Modding/Wiki/wiki - Valheim modding
♦ RaidoRadh for his suggestions
♦ Andrey059918 for his suggestions

:: SAMPLE CONFIG ::
## Settings file was created by plugin MM's Wishbone Tweaks v1.2
## Plugin GUID: MofoMojo.MMWishboneTweak

[LoggingLevel]

## Supported values are None, Normal, Verbose
# Setting type: LoggingLevel
# Default value: None
# Acceptable values: None, Normal, Verbose, Debug
PluginLoggingLevel = None

[Wishbone]

## Allows the Wishbone to find Tin and Copper
# Setting type: Boolean
# Default value: true
WishBoneTweakEnabled = true

[WishboneDestructibles]

## Allows the Wishbone to find Tin (Value > 0) and at what distance
# Setting type: Single
# Default value: 25
DetectTinDistance = 25

## Allows the Wishbone to find Copper (Value > 0) and at what distance
# Setting type: Single
# Default value: 60
DetectCopperDistance = 60

## Allows the Wishbone to find Silver (Value > 0) and at what distance
# Setting type: Single
# Default value: 20
DetectSilverDistance = 20

## Allows the Wishbone to find Mud Piles (Value > 0) and at what distance
# Setting type: Single
# Default value: 20
DetectMudPileDistance = 20

[WishboneHumanoids]

## Allows the Wishbone to find Deathquitos (Value > 0) and at what distance
# Setting type: Single
# Default value: 60
DetectDeathsquitoDistance = 60

[WishbonePieces]

## Allows the Wishbone to find Buried Objects (Value > 0) and at what distance
# Setting type: Single
# Default value: 20
DetectBuriedDistance = 20



My mods' source, and my personal mods that aren't published or are WIP are available here:
https://mofomojo.visualstudio.com/MofoMojoValheimMods/_git/MofoMojoValheimMods