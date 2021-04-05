using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace MMShowPlayerStatsInRange
{
    class MMShowPlayerStatsInRange
    {
        [BepInPlugin("MMShowPlayersInRange", "MMShowPlayersInRange", "1.0.2")]
        class MMShowPlayersInRange : BaseUnityPlugin
        {
            private static readonly bool isDebug = true;

            //PlayerStats structure. Eventually figure out how to populate and use this structure to just do the display
            struct PlayerStats
            {
                public Color styleColor;
                public GUIStyle style;
                public long PlayerId;
                public string Name;
                public float Health;
                public float MaxHealth;
                public float HealthPercentage;
                public PlayerStatHealthLevel HealthLevel;
                public PlayerStatHealthLevel HealthPercentageLevel;
                public PlayerStatHealthLevel MonitorLevel;
                public bool WarnStamina;
                public bool WarnHealth;
                public List<StatusEffect> statusEffects;
            }

            enum PlayerStatHealthLevel
            {
                Normal,
                Medium,
                Warning,
                Critical
            }

            // dictionary for storing and looping through player stats 
            private static Dictionary<long, PlayerStats> playersStats = new Dictionary<long, PlayerStats>();

            private static int windowId = 1194374;
            private static Font playerStatFont;
            private static GUIStyle style;
            private static bool configApplied = false;
            private static Vector2 playerStatLocation;
            private static float shownTime = 0;
            private static Rect windowRect;
            private static string newStatsString;
            private static Rect statsRect;


            private void Update()
            {
                // adjust this to display or hide the stats
                //             if (!modEnabled.Value || AedenthornUtils.IgnoreKeyPresses() || toggleClockKeyOnPress.Value || !PressedToggleKey()) return;
                if (!Settings.ShowPlayerStatsInRange.Value || UtilityClass.IgnoreKeyPresses() || !PressedToggleKey()) return;

                bool show = Settings.showingPlayerStats.Value;
                Settings.showingPlayerStats.Value = !show;
                Plugin.Instance.Config.Save();
            }

            private void OnGUI()
            {
                if (Settings.ShowPlayerStatsInRange.Value && configApplied && Player.m_localPlayer && Hud.instance)
                {

                    float alpha = 1f;

                    if (shownTime == 0)
                    {
                        newStatsString = UpdatePlayerStats();
                    }

                    shownTime += 1;


                    //style.normal.textColor = Color.white;
                    //style.active.textColor = Color.white;
                    //style.hover.textColor = Color.white;

                    if ((Settings.showingPlayerStats.Value) && Traverse.Create(Hud.instance).Method("IsVisible").GetValue<bool>())
                    {
                        //enable this for continued work on guilayout and better formatting, etc,.
                        //tempTest();
                        GUI.backgroundColor = Color.clear;
                        windowRect = GUILayout.Window(windowId, new Rect(windowRect.position, statsRect.size), new GUI.WindowFunction(WindowBuilder), "");
                        //Dbgl(""+windowRect.size);
                    }

                }
                else
                {
                    shownTime = 0;
                }

                if (!Input.GetKey(KeyCode.Mouse0) && (windowRect.x != playerStatLocation.x || windowRect.y != playerStatLocation.y))
                {
                    playerStatLocation = new Vector2(windowRect.x, windowRect.y);
                    Settings.PlayerStatLocation.Value = $"{windowRect.x},{windowRect.y}";
                    Config.Save();
                }

                if (shownTime >= Settings.PlayerStatUpdateInterval.Value)
                {
                    shownTime = 0;
                }

            }


            private Rect tempRect;

            private void tempTest()
            {
                tempRect = new Rect(windowRect.position.x, windowRect.position.y, 640, 480);
                GUILayout.BeginArea(tempRect);
                GUILayout.BeginVertical();

                foreach (PlayerStats playerStatus in playersStats.Values)
                {
                    GUIStyle rightStyle = playerStatus.style;
                    rightStyle.alignment = TextAnchor.MiddleRight;
                    rightStyle.normal.textColor = playerStatus.styleColor;

                    GUILayout.BeginHorizontal(GUILayout.Width(250));

                    // Name Column
                    GUILayout.Label(playerStatus.Name, rightStyle, GUILayout.Width(100));

                    // health column
                    GUILayout.Label($"{playerStatus.Health.ToString("0.00")}/{playerStatus.MaxHealth.ToString("0.00")}", rightStyle, GUILayout.Width(100));

                    // health percentage column
                    GUILayout.Label($"{playerStatus.HealthPercentage.ToString("00")}%", rightStyle, GUILayout.Width(50));


                    // status effects
                    /*
                    if(Settings.ShowPlayerStatuses.Value)
                    { 
                        if (playerStatus.statusEffects.Count != 0)
                        {
                            GUIStyle effectsStyle = playerStatus.style;
                            effectsStyle.alignment = TextAnchor.MiddleRight;
                            effectsStyle.normal.textColor = Color.white;

                            string tempText = String.Empty;

                            foreach (StatusEffect statusEffect in playerStatus.statusEffects)
                            {
                                string text = Localization.instance.Localize(statusEffect.m_name);
                                if (!String.IsNullOrEmpty(text))
                                {
                                    tempText += $"| {text} |";
                                }

                            }

                            if (!tempText.IsNullOrWhiteSpace())
                            {
                                GUILayout.Label(tempText, effectsStyle);
                            }

                        }
                    }
                    */
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
                GUILayout.EndArea();
            }

            private void WindowBuilder(int id)
            {
                statsRect = GUILayoutUtility.GetRect(new GUIContent(newStatsString), style);

                GUI.DragWindow(statsRect);
                GUI.Label(statsRect, newStatsString, style);
            }

            // https://forum.unity.com/threads/fancylabel-multicolor-and-multifont-label.9549/
            // https://forum.unity.com/threads/guilayout-iteration.114152/
            private void WindowBuilder2(int id)
            {
                //statsRect = GUILayoutUtility.GetRect(300, 300);
                statsRect = GUILayoutUtility.GetRect(new GUIContent(newStatsString), style);
                GUILayout.BeginArea(statsRect);
                GUILayout.BeginVertical(style);

                foreach (PlayerStats playerStatus in playersStats.Values)
                {
                    GUILayout.BeginVertical(style);
                    Plugin.Log($"Player Info: {playerStatus.Name}");
                    GUILayout.Space(10);

                    GUILayout.Label($"{playerStatus.Name} - {playerStatus.Health.ToString("0")} / {playerStatus.MaxHealth.ToString("0")} - {playerStatus.HealthPercentage * 100}%");
                    GUILayout.Space(10);
                    GUILayout.EndVertical();
                }

                GUILayout.EndVertical();
                GUILayout.EndArea();
                GUI.DragWindow(statsRect);

            }

            private static void ApplyConfig()
            {
                Plugin.Log("MMShowPlayersInRange - ApplyConfig");
                newStatsString = UpdatePlayerStats();

                string[] split = Settings.PlayerStatLocation.Value.Split(',');
                playerStatLocation = new Vector2(split[0].Trim().EndsWith("%") ? (float.Parse(split[0].Trim().Substring(0, split[0].Trim().Length - 1)) / 100f) * Screen.width : float.Parse(split[0].Trim()), split[1].Trim().EndsWith("%") ? (float.Parse(split[1].Trim().Substring(0, split[1].Trim().Length - 1)) / 100f) * Screen.height : float.Parse(split[1].Trim()));

                //windowRect = new Rect(playerStatLocation, new Vector2(1000, 100));
                windowRect = new Rect(playerStatLocation, new Vector2(1000, 480));

                Plugin.Log($"getting fonts");
                Font[] fonts = Resources.FindObjectsOfTypeAll<Font>();
                foreach (Font font in fonts)
                {
                    if (font.name == Settings.PlayerStatFontName.Value)
                    {
                        playerStatFont = font;
                        Plugin.Log($"got font {font.name}");
                        break;
                    }
                }

                style = new GUIStyle
                {
                    richText = true,
                    fontSize = Settings.PlayerStatFontSize.Value,
                    alignment = TextAnchor.MiddleLeft,
                    font = playerStatFont
                };

                configApplied = true;
            }

            private static string UpdatePlayerStats()
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                if (!Settings.showingPlayerStats.Value) return sb.ToString();

                try
                {


                    Player player = Player.m_localPlayer;

                    if (null != player)
                    {
                        List<Player> playersInRange = new List<Player>();
                        Player.GetPlayersInRange(player.transform.position, Settings.ShowPlayerStatsRadius.Value, playersInRange);


                        // sort players by name
                        playersInRange.Sort((x, y) => x.GetPlayerName().CompareTo(y.GetPlayerName()));

                        playersStats.Clear();

                        //Plugin.Log("Name, health/maxhealth - percentage");

                        foreach (Player nearbyPlayer in playersInRange)
                        {
                            long playerId = nearbyPlayer.GetPlayerID();

                            Color tempColor = Color.white;
                            PlayerStatHealthLevel healthLevel = PlayerStatHealthLevel.Normal;
                            PlayerStatHealthLevel healthPercentageLevel = PlayerStatHealthLevel.Normal;
                            PlayerStatHealthLevel monitorLevel = PlayerStatHealthLevel.Normal;

                            // set healthPercentageLevel based on health percentage
                            float healthPercentage = nearbyPlayer.GetHealthPercentage() * 100;
                            if (healthPercentage > Settings.PlayerStatHealthPercentageMediumValue.Value)
                            {
                                // health is normal
                                healthPercentageLevel = PlayerStatHealthLevel.Normal;
                            }
                            else if (healthPercentage > Settings.PlayerStatHealthPercentageWarningValue.Value)
                            {
                                // health is medium
                                healthPercentageLevel = PlayerStatHealthLevel.Medium;
                            }
                            else if (healthPercentage > Settings.PlayerStatHealthPercentageCriticalValue.Value)
                            {
                                // health is warning level
                                healthPercentageLevel = PlayerStatHealthLevel.Warning;
                            }
                            else
                            {
                                // health is critical
                                healthPercentageLevel = PlayerStatHealthLevel.Critical;
                            }

                            // set health level
                            float health = nearbyPlayer.GetHealth();
                            if (health > Settings.PlayerStatHealthMediumValue.Value)
                            {
                                healthLevel = PlayerStatHealthLevel.Normal;
                            }
                            else if (health > Settings.PlayerStatHealthWarningValue.Value)
                            {
                                healthLevel = PlayerStatHealthLevel.Medium;
                            }
                            else if (health > Settings.PlayerStatHealthCriticalValue.Value)
                            {
                                healthLevel = PlayerStatHealthLevel.Warning;
                            }
                            else
                            {
                                healthLevel = PlayerStatHealthLevel.Critical;
                            }

                            // set monitor level based on most critical
                            monitorLevel = (healthLevel == healthPercentageLevel) ? healthLevel : (healthLevel > healthPercentageLevel ? healthLevel : healthPercentageLevel);
                            //Plugin.Log($"HealthLevel: {healthLevel} HealthPercentageLevel: {healthPercentageLevel} MonitorLevel: {monitorLevel}");

                            // do color logic here
                            switch (monitorLevel)
                            {
                                case PlayerStatHealthLevel.Normal:
                                    tempColor = Settings.PlayerStatHealthNormalColor.Value;
                                    break;
                                case PlayerStatHealthLevel.Medium:
                                    tempColor = Settings.PlayerStatHealthMediumColor.Value;
                                    break;
                                case PlayerStatHealthLevel.Warning:
                                    tempColor = Settings.PlayerStatHealthWarningColor.Value;
                                    break;
                                case PlayerStatHealthLevel.Critical:
                                    tempColor = Settings.PlayerStatHealthCriticalColor.Value;
                                    break;
                            }

                            GUIStyle tempStyle = new GUIStyle
                            {
                                richText = true,
                                fontSize = Settings.PlayerStatFontSize.Value,
                                alignment = TextAnchor.MiddleLeft,
                                font = playerStatFont
                            };

                            tempStyle.normal.textColor = tempColor;

                            PlayerStats playerStat = new PlayerStats()
                            {
                                PlayerId = playerId,
                                // set style to yellow if < 50 %, Red if < 25
                                // set style to red if health is < 25
                                // red/yellow/white
                                HealthLevel = healthLevel,
                                HealthPercentageLevel = healthPercentageLevel,
                                MonitorLevel = monitorLevel,
                                styleColor = tempColor,
                                style = tempStyle,
                                Name = nearbyPlayer.GetPlayerName(),
                                Health = nearbyPlayer.GetHealth(),
                                MaxHealth = nearbyPlayer.GetMaxHealth(),
                                HealthPercentage = healthPercentage,
                                WarnStamina = false,
                                statusEffects = (nearbyPlayer as Character).m_seman.GetStatusEffects()
                            };

                            playersStats.Add(playerId, playerStat);
                        }


                        foreach (PlayerStats playerStatus in playersStats.Values)
                        {
                            // Plugin.Log($"Player Info: {playerStatus.Name}");
                            // https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html

                            string htmlstyleColor = UtilityClass.GetHtmlColorValue(playerStatus.styleColor);
                            sb.Append($"<color={htmlstyleColor}>{playerStatus.Name}:  {playerStatus.Health.ToString("0.00")}/{playerStatus.MaxHealth.ToString("0.00")} - {playerStatus.HealthPercentage}% </color>");

                            // only show the statuses if enabled. 
                            /*
                            if (Settings.ShowPlayerStatuses.Value && playerStatus.statusEffects.Count != 0)
                            {
                                sb.Append("<color=white>");

                                foreach (StatusEffect statusEffect in playerStatus.statusEffects)
                                {
                                    string text = Localization.instance.Localize(statusEffect.m_name);
                                    if (!String.IsNullOrEmpty(text))
                                    {
                                        sb.Append($"| {text} |");
                                    }

                                }

                                sb.AppendLine("</color>");
                            }
                            else
                            {
                                sb.AppendLine();
                            }
                            */
                            sb.AppendLine();
                            // Plugin.Log($"{sb.ToString()}");
                        }



                    }
                }
                catch (Exception ex)
                {
                    //do nothing
                    Plugin.LogError($"StringBuilder Exception {ex.Message}");
                }

                return sb.ToString();
            }


            private static bool CheckKeyHeld(string value)
            {
                try
                {
                    return Input.GetKey(value.ToLower());
                }
                catch
                {
                    return true;
                }
            }
            private bool PressedToggleKey()
            {
                try
                {
                    return Input.GetKeyDown(Settings.PlayerStatToggleKey.Value.ToLower());
                }
                catch
                {
                    return false;
                }
            }


            [HarmonyPatch(typeof(ZNetScene), "Awake")]
            static class ZNetScene_Awake_Patch
            {

                [HarmonyPrepare]
                static bool IsShowPlayerStatsInRangeEnabled()
                {
                    bool enabled = Settings.ShowPlayerStatsInRange.Value;
                    Plugin.Log($"ShowPlayerStatsInRange: {enabled}");

                    return enabled;
                }

                [HarmonyPostfix]
                static void Postfix()
                {
                    if (!Settings.ShowPlayerStatsInRange.Value)
                        return;

                    ApplyConfig();

                }
            }



        }
    }
}
