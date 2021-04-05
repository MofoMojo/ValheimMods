using System;
using System.Reflection;
using BepInEx.Configuration;
using BepInEx;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace MMShowPlayerStatsInRange
{
    [BepInPlugin("MofoMojo.MMShowPlayerStatsInRange", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.0";
        public const string ModName = "Show PlayerStats In Range";
        Harmony _Harmony;
        public static bool IsDebug = false;
        public static Plugin Instance;
        public static LoggingLevel PluginLoggingLevel = LoggingLevel.None;
        public enum LoggingLevel
        {
            None,
            Normal,
            Verbose
        }


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
        private Rect tempRect;

        private void Awake()
        {

            Instance = this;
            Settings.Init();
            PluginLoggingLevel = Settings.PluginLoggingLevel.Value;

            _Harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

        private void OnDestroy()
        {
            if (_Harmony != null) _Harmony.UnpatchAll(null);
        }

        public static void Log(string message)
        {
            message = ModName + " - " + message;
            if (PluginLoggingLevel > LoggingLevel.None) Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            message = ModName + " - " + message;
            if (PluginLoggingLevel > LoggingLevel.None) Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            message = ModName + " - " + message;
            if (PluginLoggingLevel > LoggingLevel.None) Debug.LogError(message);
        }

        public static void LogVerbose(string message)
        {
            message = ModName + " - " + message;
            if (PluginLoggingLevel == LoggingLevel.Verbose) Debug.Log(message);
        }


        private void Update()
        {
            // adjust this to display or hide the stats
            //             if (!modEnabled.Value || AedenthornUtils.IgnoreKeyPresses() || toggleClockKeyOnPress.Value || !PressedToggleKey()) return;
            if (!Settings.ShowPlayerStatsInRange.Value || MofoMojo.UtilityClass.IgnoreKeyPresses() || !PressedToggleKey()) return;
            Log("Toggled ShowPlayerStatsInRange");
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
                //MofoMojoMod.Plugin.Instance.Config.Save();
                MMShowPlayerStatsInRange.Plugin.Instance.Config.Save();
            }

            if (shownTime >= Settings.PlayerStatUpdateInterval.Value)
            {
                shownTime = 0;
            }

        }


        private void WindowBuilder(int id)
        {
            statsRect = GUILayoutUtility.GetRect(new GUIContent(newStatsString), style);

            GUI.DragWindow(statsRect);
            GUI.Label(statsRect, newStatsString, style);
        }

        private static void ApplyConfig()
        {
            Plugin.Log("ApplyConfig");
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
                        Plugin.Log($"Player: {nearbyPlayer.GetPlayerName()}, HealthLevel: {healthLevel}, HealthPercentageLevel: {healthPercentageLevel}, MonitorLevel: {monitorLevel}");

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

                        string htmlstyleColor = MofoMojo.UtilityClass.GetHtmlColorValue(playerStatus.styleColor);
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

    internal static class Settings
    {

        public static ConfigEntry<bool> ShowPlayerStatsInRange;
        public static ConfigEntry<float> ShowPlayerStatsRadius;
        public static ConfigEntry<string> PlayerStatLocation;
        public static ConfigEntry<string> PlayerStatFontName;
        public static ConfigEntry<int> PlayerStatFontSize;
        public static ConfigEntry<bool> showingPlayerStats;
        public static ConfigEntry<string> PlayerStatToggleKey;
        public static ConfigEntry<float> PlayerStatUpdateInterval;
        public static ConfigEntry<float> PlayerStatHealthPercentageWarningValue;
        public static ConfigEntry<float> PlayerStatHealthPercentageCriticalValue;
        public static ConfigEntry<float> PlayerStatHealthPercentageMediumValue;
        public static ConfigEntry<float> PlayerStatHealthWarningValue;
        public static ConfigEntry<float> PlayerStatHealthCriticalValue;
        public static ConfigEntry<float> PlayerStatHealthMediumValue;
        public static ConfigEntry<Color> PlayerStatHealthNormalColor;
        public static ConfigEntry<Color> PlayerStatHealthMediumColor;
        public static ConfigEntry<Color> PlayerStatHealthWarningColor;
        public static ConfigEntry<Color> PlayerStatHealthCriticalColor;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;

        // These are the settings that will be saved in the ..\plugins\mofomojo.cfg file
        public static void Init()
        {
            ShowPlayerStatsInRange = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("PlayerStats", "ShowPlayerStatsInRange", true, "When enabled, shows Player Stats in UI in range. Criticality wins. I.e. if your health percentage is 100, but your health is 25 then health level is medium");
            ShowPlayerStatsRadius = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("PlayerStats", "ShowPlayerStatsRadius", 64f, "Sets the radius for finding players");
            PlayerStatLocation = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<string>("PlayerStats", "PlayerStatLocation", "50%,3%", "Location on the screen to show the player stats (x,y) or (x%,y%). Also, Use mouse cursor to change position in game");
            PlayerStatFontName = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<string>("PlayerStats", "PlayerStatFontName", "Norsebold", "Name of the font to use, possible values Norsebold, AveriaSerifLibre-Bold");
            PlayerStatFontSize = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("PlayerStats", "PlayerStatFontSize", 24, "Size of the font to use");
            PlayerStatToggleKey = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<string>("PlayerStats", "PlayerStatToggleKey", "[", "Key to use for toggling player stats. https://docs.unity3d.com/ScriptReference/KeyCode.html");
            PlayerStatHealthPercentageMediumValue = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("PlayerStats", "PlayerStatHealthPercentageMediumValue", 75f, "HealthPercentage to be considered medium. Anything higher is normal");
            PlayerStatHealthPercentageWarningValue = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("PlayerStats", "PlayerStatHealthPercentageWarningValue", 50f, "HealthPercentage to be considered warning. Anything higher is medium");
            PlayerStatHealthPercentageCriticalValue = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("PlayerStats", "PlayerStatHealthPercentageCriticalValue", 25f, "HealthPercentage to be considered critical.Anything higher is warning");
            PlayerStatHealthMediumValue = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("PlayerStats", "PlayerStatHealthMediumValue", 30f, "Health value to be considered medium. Anything higher is normal");
            PlayerStatHealthWarningValue = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("PlayerStats", "PlayerStatHealthWarningValue", 20f, "Health value to be considered warning . Anything higher is medium");
            PlayerStatHealthCriticalValue = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("PlayerStats", "PlayerStatHealthCriticalValue", 10f, "Health value to be considered critical . Anything higher is warning");

            PlayerStatHealthNormalColor = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Color>("PlayerStats", "PlayerStatHealthNormalColor", Color.green, "Sets the color for playerstatus at Normal Level, https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html. Supported color names: aqua,black,blue,brown,cyan,darkblue,fuchsia,green,grey,lightblu,lime,magenta,maroon,navy,olive,orange,purple,red,silver,teal,white,yellow");
            PlayerStatHealthMediumColor = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Color>("PlayerStats", "PlayerStatHealthMediumColor", Color.white, "Sets the color for playerstatus at medium Level, can also use color hex tags, like FFFFFF, FF0000, etc,.");
            PlayerStatHealthWarningColor = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Color>("PlayerStats", "PlayerStatHealthWarningColor", Color.yellow, "Sets the color for playerstatus at warning Level");
            PlayerStatHealthCriticalColor = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Color>("PlayerStats", "PlayerStatHealthCriticalColor", new Color(1, 0, 0), "Sets the color for playerstatus at critical Level");
            PlayerStatUpdateInterval = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("PlayerStats", "PlayerStatUpdateInterval", 30f, "How often to update. This value represents frames, so if you're getting 30 FPS and the value is 30, you'll update every second.");
            showingPlayerStats = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("PlayerStats", "showingPlayerStats", true, "Used by mod, don't modify");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");
        }

    }
}
