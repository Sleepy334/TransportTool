using ColossalFramework.Globalization;
using ColossalFramework.UI;
using System.Reflection;
using UnityEngine;

namespace PublicTransportInfo
{
    public class MainToolbarButton
    {
        private UIButton? m_ToolbarButton = null;
        private int m_ToolbarIndex = -1;
        public UITextureAtlas? m_atlas = null;

        public MainToolbarButton()
        {
        }

        public void ShowWarningLevel(LineIssue.IssueLevel eLevel)
        {
            if (m_ToolbarButton != null && m_atlas != null)
            {
                if (eLevel == LineIssue.IssueLevel.ISSUE_WARNING)
                {
                    m_ToolbarButton.normalBgSprite = "BusImageWarning";
                }    
                else if (eLevel == LineIssue.IssueLevel.ISSUE_INFORMATION)
                {
                    m_ToolbarButton.normalBgSprite = "BusInformationIcon";
                }
                else
                {
                    m_ToolbarButton.normalBgSprite = "BusImageInverted48x48";  
                }
            }
            UpdateTooltip(eLevel);
        } 
        
        public void UpdateTooltip(LineIssue.IssueLevel eLevel)
        {
            if (m_ToolbarButton != null)
            {
                if (eLevel != LineIssue.IssueLevel.ISSUE_NONE && PublicTransportInstance.GetLineIssueManager() != null)
                {
                    m_ToolbarButton.tooltip = PublicTransportInstance.GetLineIssueManager().GetTooltip();
                }
                else
                {
                    m_ToolbarButton.tooltip = ITransportInfoMain.Title;
                }
            }
        }

        public void Focus()
        {
            m_ToolbarButton?.Focus();
        }

        public void Unfocus()
        {
            m_ToolbarButton?.Unfocus();
        }

        public void Enable()
        {
            if (m_ToolbarButton != null)
            {
                UIView oView = UIView.GetAView();
                UITabstrip toolStrip = oView.FindUIComponent<UITabstrip>("MainToolstrip");
                if (toolStrip != null)
                {
                    toolStrip.selectedIndex = m_ToolbarIndex;
                    Focus();
                }
            }
        }

        public void Disable()
        {
            if (m_ToolbarButton != null)
            {
                UIView oView = UIView.GetAView();
                UITabstrip toolStrip = oView.FindUIComponent<UITabstrip>("MainToolstrip");
                if (toolStrip != null)
                {
                    toolStrip.closeButton.SimulateClick();
                    Unfocus();
                }
            }
        }

        public void AddToolbarButton()
        {
            Debug.Log("AddToolbarButton");
            if (m_ToolbarButton == null)
            {
                Debug.Log("Adding toolbar button");
                // Adding main button
                UIView oView = UIView.GetAView();
                UITabstrip toolStrip = oView.FindUIComponent<UITabstrip>("MainToolstrip");
                toolStrip.eventSelectedIndexChanged += OnSelectedIndexChanged;

                // Add main toolbar button.
                m_ToolbarIndex = toolStrip.tabCount;
                m_ToolbarButton = toolStrip.AddUIComponent<UIButton>();

                // Load icon.
                if (m_atlas == null)
                {
                    m_atlas = PublicTransportInstance.LoadResources();
                }
                if (m_atlas != null)
                {
                    m_ToolbarButton.atlas = m_atlas;
                    m_ToolbarButton.normalBgSprite = "BusImageInverted48x48";
                }
                else
                {
                    // Use old icon if load fails.
                    m_ToolbarButton.normalBgSprite = "IconPolicyFreePublicTransport";
                }
                m_ToolbarButton.focusedFgSprite = "ToolbarIconGroup6Focused";
                m_ToolbarButton.hoveredFgSprite = "ToolbarIconGroup6Hovered";
                m_ToolbarButton.size = new Vector2(43f, 47f);
                m_ToolbarButton.name = ITransportInfoMain.ModName;
                m_ToolbarButton.tooltip = ITransportInfoMain.Title;
                m_ToolbarButton.relativePosition = new Vector3(0, 5);

                toolStrip.AddTab("Transport Tool", m_ToolbarButton.gameObject, null, null);

                FieldInfo m_ObjectIndex = typeof(MainToolbar).GetField("m_ObjectIndex", BindingFlags.Instance | BindingFlags.NonPublic);
                m_ObjectIndex.SetValue(ToolsModifierControl.mainToolbar, (int)m_ObjectIndex.GetValue(ToolsModifierControl.mainToolbar) + 1);

                Locale locale = (Locale)typeof(LocaleManager).GetField("m_Locale", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(LocaleManager.instance);
                Locale.Key key = new Locale.Key
                {
                    m_Identifier = "TUTORIAL_ADVISER_TITLE",
                    m_Key = m_ToolbarButton.name
                };
                if (!locale.Exists(key))
                {
                    locale.AddLocalizedString(key, m_ToolbarButton.name);
                }
                key = new Locale.Key
                {
                    m_Identifier = "TUTORIAL_ADVISER",
                    m_Key = m_ToolbarButton.name
                };
                if (!locale.Exists(key))
                {
                    locale.AddLocalizedString(key, "");
                }

                oView.FindUIComponent<UITabContainer>("TSContainer").AddUIComponent<UIPanel>().color = new Color32(0, 0, 0, 0);
            }
        }

        public void OnSelectedIndexChanged(UIComponent oComponent, int iSelectedIndex)
        {
            if (m_ToolbarButton != null && m_ToolbarButton.isVisible && iSelectedIndex == m_ToolbarIndex)
            {
                PublicTransportInstance.ShowMainPanel();
            }
            else
            {
                PublicTransportInstance.HideMainPanel();
            }
        }

        public void Show()
        {
            if (m_ToolbarButton == null)
            {
                AddToolbarButton();
            }
            else
            {
                m_ToolbarButton.isVisible = true;
            }
        }

        public void Hide()
        {
            if (m_ToolbarButton != null)
            {
                m_ToolbarButton.isVisible = false;

                // Seem to need to destroy tab as well
                Destroy();
            }
        }

        public void Destroy()
        {
            if (m_ToolbarButton != null)
            {
                // Delete main button
                UIView oView = UIView.GetAView();
                UITabstrip toolStrip = oView.FindUIComponent<UITabstrip>("MainToolstrip");
                if (toolStrip != null)
                {
                    //toolStrip.tabs[s_ToolbarIndex].Hide();
                    toolStrip.tabs.RemoveAt(m_ToolbarIndex);
                    Object.Destroy(m_ToolbarButton);
                    m_ToolbarButton = null;
                    m_ToolbarIndex = -1;
                }
            }
            m_atlas = null; // Dont destroy this here as it is shared
        }
    }
}
