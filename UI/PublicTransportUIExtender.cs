using ColossalFramework.UI;
using ICities;
using JetBrains.Annotations;
using System;
using UnityEngine;

namespace PublicTransportInfo
{
    public class PublicTransportUIExtender : LoadingExtensionBase
    {
        private bool m_bButtonAttached = false;
        private UIButton? m_btnTransportTool = null;
        private UISprite? m_spriteOrangeBus = null;
        private MouseEventHandler? m_btnTransportToolClick = null;
        private static UITextureAtlas? m_atlas = null;
        private const int iBUTTON_WIDTH = 30;
        private const int iBUTTON_HEIGHT = 30;
        private static bool ActiveInMode(LoadMode mode)
        {
            switch (mode)
            {
                case LoadMode.NewGame:
                case LoadMode.NewGameFromScenario:
                case LoadMode.LoadGame:
                    return true;

                default:
                    return false;
            }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (ActiveInMode(mode))
            {
                AttachUI();
            }
        }

        public override void OnLevelUnloading()
        {
            if (m_bButtonAttached)
            {
                DetachUI();
            }
        }

        private void AttachUI()
        {
            try
            {
                PublicTransportWorldInfoPanel pnlPublicTransportInfoPanel = GameObject.Find("UIView").transform.GetComponentInChildren<PublicTransportWorldInfoPanel>();
                if (pnlPublicTransportInfoPanel != null)
                {
                    UIPanel pnlUiPanel = pnlPublicTransportInfoPanel.gameObject.GetComponent<UIPanel>();
                    if (pnlUiPanel != null)
                    {
                        bool bIPT2Running = DependencyUtilities.IsPluginRunning("928128676");
                        if (bIPT2Running)
                        {
                            Debug.Log("IPT2 is running.");
                            UIComponent btnPassengerTextField = (UIComponent)pnlUiPanel.Find("Passengers"); 
                            if (btnPassengerTextField != null && btnPassengerTextField.parent != null)
                            {
                                if (btnPassengerTextField.parent.parent != null)
                                {
                                    UIPanel pnlMainParent = (UIPanel)btnPassengerTextField.parent.parent;
                                    AddOrangeBusButton(pnlMainParent, bIPT2Running);
                                }
                            }
                            else
                            {
                                PublicTransportInfo.Debug.LogError("Cannot find Passengers.");
                            }
                        }
                        else
                        {
                            // Place the orange button next to the "Lines Overview" button.
                            UIButton btnLinesOverview = (UIButton)pnlUiPanel.Find("LinesOverview");
                            if (btnLinesOverview != null)
                            {
                                UIPanel pnlButtonPanel = (UIPanel)btnLinesOverview.parent;
                                if (pnlButtonPanel != null)
                                {
                                    AddOrangeBusButton(pnlButtonPanel, bIPT2Running);
                                }
                            }
                        }
                    }
                }
            } 
            catch (Exception ex) 
            {
                Debug.Log("Unable to attach button", ex);
            }
            
        }

        private void AddOrangeBusButton(UIPanel oParent, bool bIPT2Running)
        {
            m_btnTransportTool = oParent.Find<UIButton>("btnTransportTool"); 
            if (m_btnTransportTool != null)
            {
                PublicTransportInfo.Debug.Log("m_btnTransportTool already exists.");
                oParent.RemoveUIComponent(m_btnTransportTool);
            }

            m_btnTransportTool = oParent.AddUIComponent<UIButton>();
            if (m_btnTransportTool != null)
            {
                m_btnTransportTool.name = "btnTransportTool";
                m_btnTransportTool.tooltip = "Open Transport Tool";
                m_btnTransportTool.autoSize = false;
                m_btnTransportTool.width = iBUTTON_WIDTH;
                m_btnTransportTool.height = iBUTTON_HEIGHT;
                m_btnTransportTool.normalBgSprite = "ButtonMenu";
                m_btnTransportTool.hoveredBgSprite = "ButtonMenuHovered";
                m_btnTransportTool.disabledBgSprite = "ButtonMenuDisabled";
                m_btnTransportTool.pressedBgSprite = "ButtonMenuPressed";

                if (bIPT2Running)
                {
                    // Bottom right of the main parent panel
                    m_btnTransportTool.width = iBUTTON_WIDTH;
                    m_btnTransportTool.height = iBUTTON_HEIGHT;
                    m_btnTransportTool.BringToFront(); // Bring to the right of other elements
                    m_btnTransportTool.relativePosition = new Vector2(oParent.width - iBUTTON_WIDTH, oParent.height - iBUTTON_HEIGHT);
                }
                else
                {
                    m_btnTransportTool.anchor = UIAnchorStyle.Left | UIAnchorStyle.Right | UIAnchorStyle.CenterVertical; 
                    m_btnTransportTool.SendToBack(); // Send to the left of other elements
                }
                
                // Add the orange bus sprite to the middle of the button.
                m_spriteOrangeBus = m_btnTransportTool.AddUIComponent<UISprite>();
                if (m_spriteOrangeBus != null)
                {
                    if (m_atlas == null)
                    {
                        m_atlas = PublicTransportInstance.LoadResources();
                    }
                    m_spriteOrangeBus.name = "spriteOrangeBus";
                    m_spriteOrangeBus.autoSize = false;
                    m_spriteOrangeBus.width = iBUTTON_WIDTH;
                    m_spriteOrangeBus.height = iBUTTON_HEIGHT;
                    m_spriteOrangeBus.atlas = m_atlas;
                    m_spriteOrangeBus.spriteName = "BusImageInverted48x48";
                    m_spriteOrangeBus.CenterToParent();  
                }

                oParent.ResetLayout();
                oParent.Reset();

                m_btnTransportToolClick = (x, y) =>
                {
                    Debug.Log("m_btnTransportToolClick");

                    // Hide transport info panel
                    PublicTransportWorldInfoPanel oPanel = GameObject.Find("(Library) PublicTransportWorldInfoPanel").GetComponent<PublicTransportWorldInfoPanel>();
                    if (oPanel != null)
                    {
                        oPanel.Hide();
                    }


                    // Show Transport Tool
                    PublicTransportInstance.ShowMainPanel();
                };

                m_btnTransportTool.eventClick += m_btnTransportToolClick;
                m_bButtonAttached = true;
            }
        }

        private void DetachUI()
        {
            if (m_btnTransportTool != null)
            {
                // Remove event handler
                m_btnTransportTool.eventClick -= m_btnTransportToolClick;
                m_btnTransportToolClick = null;

                // Remove button
                m_btnTransportTool.parent.RemoveUIComponent(m_btnTransportTool);
                UnityEngine.Object.Destroy(m_btnTransportTool.gameObject);
                m_btnTransportTool = null;
            }
        }
    }
}
