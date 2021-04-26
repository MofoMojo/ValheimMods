using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MofoMojo.MMDropItemUsingCameraDirection
{
    [BepInPlugin("MofoMojo.MMDropItemUsingCameraDirection", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.0";
        public const int nexusId = 1101;
        public const string ModName = "MMDropItemUsingCameraDirection";
        Harmony _Harmony;
        public static Plugin Instance;
        public static LoggingLevel PluginLoggingLevel = LoggingLevel.None;
        public enum LoggingLevel
        {
            None,
            Normal,
            Verbose,
            Debug
        }

        private void Awake()
        {

            Instance = this;
            Settings.Init();
            PluginLoggingLevel = Settings.PluginLoggingLevel.Value;

            _Harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

        private void OnDestroy()
        {
            if (_Harmony != null) _Harmony.UnpatchSelf();
        }

        public static void Log(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.None) Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.None) Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.None) Debug.LogError(message);
        }

        public static void LogVerbose(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.Verbose) Debug.Log(message);
        }

        public static void LogDebug(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel == LoggingLevel.Debug) Debug.Log(message);
        }

    }

    internal static class Settings
    {
        public static ConfigEntry<int> nexusId;
        public static ConfigEntry<bool> MMDropItemUsingCameraDirection;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;

        
        public static void Init()
        {
            MMDropItemUsingCameraDirection = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMDropItemUsingCameraDirection", "MMDropItemUsingCameraDirection", true, "Enables MMDropItemUsingCameraDirection mod");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");
            nexusId = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("General", "NexusID", 1101, "Nexus mod ID for updates. Do not modify.");
        }

    }

    [HarmonyPatch(typeof(Humanoid), "DropItem")]
    public class HarmonyPatch_DropItem
    {
        // Only enable if EnableFeatherMultiplier is set
        [HarmonyPrepare]
        static bool IsDropBasedOnCameraDirectionEnabled()
        {
            bool enabled = Settings.MMDropItemUsingCameraDirection.Value;
            Plugin.Log($"MMDropItemUsingCameraDirection {enabled}");

            return enabled;
        }

        [HarmonyPrefix]
        private static bool DropItem(Inventory inventory, ItemDrop.ItemData item, int amount, Humanoid __instance, ref bool __result)
        {
            // return and run the original if disabled or if this is not the player
            if (!Settings.MMDropItemUsingCameraDirection.Value || !__instance.IsPlayer()) return true;

            if (amount == 0)
            {
                __result = false;
                return false;
            }
            if (item.m_shared.m_questItem)
            {
                __instance.Message(MessageHud.MessageType.Center, "$msg_cantdrop");
                __result = false;
                return false;
            }
            if (amount > item.m_stack)
            {
                amount = item.m_stack;
            }
            __instance.RemoveFromEquipQueue(item);
            __instance.UnequipItem(item, triggerEquipEffects: false);
            if (__instance.m_hiddenLeftItem == item)
            {
                __instance.m_hiddenLeftItem = null;
                __instance.SetupVisEquipment(__instance.m_visEquipment, isRagdoll: false);
            }
            if (__instance.m_hiddenRightItem == item)
            {
                __instance.m_hiddenRightItem = null;
                __instance.SetupVisEquipment(__instance.m_visEquipment, isRagdoll: false);
            }
            if (amount == item.m_stack)
            {
                ZLog.Log((object)("drop all " + amount + "  " + item.m_stack));
                if (!inventory.RemoveItem(item))
                {
                    ZLog.Log((object)"Was not removed");
                    __result = false;
                    return false;
                }
            }
            else
            {
                ZLog.Log((object)("drop some " + amount + "  " + item.m_stack));
                inventory.RemoveItem(item, amount);
            }

            //our change to support dropping on camera direction
            ItemDrop itemDrop;
            Plugin.Log($"Drop by player... using camera rotation");
            itemDrop = ItemDrop.DropItem(item, amount, (__instance as Character).transform.position + GameCamera.instance.transform.forward + GameCamera.instance.transform.up, GameCamera.instance.transform.rotation);
            itemDrop.OnPlayerDrop();
            itemDrop.GetComponent<Rigidbody>().velocity = (GameCamera.instance.transform.forward + Vector3.up) * 5f;

            //end our change..

            __instance.m_zanim.SetTrigger("interact");
            __instance.m_dropEffects.Create((__instance as Character).transform.position, Quaternion.identity);
            __instance.Message(MessageHud.MessageType.TopLeft, "$msg_dropped " + itemDrop.m_itemData.m_shared.m_name, itemDrop.m_itemData.m_stack, itemDrop.m_itemData.GetIcon());
            __result = true;
            return false;

        }
    }
}
