MofoMojo's Exploration Tweaks v2.0

This is my vision for Exploration Tweaks that started out as a simple private mod for increasing the exploration radius when travelling on a ship. 
I had written it because many of the the original releases only checked if you were sailing a ship, not if you were simply standing on one. 
It then progressed into a full fledged set of tweaks to Exploration. 

Out of the box, the on foot radius for player exploration is 75, the on ship radius is 150. There's a reason for the lower numbers...
Out of the box, the time of day will have impact on how large the exploration radius is. 
	The hours between 10am and 5pm will see an improved radius of exploration
	The really early morning hours and late night hours will see a reduction in exploration radius
Out of the box, Certain types of weather will have an impact. Clear weather will see an improvement in the radius, while rain and stormy weathers will see a drop
Out of the box, Environmental/Weather effects have an additional 25% impact (positive or negative) when on a ship. This is to simulate the idea that while being on a ship
	provides a better view of your surroundings, weather can have a more profound impact.
Out of the box, running, sneaking and swimming will have a negative impact on your awareness of your surroundings. While sneaking and swimming are unlikely to really have much overall impact to "exploration" as a mechanic,
	it's added just for sake of "completeness". The idea here being that if you're running, sneaking, swimming, then you're unlikely to be able to "pay attention" and take notice of things farther away from you.
	If you want to "explore" farther, you need to slow down and take a look at your surroundings. 

:: FEATURES ::
• This is a simple QOL mod from my personal tweaks collection for Map Exploration
• Individual exploration radius for being on foot, or on boat. (Configurable)
• Exploration Radius influenced by weather. Thunderstorms and Snowstorms can have significant impact on exploration distance (configurable)
• Exploration Radius influenced by time of day. Early morning and late night the radius will decrease  (configurable)
• Exploration Radius influenced by Running, Swimming, and Sneaking  (configurable)
• Enable ability to have the map wiped on player death. Disabled by default (configurable)
• Enable ability to have map pins removed on player death. Disabled by default  (configurable)

:: NOTES ::
• Have you ever had mayonaise on a subway meatball sandwich? It's awesome.
• This mod will be incompatible with other Exploration tweaks that hook into UpdateExplore or call the Minimap.Explore method

:: REQUIREMENTS ::
• BepInEx

:: INSTALLATION ::
Place the MofoMojo.MMExplorationTweaks.dll in your \BepinEx\Plugins folder
Start Valheim
This mod is enabled by default. 
Play Valheim

:: UPGRADING ::
No upgrade requirements at this time

:: UNINSTALLATION ::
Remove the MofoMojo.MMExplorationTweaks.dll and MofoMojo.MMExplorationTweaks.CFG file from the \Plugins and \Config folders respectively.

::  KNOWN ISSUES ::
• None known

:: VERSIONS ::
2.0 Initial Public release

:: Sample Settings ::
## Settings file was created by plugin MMExplorationTweaks v2.0
## Plugin GUID: MofoMojo.MMExplorationTweaks

[ActionImpact]

## Sets the impact percentage to the exploration radius when running, positive numbers increase radius, negative numbers decrease it
# Setting type: Single
# Default value: -0.5
Running = -0.5

## Sets the impact percentage to the exploration radius when swimming, positive numbers increase radius, negative numbers decrease it
# Setting type: Single
# Default value: -0.75
Swimming = -0.75

## Sets the impact percentage to the exploration radius when sneaking, positive numbers increase radius, negative numbers decrease it
# Setting type: Single
# Default value: -0.33
Sneaking = -0.33

[EnvironmentalImpact]

## When there is an environmental impact, it incurs additional penalties when on a ship
# Setting type: Single
# Default value: -0.25
OnShipEnvironmentalPenalty = -0.25

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: 0
Default = 0

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: 0.1
Clear = 0.1

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: 0.1
Twilight_Clear = 0.1

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: -0.33
Misty = -0.33

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: 0
Darklands_dark = 0

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: 0
Heath clear = 0

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: -0.25
DeepForest Mist = -0.25

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: 0
GDKing = 0

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: -0.33
Rain = -0.33

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: -0.2
LightRain = -0.2

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: -0.5
ThunderStorm = -0.5

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: -0.2
Eikthyr = -0.2

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: -0.2
GoblinKing = -0.2

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: 0
nofogts = 0

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: -0.33
SwampRain = -0.33

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: -0.33
Bonemass = -0.33

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: -0.2
Snow = -0.2

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: -0.2
Twilight_Snow = -0.2

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: -0.5
Twilight_SnowStorm = -0.5

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: -0.5
SnowStorm = -0.5

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: -0.3
Moder = -0.3

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: -0.3
Ashrain = -0.3

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: -0.3
Crypt = -0.3

## Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease
# Setting type: Single
# Default value: -0.3
SunkenCrypt = -0.3

Heath_clear = 0

DeepForest_Mist = -0.25

[ExplorationRadius]

## Sets the exploration radius when on foot
# Setting type: Single
# Default value: 75
ExploreOnFootRadius = 75

## Sets the exploration radius when on a boat
# Setting type: Single
# Default value: 150
ExploreOnShipRadius = 150

## After penalties or bonuses are applied, this clamps the maximum exploration radius to this value
# Setting type: Single
# Default value: 300
MaximumExplorationRadius = 300

## After penalties or bonuses are applied, this clamps the minimum exploration radius to this value (minimum value is 0)
# Setting type: Single
# Default value: 5
MinimumExplorationRadius = 5

[LoggingLevel]

## Supported values are None, Normal, Verbose
# Setting type: LoggingLevel
# Default value: None
# Acceptable values: None, Normal, Verbose
PluginLoggingLevel = Verbose

[Plugin]

## Enable Exploration Radius Tweaks
# Setting type: Boolean
# Default value: true
ExploreMapTweaksEnabled = true

## Enable Exploration Radius Tweaks based on weather
# Setting type: Boolean
# Default value: true
ExploreWeatherTweaksEnabled = true

## Enable Exploration Radius Tweaks based on time
# Setting type: Boolean
# Default value: true
ExploreTimeTweaksEnabled = true

## Resets the player map on death
# Setting type: Boolean
# Default value: false
ResetMapOnDeath = false

## Removes all pins on player map on death
# Setting type: Boolean
# Default value: false
RemovePinsOnDeath = false

[TimeImpact]

## Percentage Impact the hour has on exploration radius. Hour 0 - 23. There must be 24 values separated by comma
# Setting type: String
# Default value: -0.50,-0.45,-0.40,-0.35,-0.30,-0.25,-0.1,0,0,0,0.10,0.15,0.20,0.25,0.25,0.2,0.15,-0.10,-0.15,-0.20,-0.25,-0.30,-0.40,-0.50
Hours = -0.50,-0.45,-0.40,-0.35,-0.30,-0.25,-0.1,0,0,0,0.10,0.15,0.20,0.25,0.25,0.2,0.15,-0.10,-0.15,-0.20,-0.25,-0.30,-0.40,-0.50

