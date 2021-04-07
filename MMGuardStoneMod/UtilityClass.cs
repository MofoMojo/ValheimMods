using System.Text;
using UnityEngine;
using System.IO;


namespace MofoMojo
{
 public class UtilityClass
    {

        public static bool CheckKeyHeld(KeyCode value, bool req = true)
        {
            try
            {
                return Input.GetKey(value);
            }
            catch
            {
                return !req;
            }
        }

        public static bool IgnoreKeyPresses(bool extra = false)
        {
            return ZNetScene.instance == null || Player.m_localPlayer == null || Minimap.IsOpen() || Console.IsVisible() || TextInput.IsVisible() || ZNet.instance.InPasswordDialog() || Chat.instance?.HasFocus() == true || StoreGui.IsVisible() || InventoryGui.IsVisible() || Menu.IsVisible() || TextViewer.instance?.IsVisible() == true;
        }

        public static string GetHtmlColorValue(Color color)
        {
            StringBuilder sb = new StringBuilder("#");
            sb.Append(UnityEngine.ColorUtility.ToHtmlStringRGB(color));
            return sb.ToString();
        }

    }
}
