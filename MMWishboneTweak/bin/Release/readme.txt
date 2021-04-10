MofoMojo's Wishbone Tweaks
This mod provides the ability for the Wishbone to find Copper, Tin and now Deathquitos (thanks to RaidoRadh for suggestion)!

Yes, it's pretty easy to find these... and you don't get a wishbone until well after you find copper and tin
but it was requested by a server mate. 

It currently extends the range at which Copper will be found to 60, and Tin at 25.
Traditionally, Silver and Treasure are found at 20.

Deathquito's default to 60 as well. 


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
• 1.1 By suggestion from RaidoRadh 
	Added ability to detect Deathsquitos
• 1.0 Initial Release

::  CREDITS ::
♦ https://www.youtube.com/watch?v=p_gsFASlvRw
♦ https://harmony.pardeike.net/ - Harmony Documentation
♦ https://github.com/Valheim-Modding/Wiki/wiki - Valheim modding
♦ RaidoRadh for his suggestions

:: SAMPLE CONFIG ::
## Settings file was created by plugin Wishbone Tweaks v1.1
## Plugin GUID: MofoMojo.MMWishboneTweak

[LoggingLevel]

## Supported values are None, Normal, Verbose
# Setting type: LoggingLevel
# Default value: None
# Acceptable values: None, Normal, Verbose
PluginLoggingLevel = Verbose

[Wishbone]

## Allows the Wishbone to find Tin and Copper
# Setting type: Boolean
# Default value: true
WishBoneTweakEnabled = true

## Allows the Wishbone to find Tin (Value > 0) and at what distance
# Setting type: Single
# Default value: 25
DetectTinDistance = 25

## Allows the Wishbone to find Copper (Value > 0) and at what distance
# Setting type: Single
# Default value: 60
DetectCopperDistance = 60

## Allows the Wishbone to find Deathquitos (Value > 0) and at what distance
# Setting type: Single
# Default value: 60
DetectDeathquitoDistance = 60

