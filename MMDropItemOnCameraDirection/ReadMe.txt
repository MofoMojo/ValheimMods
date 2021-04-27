MofoMojo's Drop Item Using Camera Direction v1.0

Have you ever dropped an item over the edge of a cliff/roof/precipice into a swamp/abyss/ocean/lava flows of morder because
that was the direction your character was facing as opposed to dropping it in the direction the camera was facing? Well, be
prepared to untrain yourself as this mod alters the Humanoid DropItem method to check if the player is dropping an item
and if so, drops the item in the direction the camera is facing, NOT your character.

:: FEATURES ::
• This is a simple QOL mod from my personal tweaks collection that drops items based on your camera facing, and not your character facing. 

:: NOTES ::
• Have you ever had mayonaise on a subway meatball sandwich? It's awesome.
• This mod alters the _Humanoid_ class' DropItem method whenever the Humanoid _IS_ the player, otherwise it runs the original. 
• It will likely be incompatible with any other mod that alters this method specifically
• This mod does NOT alter the ItemDrop class' DropItem method. 
• The mod only affects dropping items from inventory. It does not affect spawning items via consoles/other mods
• Anyone that wants to add this into their own mod to provide this capability, it's a pretty easy change. 
• Here's my code that I modified. The GameCamera.transform is used instead of the base.transform. 

            //our change to support dropping on camera direction
            ItemDrop itemDrop;
            Plugin.Log($"Drop by player... using camera rotation");
            itemDrop = ItemDrop.DropItem(item, amount, (__instance as Character).transform.position + GameCamera.instance.transform.forward + GameCamera.instance.transform.up, GameCamera.instance.transform.rotation);
            itemDrop.OnPlayerDrop();
            itemDrop.GetComponent<Rigidbody>().velocity = (GameCamera.instance.transform.forward + Vector3.up) * 5f;

:: REQUIREMENTS ::
• BepInEx - 

:: INSTALLATION ::
Place the MofoMojo.MMRandomSpawnPoint.dll in your \BepinEx\Plugins folder
Start Valheim and hold on to your pants
This mod is enabled by default. Modify the .cfg to disable it if you want.
Play Valheim

:: UPGRADING ::
This is 1.0... there are no upgrades here.

:: UNINSTALLATION ::
Remove the MofoMojo.MMDropItemUsingCameraDirection.dll and MofoMojo.MMDropItemUsingCameraDirection.CFG file from the \Plugins and \Config folders respectively.

::  KNOWN ISSUES ::
• None known

:: VERSIONS ::
1.0 Initial Release

:: Sample Settings ::
## Settings file was created by plugin MMDropItemUsingCameraDirection v1.0
## Plugin GUID: MofoMojo.MMDropItemUsingCameraDirection

[LoggingLevel]

## Supported values are None, Normal, Verbose
# Setting type: LoggingLevel
# Default value: None
# Acceptable values: None, Normal, Verbose, Debug
PluginLoggingLevel = None

[MMDropItemUsingCameraDirection]

## Enables MMDropItemUsingCameraDirection mod
# Setting type: Boolean
# Default value: true
MMDropItemUsingCameraDirection = true

