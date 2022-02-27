using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using PublicTransportInfo.UnifiedUI;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PublicTransportInfo 
{
    public class PublicTransportInstance : MonoBehaviour
    {
        internal static PublicTransportInfoPanel? s_mainPanel = null;
        internal static bool s_isGameLoaded = false;
        internal static UIButton s_mToolbarButton;
        internal static int s_ToolbarIndex;
        public static UITextureAtlas atlas = null;

        public PublicTransportInstance() : base()
        {
            s_mainPanel = null;
        }

        public void Start()
        {
            try
            {
                s_isGameLoaded = true;
                ModSettings.Load();

            }
            catch (Exception e)
            {
                Debug.Log("UI initialization failed.");
                Debug.LogException(e);
                GameObject.Destroy(gameObject);
            }
        }

        public static void Create() 
        {
            if (s_isGameLoaded)
            {
                s_mainPanel = UIView.GetAView().AddUIComponent(typeof(PublicTransportInfoPanel)) as PublicTransportInfoPanel;
            }

            if (ModSettings.s_bAddMainToolbarButton)
            {
                AddToolbarButton();
            }

            // Add UnifiedUI button if module found
            if (ModSettings.s_bAddUnifiedUIButton)
            {
                UnifiedUITool.AddUnifiedUITool();
            }
            
        }

        public static void Destroy()
        {
            if (s_mainPanel != null)
            {
                GameObject.Destroy(s_mainPanel.gameObject);
            }
            UnifiedUITool.RemoveUnifiedUITool();
        }

        public static void ShowPanel()
        {
            if (s_isGameLoaded)
            {
                if (s_mainPanel == null)
                {
                    // Creating GUI
                    s_mainPanel = UIView.GetAView().AddUIComponent(typeof(PublicTransportInfoPanel)) as PublicTransportInfoPanel;
                }

                if (s_mainPanel != null)
                {

                    if (!s_mainPanel.isVisible)
                    {
                        s_mainPanel.ShowPanel();
                        if (s_mToolbarButton != null)
                        {
                            s_mToolbarButton.Focus();
                        }
                    }
                  
                } 
                else
                {
                    Debug.Log("PublicTransportInstance::ShowPanel - m_mainPanel is null");
                }
            }
        }

        public static void HidePanel(bool bClearInfoPanel = true)
        {
            if (s_mainPanel != null && s_mainPanel.isVisible)
            {
                if (s_mToolbarButton != null)
                {
                    UIView oView = UIView.GetAView();
                    UITabstrip toolStrip = oView.FindUIComponent<UITabstrip>("MainToolstrip");
                    toolStrip.closeButton.SimulateClick();
                    s_mToolbarButton.Unfocus();
                }

                // Removing GUI
                s_mainPanel.HidePanel(bClearInfoPanel); 
            }
        }

        public static void UpdatePanel()
        {
            if  (s_mainPanel != null && s_mainPanel.isVisible)
            {
                s_mainPanel.UpdateLineData();
            }
        }

        public static void AddToolbarButton()
        {
            Debug.Log("AddToolbarButton");
            if (!s_isGameLoaded || s_mainPanel == null)
            {
                return;
            }

            if (s_mToolbarButton == null)  
            {
                // Adding main button
                UIView oView = UIView.GetAView(); 
                UITabstrip toolStrip = oView.FindUIComponent<UITabstrip>("MainToolstrip"); 

                // Add a handler to hide panel when toolbar changes
                void OnSelectedIndexChanged(UIComponent oComponent, int iSelectedIndex)
                {
                    if (iSelectedIndex == s_ToolbarIndex)
                    {
                        ShowPanel();
                    } else
                    {
                        HidePanel();
                    }
                }
                toolStrip.eventSelectedIndexChanged += OnSelectedIndexChanged;

                // Add main toolbar button.
                s_ToolbarIndex = toolStrip.tabCount;
                Debug.Log("AddToolbarButton::s_ToolbarIndex: " + s_ToolbarIndex);
                s_mToolbarButton = toolStrip.AddUIComponent<UIButton>();

                // Load icon.
                if (atlas == null)
                {
                    atlas = LoadResources();
                }
                if (atlas != null)
                {
                    s_mToolbarButton.atlas = atlas;
                    s_mToolbarButton.normalBgSprite = "BusImageInverted48x48";// "IconPolicyFreePublicTransport";
                } else
                {
                    // Use old icon if load fails.
                    s_mToolbarButton.normalBgSprite = "IconPolicyFreePublicTransport";
                }
                s_mToolbarButton.focusedFgSprite = "ToolbarIconGroup6Focused";
                s_mToolbarButton.hoveredFgSprite = "ToolbarIconGroup6Hovered";
                s_mToolbarButton.size = new Vector2(43f, 47f);
                s_mToolbarButton.name = ITransportInfoMain.ModName;
                s_mToolbarButton.tooltip = ITransportInfoMain.Title;
                s_mToolbarButton.relativePosition = new Vector3(0, 5);
                toolStrip.AddTab("Transport Tool", s_mToolbarButton.gameObject, null, null);

                FieldInfo m_ObjectIndex = typeof(MainToolbar).GetField("m_ObjectIndex", BindingFlags.Instance | BindingFlags.NonPublic);
                m_ObjectIndex.SetValue(ToolsModifierControl.mainToolbar, (int)m_ObjectIndex.GetValue(ToolsModifierControl.mainToolbar) + 1);

                Locale locale = (Locale)typeof(LocaleManager).GetField("m_Locale", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(LocaleManager.instance);
                Locale.Key key = new Locale.Key
                {
                    m_Identifier = "TUTORIAL_ADVISER_TITLE",
                    m_Key = s_mToolbarButton.name
                };
                if (!locale.Exists(key))
                {
                    locale.AddLocalizedString(key, s_mToolbarButton.name);
                }
                key = new Locale.Key
                {
                    m_Identifier = "TUTORIAL_ADVISER",
                    m_Key = s_mToolbarButton.name
                };
                if (!locale.Exists(key))
                {
                    locale.AddLocalizedString(key, "");
                }

                oView.FindUIComponent<UITabContainer>("TSContainer").AddUIComponent<UIPanel>().color = new Color32(0, 0, 0, 0);
            }
        }

        public static void HideMainToolbarButton()
        {
            if (s_mToolbarButton != null)
            {
                // Adding main button
                UIView oView = UIView.GetAView();
                UITabstrip toolStrip = oView.FindUIComponent<UITabstrip>("MainToolstrip");
                if (toolStrip != null)
                {
                    //toolStrip.tabs[s_ToolbarIndex].Hide();
                    toolStrip.tabs.RemoveAt(s_ToolbarIndex);
                    Destroy(s_mToolbarButton);
                    s_mToolbarButton = null;
                }
            }
        }

        public static UITextureAtlas LoadResources()
        {
            if (atlas == null)
            {
                Debug.Log("PublicTransportInstance.LoadResources");
                string[] spriteNames = new string[]
                {
                    "BusImageInverted48x48",
                };

                atlas = ResourceLoader.CreateTextureAtlas("TransportToolAtlas", spriteNames, "PublicTransportInfo.Resources.");
                if (atlas == null)
                {
                    Debug.Log("Loading of resources failed.");
                }

                UITextureAtlas defaultAtlas = ResourceLoader.GetAtlas("Ingame");
                Texture2D[] textures = new Texture2D[]
                {
                    defaultAtlas["ToolbarIconGroup6Focused"].texture, 
                    defaultAtlas["ToolbarIconGroup6Hovered"].texture,
                    defaultAtlas["ToolbarIconGroup6Normal"].texture,
                    defaultAtlas["ToolbarIconGroup6Pressed"].texture
                };

                ResourceLoader.AddTexturesInAtlas(atlas, textures);
            }

            return atlas;
        }
    }
}
