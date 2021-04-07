using System;
using System.Text;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEditor;

namespace MofoMojo.MMGuardStoneMod
{
    class MMGuardWardTweaks
    {
        // this hooks into PrivateArea type's Awake method
        [HarmonyPatch(typeof(PrivateArea), "Awake")]
        public class HarmonyPatch_Awake
        {
            [HarmonyPrepare]
            static bool IsGuardStoneTweakEnabled()
            {
                bool enabled = Settings.MMGuardStoneModEnabled.Value;
                Plugin.Log($"EnableGuardStoneTweak {enabled}");

                return enabled;
            }

            // this is a prefix so it will overwrite the m_radius of the instance before Awake is called
            [HarmonyPrefix]
            // Prefix indicates that your code will execute before the actual code
            private static void Prefix(PrivateArea __instance)
            {
                Plugin.Log($"GuardStone/PrivateArea Radius: {__instance.m_radius}");
                __instance.m_radius = Settings.GuardStoneRadius.Value;
                FixupRadii(__instance, Settings.GuardStoneRadius.Value);

                Plugin.Log($"Modified GuardStone/PrivateArea Radius: {__instance.m_radius}");

            }

            // this is a postfix to Awake method which checks to see if the instance is already enabled and if so, add's the no monster area... area.
            [HarmonyPostfix]
            private static void PostFix(PrivateArea __instance)
            {
                SetNoMonsterArea(__instance, __instance.IsEnabled());
            }

            private static void FixupRadii(PrivateArea pa, float radius)
            {
                Plugin.Log($"FixupRaddi called - {radius}");
                foreach (SphereCollider sphere in pa.GetComponents<SphereCollider>())
                {
                    Plugin.Log("sphere adjusted");
                    sphere.radius = radius;
                }
            }

        }

        [HarmonyPatch(typeof(PrivateArea), "SetEnabled")]
        public class HarmonyPatch_SetEnabled
        {

            [HarmonyPrepare]
            static bool IsGuardStoneTweakEnabled()
            {
                bool enabled = Settings.MMGuardStoneModEnabled.Value;
                Plugin.Log($"EnableGuardStoneTweak {enabled}");

                return enabled;
            }

            // this is a postfix so it will enable the proper EventArea when the ward is activated and remove it when deactivated
            [HarmonyPostfix]
            private static void SetEnabled_PostFix(PrivateArea __instance, bool enabled)
            {
                Plugin.Log($"GuardStone SetEnabled Called with {enabled}");

                try
                {
                    SetNoMonsterArea(__instance, enabled);

                }
                catch (Exception ex)
                {
                    Plugin.LogError($"Exception GuardStoneTweakEnabled: {ex.Message}");
                }
            }
        }

        private static void SetNoMonsterArea(PrivateArea pa, bool enabled)
        {
            Plugin.Log($"SetNoMonsterArea called: {enabled}");

            EffectArea NoMonsters = GetNoMonsterArea(pa);

            if (null != NoMonsters)
            {
                Plugin.Log($"SetNoMonsterArea Got NoMonsterArea");

                // NoMonsters keeps monsters from "attempting" to navigate inside
                // Playerbase ensures that creatures don't spawn within the radius
                // however you can't build when it's enabled. 
                NoMonsters.m_type = enabled ? (EffectArea.Type.NoMonsters | EffectArea.Type.PlayerBase) : EffectArea.Type.None;


                Plugin.Log($"SetEnabled - type set to {NoMonsters.m_type }");

                NoMonsters.enabled = enabled;

                SphereCollider collider = NoMonsters.GetComponent<SphereCollider>();
                if (null != collider)
                {
                    collider.enabled = enabled;
                    collider.radius = pa.m_radius;
                }

            }

        }

        private static EffectArea GetNoMonsterArea(PrivateArea pa)
        {
            Plugin.Log("GetNoMonsterArea called");
            EffectArea NoMonstersEffectArea = null;
            try
            {
                EffectArea[] effectAreas = pa.GetComponents<EffectArea>();
                foreach (EffectArea effectArea in effectAreas)
                {
                    if (effectArea.name == Plugin.NoMonsterEffectAreaName)
                    {
                        Plugin.Log("GetNoMonsterArea found NoMonsterArea");
                        NoMonstersEffectArea = effectArea;
                        break;
                    }
                }

                // if we didn't find one, make one
                if (null == NoMonstersEffectArea)
                {
                    Plugin.Log("GetNoMonsterArea Creating NoMonsterArea");
                    NoMonstersEffectArea = pa.gameObject.AddComponent<EffectArea>();

                    NoMonstersEffectArea.name = Plugin.NoMonsterEffectAreaName;

                    // initialize this with no affect at first...
                    NoMonstersEffectArea.m_type = EffectArea.Type.None;

                    // you need a sphere collider attached to the EffectArea to define the radius
                    SphereCollider sphereCollider = NoMonstersEffectArea.gameObject.AddComponent<SphereCollider>();

                    // set this off by default if we're making it
                    sphereCollider.enabled = false;

                    // make sure IsTrigger is set so physics don't apply. 
                    sphereCollider.isTrigger = true;

                    // set the radius to the radius of the private area
                    Plugin.Log("GetNoMonsterArea - adjusting radius of SphereCollider");
                    sphereCollider.radius = pa.m_radius;
                    /*
                    try { 
                        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                        Transform transform = sphere.GetComponent<Transform>();
                        transform = sphereCollider.transform;
                        
                        Material mats = Resources.Load("ForceField.mat", typeof(Material)) as Material;
                        if (null != mats) Plugin.Log("Loaded Forcefield Material");

                        Renderer render = sphere.GetComponent<Renderer>();
                        render.material = mats;


                    }
                    catch(Exception ex)
                    {
                        Plugin.LogError($"Problem loading material/mesh {ex}");
                    }
                    */

                }


            }
            catch (Exception ex)
            {
                Plugin.LogError($"AddNoMonsterArea - {ex.Message}");
            }

            return NoMonstersEffectArea;

        }

        //no need for this but will keep around in case code changes and requires it
        private static void RemoveNoMonsterArea(PrivateArea pa)
        {
            Plugin.Log("RemoveNoMonsterArea");
            EffectArea[] effectAreas = pa.GetComponents<EffectArea>();
            EffectArea noMonstersArea = null;
            foreach (EffectArea effectArea in effectAreas)
            {
                if (effectArea.name == Plugin.NoMonsterEffectAreaName)
                {
                    Plugin.Log("RemoveNoMonsterArea - Found effect area");
                    noMonstersArea = effectArea;
                    break;
                }
            }


            if (null != noMonstersArea)
            {
                // destroy the colliders
                SphereCollider[] colliders = noMonstersArea.GetComponents<SphereCollider>();
                for (int i = 0; i < colliders.Length; i++)
                {
                    Plugin.Log("RemoveNoMonsterArea - Destroying SphereCollider");
                    GameObject.Destroy(colliders[i]);
                }

                // destroy the object
                Plugin.Log("RemoveNoMonsterArea - Destroying EffectArea");
                GameObject.Destroy(noMonstersArea);
            }




        }

        [HarmonyPatch(typeof(PrivateArea), "Interact")]
        public class HarmonyPatch_Interact
        {
            [HarmonyPrepare]
            static bool IsGuardStoneTweakEnabled()
            {
                bool enabled = Settings.MMGuardStoneModEnabled.Value;
                Plugin.Log($"EnableGuardStoneTweak {enabled}");

                return enabled;
            }

            [HarmonyPrefix]
            public static bool Interact(Humanoid human, bool hold, PrivateArea __instance, ref bool __result)
            {
                // default to a false just in case.
                __result = false;

                Plugin.Log("Patched Interact");

                // if not allowing permitted players to activate/deactivate, just run the standard function
                if (Settings.WardInteractBehavior.Value == Plugin.WardBehaviorType.OwnerOnly) return true;

                if (hold)
                {
                    Plugin.Log("HOLDING");
                    __result = false;

                    //don't execute.. what's the point?
                    return false;
                }
                Player player = human as Player;

                // If player is creator
                // or if ward mode is ownerandpermitted and is permitted or behavior is all AND modifier key is not being held....
                if (__instance.m_piece.IsCreator()
                    ||
                    ((Settings.WardInteractBehavior.Value == Plugin.WardBehaviorType.OwnerAndPermitted && __instance.IsPermitted(player.GetPlayerID())  ||  (Settings.WardInteractBehavior.Value == Plugin.WardBehaviorType.All))
                    && !UtilityClass.CheckKeyHeld(Settings.InteractModifier.Value))
                    )
                {
                    __instance.m_nview.InvokeRPC("ToggleEnabled", player.GetPlayerID());
                    __result = true;

                    // don't execute the original code
                    return false;
                }
                Plugin.Log("Wasn't normal activation");

                // if OwnerOnly then don't allow anyone to do anything else
                if (Settings.WardInteractBehavior.Value == Plugin.WardBehaviorType.OwnerOnly)
                {
                    __result = false;
                    return false;
                }

                // if the player is using the modifier key and gets this far, then toggle permitted
                // can only do this when the peice isn't active
                if (UtilityClass.CheckKeyHeld(Settings.InteractModifier.Value) && !__instance.IsEnabled())
                {
                    __instance.m_nview.InvokeRPC("TogglePermitted", player.GetPlayerID(), player.GetPlayerName());
                    __result = true;

                    // don't execute original code
                    return false;
                }

                __result = false;
                return false;
            }
        }

        [HarmonyPatch(typeof(PrivateArea), "RPC_ToggleEnabled")]
        public class HarmonyPatch_RPC_ToggleEnabled
        {
            [HarmonyPrepare]
            static bool IsGuardStoneTweakEnabled()
            {
                bool enabled = Settings.MMGuardStoneModEnabled.Value;
                Plugin.Log($"EnableGuardStoneTweak {enabled}");

                return enabled;
            }

            // this patch allows permitted players to toggle the ward on and off
            [HarmonyPrefix]
            public static bool RPC_ToggleEnabled(long uid, long playerID, PrivateArea __instance)
            {
                ZLog.Log((object)("Toggle enabled from " + playerID + "  creator is " + __instance.m_piece.GetCreator()));
                if (__instance.m_nview.IsOwner() && ((__instance.IsPermitted(playerID) && Settings.WardInteractBehavior.Value == Plugin.WardBehaviorType.OwnerAndPermitted) || (Settings.WardInteractBehavior.Value == Plugin.WardBehaviorType.All)))
                {
                    __instance.SetEnabled(!__instance.IsEnabled());
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(PrivateArea), "GetHoverText")]
        public class HarmonyPatch_GetHoverText
        {
            [HarmonyPrepare]
            static bool IsGuardStoneTweakEnabled()
            {
                bool enabled = Settings.MMGuardStoneModEnabled.Value;
                Plugin.Log($"EnableGuardStoneTweak {enabled}");

                return enabled;
            }

            // this patch allows permitted players to toggle the ward on and off
            [HarmonyPrefix]
            public static bool GetHoverText(PrivateArea __instance, ref string __result)
            {
                Plugin.LogVerbose("Executing Patched GetHoverText");
                if (!__instance.m_nview.IsValid())
                {
                    __result = "";
                    return false;
                }
                if (Player.m_localPlayer == null)
                {
                    __result = "";
                    return false;
                }
                __instance.ShowAreaMarker();
                StringBuilder stringBuilder = new StringBuilder(256);

                // Player or owner hovering over so display activate/dectivate options
                if (__instance.m_piece.IsCreator() || (Settings.WardInteractBehavior.Value == Plugin.WardBehaviorType.OwnerAndPermitted && __instance.IsPermitted(Player.m_localPlayer.GetPlayerID())) || Settings.WardInteractBehavior.Value == Plugin.WardBehaviorType.All)
                {
                    if (__instance.IsEnabled())
                    {
                        stringBuilder.Append(__instance.m_name + " ( $piece_guardstone_active )");
                        stringBuilder.Append("\n$piece_guardstone_owner:" + __instance.GetCreatorName());
                        stringBuilder.Append("\n[<color=yellow><b>$KEY_Use</b></color>] $piece_guardstone_deactivate");
                    }
                    else
                    {
                        stringBuilder.Append(__instance.m_name + " ($piece_guardstone_inactive )");
                        stringBuilder.Append("\n$piece_guardstone_owner:" + __instance.GetCreatorName());
                        stringBuilder.Append("\n[<color=yellow><b>$KEY_Use</b></color>] $piece_guardstone_activate");
                        if (Settings.WardInteractBehavior.Value != Plugin.WardBehaviorType.OwnerOnly)
                        {
                            if(__instance.IsPermitted(Player.m_localPlayer.GetPlayerID()))
                            {
                                stringBuilder.Append("\n[<color=yellow><b>" + Settings.InteractModifier.Value.ToString() + " - $KEY_Use</b></color>] $piece_guardstone_remove");
                            }
                            else
                            {
                                stringBuilder.Append("\n[<color=yellow><b>" + Settings.InteractModifier.Value.ToString() + " - $KEY_Use</b></color>] $piece_guardstone_add");
                            }
                        }

                    }
                }
                else if (__instance.IsEnabled())
                {
                    // Player is not permitted and is not the owner and ward is active... do nothing
                    stringBuilder.Append(__instance.m_name + " ( $piece_guardstone_active )");
                    stringBuilder.Append("\n$piece_guardstone_owner:" + __instance.GetCreatorName());
                    if (Settings.WardInteractBehavior.Value != Plugin.WardBehaviorType.OwnerOnly) stringBuilder.Append("\n[<color=yellow><b>" + Settings.InteractModifier.Value.ToString() + " - $KEY_Use</b></color>] $piece_guardstone_remove");
                }
                else 
                {
                    // player is not permitted and is not the owner, but ward is inactive.... show modifier
                    stringBuilder.Append(__instance.m_name + " ( $piece_guardstone_inactive )");
                    stringBuilder.Append("\n$piece_guardstone_owner:" + __instance.GetCreatorName());
                    if (Settings.WardInteractBehavior.Value != Plugin.WardBehaviorType.OwnerOnly) stringBuilder.Append("\n[<color=yellow><b>" + Settings.InteractModifier.Value.ToString() + " - $KEY_Use</b></color>] $piece_guardstone_add");
                }
                __instance.AddUserList(stringBuilder);
                __result = Localization.instance.Localize(stringBuilder.ToString());
                Plugin.Log($"Result {__result}");
                return false;
            }
        }
    }
}
