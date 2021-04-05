using BepInEx;
using HarmonyLib;
using UnityEngine;
using System;

namespace MMPlayerMiniMapMod
{
    class MMPlayerMiniMap
    {
        private bool PressedToggleKey()
        {
            try
            {
                return Input.GetKeyDown(Settings.HideMapToggleKey.Value.ToLower());
            }
            catch
            {
                return false;
            }
        }

        class MMShowPlayersInRange : BaseUnityPlugin
        {
            [HarmonyPatch(typeof(Minimap), "Awake")]
            static class Minimap_Awake_Patch
            {

                [HarmonyPrepare]
                static bool IsHideMapToggleEnabled()
                {
                    bool enabled = Settings.HideMapToggleEnabled.Value;
                    Plugin.Log($"HideMapToggleEnabled: {enabled}");

                    return enabled;
                }

                [HarmonyPostfix]
                static void Postfix(Minimap __instance)
                {
                    try
                    {

                        if (!Settings.HideMapToggleEnabled.Value)
                            return;

                        // set state to last state saved in config file
                        if (!Settings.IsMapVisible.Value)
                        {
                            __instance.m_smallRoot.SetActive(false);
                            //__instance.SetMapMode(Minimap.MapMode.None);
                        }

                    }
                    catch (Exception ex)
                    {
                        // do nothing, just swallow it up
                        Plugin.LogError(ex.ToString());
                    }
                }



            }

            [HarmonyPatch(typeof(Minimap), "SetMapMode")]
            static class Minimap_SetMapMode
            {

                [HarmonyPrepare]
                static bool IsHideMapToggleEnabled()
                {
                    bool enabled = Settings.HideMapToggleEnabled.Value;
                    Plugin.Log($"HideMapToggleEnabled: {enabled}");

                    return enabled;
                }

                [HarmonyPostfix]
                static void Postfix(Minimap __instance)
                {
                    try
                    {
                        // if it's not supposed to be visible then re-hide the small minimap after switching mapmodes
                        if (__instance.m_mode == Minimap.MapMode.Small && !Settings.IsMapVisible.Value)
                        {
                            __instance.m_smallRoot.SetActive(false);
                            //__instance.SetMapMode(Minimap.MapMode.None);
                        }

                    }
                    catch (Exception ex)
                    {
                        // do nothing, just swallow it up
                        Plugin.LogError(ex.ToString());
                    }
                }



            }

            [HarmonyPatch(typeof(Minimap), "Awake")]
            static class Minimap_UpdatePlayerMarker
            {

                [HarmonyPrepare]
                static bool IsHideMapToggleEnabled()
                {
                    bool enabled = Settings.AdjustMapMarkerScale.Value;
                    Plugin.Log($"HideMapToggleEnabled: {enabled}");

                    return enabled;
                }

                [HarmonyPostfix]
                static void PostFix(Minimap __instance)
                {
                    try
                    {
                        Vector3 newSmallMarkerLocalScale = __instance.transform.localScale * Settings.SmallMarkerLocalScale.Value;
                        Vector3 newLargeMarkerLocalScale = __instance.transform.localScale * Settings.LargeMarkerLocalScale.Value;
                        __instance.m_smallMarker.localScale = newSmallMarkerLocalScale;
                        __instance.m_largeMarker.localScale = newLargeMarkerLocalScale;

                    }
                    catch (Exception ex)
                    {
                        // do nothing, just swallow it up
                        Plugin.LogError(ex.ToString());
                    }
                }



            }

        }
    }
}
