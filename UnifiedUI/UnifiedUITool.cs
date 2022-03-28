using ColossalFramework.UI;
using System;
using UnifiedUI.GUI;
using UnifiedUI.Helpers;
using UnityEngine;

namespace PublicTransportInfo
{
    public class UnifiedUITool : ToolBase
    {
        public static UnifiedUITool? Instance = null;

        private UIComponent? m_btnUnifiedUI = null;
        private UISprite? m_sprite = null;
        private UITextureAtlas? m_atlas = null;

        private static bool s_bLoadingTool = false;

        public static bool HasButtonBeenAdded()
        {
            return (Instance != null && Instance.m_btnUnifiedUI != null);
        }

        public static void AddUnifiedUITool()
        {
            if (!PublicTransportInstance.s_isGameLoaded)
            {
                Debug.Log("Game not loaded");
                return;
            }

            if (DependencyUtilities.IsUnifiedUIRunning() && PublicTransportInstance.GetSettings().AddUnifiedUIButton)
            {
                // Add UnifiedUI button if module found
                if (Instance == null)
                {
                    try
                    {
                        // UnifiedUI toolbar seems to get confused when adding a new tool and sends an OnEnable event,
                        // it then thinks my panel is open and wont open the panel the first few times you click the button.
                        // By changing the tool back after adding my tool it seems to reset itself correctly.
                        s_bLoadingTool = true;
                        ToolBase oCurrentTool = ToolsModifierControl.toolController.CurrentTool;
                        Instance = ToolsModifierControl.toolController.gameObject.AddComponent<UnifiedUITool>();
                        ToolsModifierControl.toolController.CurrentTool = oCurrentTool;
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Unified UI tool failed", e);
                    }
                    finally
                    {
                        s_bLoadingTool = false;
                    }
                }
                else if (Instance.m_btnUnifiedUI != null)
                {
                    Instance.m_btnUnifiedUI.isVisible = true;
                }
            }
        }

        public static void RemoveUnifiedUITool()
        {
            if (Instance != null)
            {
                Instance.Destroy();
            }
        }

        protected override void Awake()
        {
            Debug.Log("Awake");
            base.Awake();

            if (PublicTransportInstance.GetSettings().AddUnifiedUIButton)
            {
                AddUnifiedUIButton();
            }
        }

        public void AddUnifiedUIButton()
        {
            if (m_btnUnifiedUI == null)
            {
                try
                {
                    if (m_atlas == null)
                    {
                        m_atlas = PublicTransportInstance.LoadResources();
                    }
                    Texture2D? icon = TextureResources.LoadDllResource("BusImageInverted48x48.png", 48, 48);
                    if (icon == null)
                    {
                        Debug.Log("Failed to load icon from resources");
                        return;
                    }

                    var hotkeys = new UUIHotKeys { ActivationKey = ModSettings.Hotkey };
                    m_btnUnifiedUI = UUIHelpers.RegisterToolButton(
                        name: "TransportTool",
                        groupName: null, // default group
                        tooltip: ITransportInfoMain.Title,
                        tool: this,
                        icon: icon,
                        hotkeys: hotkeys);

                    m_sprite = m_btnUnifiedUI.AddUIComponent<UISprite>();
                    m_sprite.atlas = m_atlas;
                    m_sprite.relativePosition = default;
                    m_sprite.size = m_btnUnifiedUI.size;
                    m_sprite.spriteName = "BusImageInverted48x48";

                    // Need to tell the panel to refresh so it sizes correctly for new button
                    MainPanel.Instance.Refresh();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                    UIView.ForwardException(ex);
                }
            }
             
            if (m_btnUnifiedUI != null)
            {
                m_btnUnifiedUI.isVisible = true; 
            }
        }

        public void HideUnifiedUIButton() 
        {
            if (m_btnUnifiedUI != null)
            {
                m_btnUnifiedUI.isVisible = false;
            }
        }

        public void OnUnifiedToolbarButtonChanged()
        {
            if (PublicTransportInstance.GetSettings().AddUnifiedUIButton)
            {
                AddUnifiedUIButton();
            }
            else
            {
                HideUnifiedUIButton();
            }
        }

        public void SetIcon(string sIconName)
        {
            if (m_sprite != null && m_atlas != null)
            {
                m_sprite.spriteName = sIconName;
            }
        }

        public void ShowWarningLevel(LineIssue.IssueLevel eLevel)
        {
            if (eLevel == LineIssue.IssueLevel.ISSUE_WARNING)
            {
                SetIcon("BusImageWarning");
            }
            else if (eLevel == LineIssue.IssueLevel.ISSUE_INFORMATION)
            {
                SetIcon("BusInformationIcon");
            }
            else
            {
                SetIcon("BusImageInverted48x48");
            }
        }

        public void Enable()
        {
            if (HasButtonBeenAdded())
            {
                if (Instance != null && !Instance.enabled)
                {
                    if (m_btnUnifiedUI != null)
                    {
                        m_btnUnifiedUI.SimulateClick();
                    }
                }
            }
        }

        public void Disable()
        {
            if (HasButtonBeenAdded())
            {
                ToolBase oCurrentTool = ToolsModifierControl.toolController.CurrentTool;
                if (oCurrentTool != null && oCurrentTool == Instance && oCurrentTool.enabled)
                {
                    if (m_btnUnifiedUI != null)
                    {
                        m_btnUnifiedUI.SimulateClick();
                    }
                }
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                m_atlas = null; // Do not delete this here as it is shared.

                if (m_sprite != null)
                {
                    m_sprite.Destroy();
                    m_sprite = null;
                }
                if (m_btnUnifiedUI != null)
                {
                    m_btnUnifiedUI.Destroy();
                    m_btnUnifiedUI = null;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                UIView.ForwardException(ex);
            }
            base.OnDestroy();
        }

        protected override void OnEnable()
        {
            // We seem to get an erroneous OnEnable call when adding the UI tool.
            // Ignore it here so we dont show the panel when not requested.
            if (!s_bLoadingTool)
            {
                try
                {
                    PublicTransportInstance.ShowMainPanel();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                    UIView.ForwardException(ex);
                }
            }
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            try
            {
                PublicTransportInstance.HideMainPanel();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                UIView.ForwardException(ex);
            }
            base.OnDisable();
        }
    }
}
