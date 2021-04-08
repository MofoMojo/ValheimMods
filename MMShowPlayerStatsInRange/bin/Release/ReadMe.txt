MofoMojo's Player Stats Mod
This mod is a UI tweak to add nearby players' health and health percentages to the HUD.
The player information is color coded based on the Players Health or Health Percantage. 

Health and Health Percentage is classified into 4 different categories.
* Normal   - Green
* Medium   - White
* Warning  - Yellow
* Critical - Red

Each category can be color coded, with the defaults as indicated above. 

You define where the Medium, Warning, and Critical values start for both Health and HealthPercentage.
For instance, if you set PlayerStatHealthPercentageMediumValue to 70, then the player health percentage
is considered to be NORMAL at anything above 70. 

You can define the HealthWarningValue to be 20 so that anything above 20 is considered Medium (assuming
it's below the MediumValue threshold).

The most "critical" level wins the color coding fight. In other words, if a player's health percentage is 100 (normal
in the example above) but their health is 21 (Medium Range), then their health is considered to be MEDIUM and will
appear in WHITE. 

PlayerStatUpdateInterval;
PlayerStatHealthPercentageWarningValue;
PlayerStatHealthPercentageCriticalValue;
PlayerStatHealthPercentageMediumValue;
PlayerStatHealthWarningValue;
PlayerStatHealthCriticalValue;
PlayerStatHealthMediumValue;
PlayerStatHealthNormalColor;
PlayerStatHealthMediumColor;
PlayerStatHealthWarningColor;
PlayerStatHealthCriticalColor;

:: REQUIREMENTS ::
• BepinEx - 

:: INSTALLATION ::
Place the MMWishboneTweak.dll in your \BepinEx\Plugins folder
Start Valheim Once and then exit. 
Modify the \BepinEx\Config\MofoMojo.MMWishboneTweak.cfg file to enable the mod.
Play Valheim

:: UNINSTALLATION ::
Remove the .DLL and the .CFG file from the \Plugins and \Config folders respectively.

:: FEATURES ::
• Adds beacons to Copper and Tin so that the Wishbone will find them
• Tin range is increased from default 20 to 25
• Copper range is increased from default 20 to 60 (otherwise you're literally on top of the copper almost before it pings)


::  KNOWN ISSUES ::
• None

::  CREDITS Template::
♦ https://www.youtube.com/watch?v=p_gsFASlvRw
♦ https://harmony.pardeike.net/ - Harmony Documentation
♦ https://github.com/Valheim-Modding/Wiki/wiki - Valheim modding