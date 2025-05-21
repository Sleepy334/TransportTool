using ColossalFramework.UI;
using System;
using System.Reflection;
using UnifiedUI.GUI;
using UnityEngine;

namespace PublicTransportInfo
{
    public class MainToolbarButton
    {
        private ToggleButtonComponents? m_toggleButtonComponents = null;
        private static readonly string kEmptyContainer = "EmptyContainer";
        private static readonly string kMainToolbarSeparatorTemplate = "MainToolbarSeparator";
        private static readonly string kMainToolbarButtonTemplate = "MainToolbarButtonTemplate";
        private static readonly string kToggleButton = "TransportTool";
        
        // Instance object
        private static MainToolbarButton? s_ToolbarButton = null;


        // ----------------------------------------------------------------------------------------
        public static MainToolbarButton Instance
        {
            get
            {
                if (s_ToolbarButton == null)
                {
                    s_ToolbarButton = new MainToolbarButton();
                }

                return s_ToolbarButton;
            }
        }

        public static bool Exists
        {
            get
            {
                return s_ToolbarButton is not null;
            }
        }

        public MainToolbarButton()
        {
        }

        public void Add()
        {
            if (m_toggleButtonComponents == null)
            {
                // Adding main button
                UITabstrip? toolStrip = GetMainToolStrip();
                if (toolStrip != null)
                {
                    m_toggleButtonComponents = CreateToggleButtonComponents(toolStrip);

                    if (m_toggleButtonComponents != null)
                    {
                        toolStrip.eventSelectedIndexChanged += OnSelectedIndexChanged;
                    }
                    else
                    {
                        Debug.LogError("AddToolButton - Failed to create toolbar button.");
                    }
                }
            }
        }

        public void ShowWarningLevel(LineIssue.IssueLevel eLevel)
        {
            if (m_toggleButtonComponents != null)
            {
                if (eLevel == LineIssue.IssueLevel.ISSUE_WARNING)
                {
                    m_toggleButtonComponents.ToggleButton.normalFgSprite = "BusImageWarning";
                }    
                else if (eLevel == LineIssue.IssueLevel.ISSUE_INFORMATION)
                {
                    m_toggleButtonComponents.ToggleButton.normalFgSprite = "BusInformationIcon";
                }
                else
                {
                    m_toggleButtonComponents.ToggleButton.normalFgSprite = "BusImageInverted48x48";
                }
            }
            UpdateTooltip(eLevel);
        } 
        
        public void UpdateTooltip(LineIssue.IssueLevel eLevel)
        {
            if (m_toggleButtonComponents != null)
            {
                if (eLevel != LineIssue.IssueLevel.ISSUE_NONE && LineIssueManager.Instance != null)
                {
                    m_toggleButtonComponents.ToggleButton.tooltip = LineIssueManager.Instance.GetTooltip();
                }
                else
                {
                    m_toggleButtonComponents.ToggleButton.tooltip = ITransportInfoMain.Title;
                }
            }
        }

        public void Focus()
        {
            m_toggleButtonComponents.ToggleButton?.Focus();
        }

        public void Unfocus()
        {
            m_toggleButtonComponents.ToggleButton?.Unfocus();
        }

        public void Enable()
        {
            if (m_toggleButtonComponents != null)
            {
                UIView oView = UIView.GetAView();
                UITabstrip toolStrip = oView.FindUIComponent<UITabstrip>("MainToolstrip");
                if (toolStrip != null)
                {
                    toolStrip.selectedIndex = FindButtonIndex();
                    Focus();
                }
            }
        }

        public void Disable()
        {
            if (m_toggleButtonComponents != null)
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

        private ToggleButtonComponents CreateToggleButtonComponents(UITabstrip tabstrip)
        {
            GameObject tabStripPage = UITemplateManager.GetAsGameObject(kEmptyContainer);
            GameObject mainToolbarButtonTemplate = UITemplateManager.GetAsGameObject(kMainToolbarButtonTemplate);

            UIButton? toggleButton = tabstrip.AddTab(kToggleButton, mainToolbarButtonTemplate, tabStripPage, new Type[0]) as UIButton;
            if (toggleButton != null)
            {
                toggleButton.atlas = PublicTransportInstance.LoadResources();
                toggleButton.tooltip = ITransportInfoMain.Title;
                toggleButton.normalFgSprite = "BusImageInverted48x48";
                toggleButton.focusedBgSprite = "ToolbarIconGroup6Focused";
                toggleButton.hoveredBgSprite = "ToolbarIconGroup6Hovered";

                toggleButton.parent.height = 1f;

                IncrementObjectIndex();
            }
            else
            {
                Debug.Log("toggleButton is null.");
            }

            return new ToggleButtonComponents(null, tabStripPage, mainToolbarButtonTemplate, toggleButton, null);
        }

        private void DestroyToggleButtonComponents(ToggleButtonComponents toggleButtonComponents)
        {
            DecrementObjectIndex();

            UnityEngine.Object.Destroy(toggleButtonComponents.ToggleButton.gameObject);
            UnityEngine.Object.Destroy(toggleButtonComponents.MainToolbarButtonTemplate.gameObject);
            UnityEngine.Object.Destroy(toggleButtonComponents.TabStripPage.gameObject);
        }

        public void OnSelectedIndexChanged(UIComponent oComponent, int iSelectedIndex)
        {
            if (m_toggleButtonComponents != null && m_toggleButtonComponents.ToggleButton.isVisible && IsToolbarButton(iSelectedIndex))
            {
                MainPanel.Instance.Show();
            }
            else if (MainPanel.IsVisible()) 
            {
                MainPanel.Instance.Hide();
            }
        }

        public void Show()
        {
            if (m_toggleButtonComponents == null)
            {
                Add();
            }
            else
            {
                m_toggleButtonComponents.ToggleButton.isVisible = true;
            }
        }

        public void Hide()
        {
            if (m_toggleButtonComponents != null)
            {
                m_toggleButtonComponents.ToggleButton.isVisible = false;

                // Seem to need to destroy tab as well
                //Destroy();
            }
        }

        private void IncrementObjectIndex()
        {
            FieldInfo m_ObjectIndex = typeof(MainToolbar).GetField("m_ObjectIndex", BindingFlags.Instance | BindingFlags.NonPublic);
            m_ObjectIndex.SetValue(ToolsModifierControl.mainToolbar, (int)m_ObjectIndex.GetValue(ToolsModifierControl.mainToolbar) + 1);
        }

        private void DecrementObjectIndex()
        {
            FieldInfo m_ObjectIndex = typeof(MainToolbar).GetField("m_ObjectIndex", BindingFlags.Instance | BindingFlags.NonPublic);
            m_ObjectIndex.SetValue(ToolsModifierControl.mainToolbar, (int)m_ObjectIndex.GetValue(ToolsModifierControl.mainToolbar) - 1);
        }

        private UITabstrip? GetMainToolStrip()
        {
            if (ToolsModifierControl.mainToolbar is not null)
            {
                return ToolsModifierControl.mainToolbar.component as UITabstrip;
            }
            return null;
        }

        private bool IsToolbarButton(int iIndex)
        {
            if (iIndex != -1)
            {
                UITabstrip? toolStrip = GetMainToolStrip();
                if (toolStrip != null)
                {
                    return toolStrip.tabs[iIndex].name.Contains(kToggleButton);
                }
            }

            return false;
        }

        private int FindButtonIndex()
        {
            UITabstrip? toolStrip = GetMainToolStrip();
            if (toolStrip != null)
            {
                // Start from the end as our button will be added close to the end of the tabs
                for (int i = toolStrip.tabs.Count - 1; i >= 0; --i)
                {
                    UIComponent tab = toolStrip.tabs[i];
                    if (tab.name.Contains(kToggleButton))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public void Destroy()
        {
            // Remove event handler
            if (m_toggleButtonComponents != null)
            {
                UITabstrip? toolStrip = GetMainToolStrip();
                if (toolStrip != null)
                {
                    toolStrip.eventSelectedIndexChanged -= OnSelectedIndexChanged;
                }
                DestroyToggleButtonComponents(m_toggleButtonComponents);
                m_toggleButtonComponents = null;
            }

            s_ToolbarButton = null;
        }
    }
}
