using ColossalFramework.UI;
using System;
using UnityEngine;

namespace PublicTransportInfo 
{
    public class PublicTransportInstance : MonoBehaviour
    {
        internal static PublicTransportInfoPanel? s_mainPanel = null;
        internal static bool s_isGameLoaded = false;

        internal static MainToolbarButton s_ToolbarButton = null;
        internal static UITextureAtlas s_atlas = null;
        internal static VehicleProgressManager m_vehicleProgress = null;

        private static ModSettings m_settings = null;
        
        public PublicTransportInstance() : base()
        {
            s_mainPanel = null;
            m_vehicleProgress = new VehicleProgressManager();
        }

        public static ModSettings GetSettings()
        {
            if (m_settings == null)
            {
                m_settings = ModSettings.Load();
                Debug.Log("Settings loaded." + m_settings.AddUnifiedUIButton + " " + m_settings.BoredThreshold);
            }
            return m_settings;
        }

        public void Start()
        {
            try
            {
                s_isGameLoaded = true;
            }
            catch (Exception e)
            {
                Debug.Log("UI initialization failed.", e);
                GameObject.Destroy(gameObject);
            }
        }

        public static void Create() 
        {
            if (s_isGameLoaded)
            {
                s_mainPanel = UIView.GetAView().AddUIComponent(typeof(PublicTransportInfoPanel)) as PublicTransportInfoPanel;
            }

            if (GetSettings().MainToolbarButton)
            {
                ShowToolbarButton();
            }

            // Add UnifiedUI button if module found
            if (PublicTransportInstance.GetSettings().AddUnifiedUIButton)
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
            if (s_ToolbarButton != null)
            {
                s_ToolbarButton.Destroy();
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
                        if (s_ToolbarButton != null)
                        {
                            s_ToolbarButton.Focus();
                        }
                    }
                  
                } 
                else
                {
                    Debug.Log("m_mainPanel is null");
                }
            }
        }

        public static void HidePanel(bool bClearInfoPanel = true)
        {
            if (s_mainPanel != null && s_mainPanel.isVisible)
            {
                if (s_ToolbarButton != null)
                {
                    UIView oView = UIView.GetAView();
                    UITabstrip toolStrip = oView.FindUIComponent<UITabstrip>("MainToolstrip");
                    toolStrip.closeButton.SimulateClick();
                    s_ToolbarButton.Unfocus();
                }

                // Removing GUI
                s_mainPanel.HidePanel(bClearInfoPanel); 
            }
        }

        public static void TogglePanel()
        {
            if (s_mainPanel != null)
            {
                if (s_mainPanel.isVisible)
                {
                    HidePanel();
                }
                else
                {
                    ShowPanel();
                }
            }
        }

        public static void UpdatePanel()
        {
            if  (s_mainPanel != null && s_mainPanel.isVisible)
            {
                s_mainPanel.UpdateLineData();
            }
        }

        public static void ShowToolbarButton()
        {
            Debug.Log("AddToolbarButton");
            if (s_ToolbarButton == null)
            {
                if (!s_isGameLoaded || s_mainPanel == null)
                {
                    return;
                }
                s_ToolbarButton = new MainToolbarButton();
                s_ToolbarButton.AddToolbarButton();
            } else
            {
                s_ToolbarButton.Show();
            }
        }

        public static void HideMainToolbarButton()
        {
            if (s_ToolbarButton != null)
            {
                s_ToolbarButton.Hide();
            }
        }
        
        public static UITextureAtlas LoadResources()
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
                };

                s_atlas = ResourceLoader.CreateTextureAtlas("TransportToolAtlas", spriteNames, "PublicTransportInfo.Resources.");
                if (s_atlas == null)
                {
                    PublicTransportInfo.Debug.Log("Loading of resources failed.");
                }

                UITextureAtlas defaultAtlas = ResourceLoader.GetAtlas("Ingame");
                Texture2D[] textures = new Texture2D[]
                {
                    defaultAtlas["ToolbarIconGroup6Focused"].texture, 
                    defaultAtlas["ToolbarIconGroup6Hovered"].texture,
                    defaultAtlas["ToolbarIconGroup6Normal"].texture,
                    defaultAtlas["ToolbarIconGroup6Pressed"].texture
                };

                ResourceLoader.AddTexturesInAtlas(s_atlas, textures);
            }

            return s_atlas;
        }
        
        public static void UpdateVehicleProgress()
        {
            if (m_vehicleProgress != null)
            {
                m_vehicleProgress.UpdateVehicleProgress();
            }
        }
    }
}
