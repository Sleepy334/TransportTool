using ColossalFramework.UI;
using UnityEngine;
using SleepyCommon;
using System.Reflection;

namespace PublicTransportInfo 
{
    public class PublicTransportInstance : MonoBehaviour
    { 
        private static UITextureAtlas? s_atlas = null;
        private static Font? s_ConstantWidthFont = null;
        
        public PublicTransportInstance() : base()
        {
        }

        public void Start()
        {
        }

        public static void Create() 
        {
            if (TransportToolMod.Instance.IsLoaded)
            {
                if (ModSettings.GetSettings().MainToolbarButton)
                {
                    ShowToolbarButton();
                }

                // Add UnifiedUI button if module found
                if (ModSettings.GetSettings().AddUnifiedUIButton)
                {
                    UnifiedUIButton.Add();
                }
            }
        }

        public static void ShowToolbarButton()
        {
            if (TransportToolMod.Instance.IsLoaded)
            {
                MainToolbarButton.Instance.Show();
            }
        }

        public static void HideMainToolbarButton()
        {
            if (MainToolbarButton.Exists)
            {
                MainToolbarButton.Instance.Hide();
            }
        }
        
        public static UITextureAtlas? LoadResources()
        {
            if (s_atlas == null)
            {
                string[] spriteNames = new string[]
                {
                    "BusImageInverted48x48",
                    "BusImageWarning",
                    "BusInformationIcon",
                    "Warning_icon48x48",
                    "Information",
                    "clear"
                };

                s_atlas = ResourceLoader.CreateTextureAtlas("TransportToolAtlas", spriteNames, Assembly.GetExecutingAssembly(), "PublicTransportInfo.Resources.");
                if (s_atlas == null)
                {
                    CDebug.Log("Loading of resources failed.");
                }

                UITextureAtlas defaultAtlas = ResourceLoader.GetAtlas("Ingame");
                Texture2D[] textures = new Texture2D[]
                {
                    defaultAtlas["ToolbarIconGroup6Focused"].texture, 
                    defaultAtlas["ToolbarIconGroup6Hovered"].texture,
                    defaultAtlas["ToolbarIconGroup6Normal"].texture,
                    defaultAtlas["ToolbarIconGroup6Pressed"].texture
                };

                if (s_atlas != null)
                {
                    ResourceLoader.AddTexturesInAtlas(s_atlas, textures);
                }
            }

            return s_atlas;
        }

        public static Font GetConstantWidthFont()
        {
            if (s_ConstantWidthFont == null) {
                s_ConstantWidthFont = Font.CreateDynamicFontFromOSFont("Courier New Bold", ModSettings.GetSettings().TooltipFontSize);
            }
            return s_ConstantWidthFont;
        }

        public static void InvalidateFont()
        {
            if (s_ConstantWidthFont != null)
            {
                Destroy(s_ConstantWidthFont);
                s_ConstantWidthFont = null;
            }
        }

        public static bool HandleEscape()
        {
            
            if (LineIssuePanel.Exists && LineIssuePanel.Instance.HandleEscape())
            {
                return true;
            }
            if (MainPanel.Exists && MainPanel.Instance.HandleEscape())
            {
                return true;
            }

            return false;
        }

        public static void Destroy()
        {
            if (LineIssuePanel.Exists)
            {
                LineIssuePanel.Instance.OnDestroy();
            }

            if (MainPanel.Exists)
            {
                MainPanel.Instance.OnDestroy();
            }

            if (MainToolbarButton.Exists)
            {
                MainToolbarButton.Instance.Destroy();
            }

            UnifiedUIButton.Remove();

            if (s_ConstantWidthFont != null)
            {
                Destroy(s_ConstantWidthFont);
                s_ConstantWidthFont = null;
            }

            if (s_atlas != null)
            {
                Destroy(s_atlas);
                s_atlas = null;
            }
        }
    }
}
