using System;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace MofoMojo.MMServerMessages
{
    class MMServerMessages_Patch
    {
        [HarmonyPatch(typeof(Game), "Awake")]
        static class Game_Awake_Patch
        {


            [HarmonyPrepare]
            static bool IsMMServerMessagesEnabled()
            {
                bool enabled = Plugin.MMServerMessagesEnabled.Value;
                Plugin.Log($"MMServerMessagesEnabled: {enabled}");

                return enabled;
            }

            [HarmonyPostfix]
            static void Postfix(ref Game __instance)
            {
                try
                {
                    Plugin.gameinstance = __instance;
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
