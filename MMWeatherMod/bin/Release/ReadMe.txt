MofoMojo's Prayers for Rain Mod v 1.1
This mod is a simple mod that allows you to pray for clear weather. 

When outside, with the sky above you
	Have a seat and pray
When the fog's against you and your ships' crew
	Have a seat and pray

:: USE ::
Open the chat window and issue 
/pray

or ... 

:: REQUIREMENTS ::
• BepInEx - 

:: INSTALLATION ::
Place the MMWeatherMod.dll in your \BepinEx\Plugins folder
Start Valheim Once and then exit. 
Modify the \BepInEx\Config\MofoMojo.MMWeatherMod.cfg as you see fit.
Play Valheim

:: UNINSTALLATION ::
Remove the .DLL and the .CFG file from the \Plugins and \Config folders respectively.

:: UPGRADING ::
I recommend deleting your current .cfg and letting a new one generate with defaults. 
Nothing bad will happen if you don't...it's not like Odin is going to smite you, but you'll experience the mod the way it was intended.

:: FEATURES ::
• You can pray for other weather such as rain, thunderstorm, lightrain, snow, and others that i'll leave up to you to find.
• Uses Localization strings built into the game for hints...
• No dramatic instant changes in Weather. It will be "queued" properly
• Your unique kills will influence your behavior. 2% for each unique kill

::  KNOWN ISSUES ::
• You must be outside
• You must be seated or sitting or in a boat
• You can't always get what you want...
• Unfortunately weather is largely handled client side so there will be inconistency when used in online sessions. 
	For that reason, you have been chopped.... I mean, I recommend single player sessions only at this time.

:: VERSIONS :: 
• 1.1 By suggestion from greenskye
	Added unique kills increase base chance of success option. 2% per unique kill (see your trophy wall)
		This can be turned on/off via cfg
	Added SuccessChance
		Angry Chance
		Angry Odin summons weather more appropriate to environment
		If angry Odin summons weather innappropriately, let me know and I'll have a stern talking to him (let me know the biome and weather you think it should be)
	
• 1.0 Initial Release

::  CREDITS ::
♦ https://www.youtube.com/watch?v=p_gsFASlvRw
♦ https://harmony.pardeike.net/ - Harmony Documentation
♦ https://github.com/Valheim-Modding/Wiki/wiki - Valheim modding
♦ greenskye for his suggestions

:: SAMPLE CONFIG ::

## Settings file was created by plugin MMWeatherMod v1.1
## Plugin GUID: MofoMojo.MMWeatherMod

[LoggingLevel]

## Supported values are None, Normal, Verbose
# Setting type: LoggingLevel
# Default value: None
# Acceptable values: None, Normal, Verbose
PluginLoggingLevel = None

[MMWeatherMod]

## Enables MMWeatherMod mod
# Setting type: Boolean
# Default value: true
MMWeatherModEnabled = true

## How many seconds must pass between attempts
# Setting type: Int32
# Default value: 60
PrayerFrequency = 60

## What are the odds Odin will grant your prayer wish
# Setting type: Single
# Default value: 33
SuccessChance = 33

## What are the odds you'll anger Odin if your prayer is unsuccessful
# Setting type: Single
# Default value: 25
AngryChance = 25

## Each new mob type you've slain gives you a 2% increase in success
# Setting type: Boolean
# Default value: true
TrophyKillInfluence = true

