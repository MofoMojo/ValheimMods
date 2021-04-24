using System.Text;
using UnityEngine;
using System.IO;


namespace MofoMojo
{
 public class UtilityClass
    {

        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
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

        private static string ConvertFloatTo2DigitHexString(float value)
        {
            string newValue = string.Empty;

            int intValue = (int)(value * 255);
            string tempString = intValue.ToString("X");
            if (tempString.Length == 1) tempString = "0" + tempString;
            return tempString;
        }

        public static Texture2D LoadTexture(string fn, bool normalMap = false)
        {
            if (!File.Exists(fn))
                return null;
            string ext = Path.GetExtension(fn).ToLower();
            if (ext == ".png" || ext == ".jpg")
            {
                Texture2D t2d = new Texture2D(1, 1);
                t2d.LoadImage(File.ReadAllBytes(fn));
                if (normalMap)
                    SetNormalMap(ref t2d);
                return t2d;
            }
            {
                Debug.Log("texture not supported : " + fn);
            }
            return null;
        }

        public static void SetNormalMap(ref Texture2D tex)
        {
            Color[] pixels = tex.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                Color temp = pixels[i];
                temp.r = pixels[i].g;
                temp.a = pixels[i].r;
                pixels[i] = temp;
            }
            tex.SetPixels(pixels);
        }

    }
}
