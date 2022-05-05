using ColossalFramework.UI;
using System;
using UnityEngine;

namespace PublicTransportInfo 
{
    public class PublicTransportInstance : MonoBehaviour
    {
        internal static bool s_isGameLoaded = false; 
        internal static PublicTransportInfoPanel? s_mainPanel = null;
        internal static LineIssuePanel? s_LineIssuePanel = null;
        internal static MainToolbarButton? s_ToolbarButton = null;
        private static UITextureAtlas? s_atlas = null;
        private static LineIssueManager m_lineIssueManager = new LineIssueManager();
        private static Font? s_ConstantWidthFont = null;
        private static ModSettings? m_settings = null;
        
        public PublicTransportInstance() : base()
        {
            s_mainPanel = null;
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

        public static LineIssueManager GetLineIssueManager()
        {
            return m_lineIssueManager;
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
                s_LineIssuePanel = UIView.GetAView().AddUIComponent(typeof(LineIssuePanel)) as LineIssuePanel;
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

        public static void ShowMainPanel()
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
                        
                        // Keep buttons in sync
                        if (s_ToolbarButton != null)
                        {
                            s_ToolbarButton.Enable();
                        }
                        if (UnifiedUITool.Instance != null)
                        {
                            UnifiedUITool.Instance.Enable();
                        }
                        if (DependencyUtilities.IsCommuterDestinationsRunning())
                        {
                            UIPanel pnlCommuterDestination = (UIPanel) UIView.GetAView().FindUIComponent("StopDestinationInfoPanel");
                            if (pnlCommuterDestination != null)
                            {
                                pnlCommuterDestination.Hide();
                            }
                        }
                        s_mainPanel.ShowPanel();
                    }
                  
                } 
                else
                {
                    Debug.Log("m_mainPanel is null");
                }
            }
        }

        public static void HideMainPanel(bool bClearInfoPanel = true)
        {
            if (s_mainPanel != null && s_mainPanel.isVisible)
            {
                // Keep buttons in sync
                if (s_ToolbarButton != null)
                {
                    s_ToolbarButton.Disable();
                }

                if (UnifiedUITool.Instance != null)
                {
                    UnifiedUITool.Instance.Disable();
                }

                // Removing GUI
                s_mainPanel.HidePanel(bClearInfoPanel); 
            }
        }

        public static void ToggleMainPanel()
        {
            if (s_mainPanel != null)
            {
                if (s_mainPanel.isVisible)
                {
                    HideMainPanel();
                }
                else
                {
                    ShowMainPanel();
                }
            }
        }

        public static void ToggleLineIssuePanel()
        {
            if (s_LineIssuePanel != null)
            {
                if (s_LineIssuePanel.isVisible)
                {
                    HideLineIssuePanel();
                }
                else
                {
                    ShowLineIssuePanel(-1);
                }
            }
        }

        public static void UpdateMainPanel()
        {
            if (s_mainPanel != null && s_mainPanel.isVisible)
            {
                s_mainPanel.UpdateLineData();
            }
        }

        public static void ShowLineIssuePanel(int iInitialLineId)
        {
            if (s_LineIssuePanel != null)
            {
                GetLineIssueManager().UpdateLineIssues();
                s_LineIssuePanel.SetInitialLineId(iInitialLineId);
                s_LineIssuePanel.Show();
            }
        }

        public static void HideLineIssuePanel()
        {
            if (s_LineIssuePanel != null && s_LineIssuePanel.isVisible)
            {
                s_LineIssuePanel.isVisible = false;

                if (PublicTransportInstance.GetSettings().DeleteLineIssuesOnClosing)
                {
                    GetLineIssueManager().ClearIssuesWhenClosingPanel();
                }
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

                if (s_atlas != null)
                {
                    ResourceLoader.AddTexturesInAtlas(s_atlas, textures);
                }
            }

            return s_atlas;
        }
        
        public static void UpdateVehicleDetectors()
        {
            if (m_lineIssueManager != null)
            {
                m_lineIssueManager.UpdateVehicleDetectors();
            }
        }

        public static Font GetConstantWidthFont()
        {
            if (s_ConstantWidthFont == null) {
                s_ConstantWidthFont = Font.CreateDynamicFontFromOSFont("Courier New Bold", GetSettings().TooltipFontSize);
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
    }
}
