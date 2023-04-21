using ColossalFramework.UI;
using System;
using UnityEngine;

namespace PublicTransportInfo 
{
    public class PublicTransportInstance : MonoBehaviour
    { 
        internal static MainPanel? s_mainPanel = null;
        internal static LineIssuePanel? s_LineIssuePanel2 = null;
        internal static MainToolbarButton? s_ToolbarButton = null;
        private static UITextureAtlas? s_atlas = null;
        private static Font? s_ConstantWidthFont = null;
        
        public PublicTransportInstance() : base()
        {
            s_mainPanel = null;
        }

        public void Start()
        {
        }

        public static void Create() 
        {
            if (PublicTransportLoader.isGameLoaded)
            {
                s_mainPanel = UIView.GetAView().AddUIComponent(typeof(MainPanel)) as MainPanel;

                if (ModSettings.GetSettings().MainToolbarButton)
                {
                    ShowToolbarButton();
                }

                // Add UnifiedUI button if module found
                if (ModSettings.GetSettings().AddUnifiedUIButton)
                {
                    UnifiedUITool.AddUnifiedUITool();
                }
            }
        }

        public static void ShowMainPanel()
        {
            if (PublicTransportLoader.isGameLoaded)
            {
                if (s_mainPanel == null)
                {
                    // Creating GUI
                    s_mainPanel = UIView.GetAView().AddUIComponent(typeof(MainPanel)) as MainPanel;
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
            CreateLineIssuePanel();
            if (s_LineIssuePanel2 != null)
            {
                if (s_LineIssuePanel2.isVisible)
                {
                    HideLineIssuePanel();
                }
                else
                {
                    ShowLineIssuePanel(-1);
                }
            }
        }

        public static void CreateLineIssuePanel()
        {
            if (s_LineIssuePanel2 == null)
            {
                s_LineIssuePanel2 = UIView.GetAView().AddUIComponent(typeof(LineIssuePanel)) as LineIssuePanel;
                if (s_LineIssuePanel2 == null)
                {
                    //Prompt.Info("Transfer Manager CE", "Error creating Stats Panel.");
                }
            }
        }

        public static void ShowLineIssuePanel(int iInitialLineId)
        {
            if (s_LineIssuePanel2 == null)
            {
                CreateLineIssuePanel();
            }
            if (s_LineIssuePanel2 != null)
            {
                s_LineIssuePanel2.Show();
            }
        }

        public static void HideLineIssuePanel()
        {
            if (s_LineIssuePanel2 != null && s_LineIssuePanel2.isVisible)
            {
                s_LineIssuePanel2.Hide();
            }
        }

        public static void UpdateLineIssuePanel()
        {
            if (s_LineIssuePanel2 != null && s_LineIssuePanel2.isVisible)
            {
                s_LineIssuePanel2.UpdatePanel();
            }   
        }

        public static void ShowToolbarButton()
        {
            if (PublicTransportLoader.isGameLoaded)
            {
                if (s_ToolbarButton == null)
                {
                    s_ToolbarButton = new MainToolbarButton();
                    s_ToolbarButton.AddToolbarButton();
                }
                else
                {
                    s_ToolbarButton.Show();
                }
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
            if (s_LineIssuePanel2 != null && s_LineIssuePanel2.isVisible)
            {
                HideLineIssuePanel();
                return true;
            }
            else if(s_mainPanel != null && s_mainPanel.isVisible)
            {
                HideMainPanel();
                return true;
            }

            return false;
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
    }
}
