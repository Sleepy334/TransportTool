using ColossalFramework.UI;
using System;
using UnifiedUI.GUI;
using UnifiedUI.Helpers;
using UnityEngine;

namespace PublicTransportInfo.UnifiedUI
{
    public class UnifiedUITool : ToolBase
    {
        public static UnifiedUITool Instance = null;
        private static UIComponent btnUnifiedUI = null;

        private static bool s_bHasBeenCheckedRunning = false;
        private static bool s_bIsUnifiedUIRunning = false;
        private static bool s_bLoadingTool = false;
        public static bool HasButtonBeenAdded()
        {
            return (btnUnifiedUI != null);
        }

        public static bool IsUnifiedUIRunning()
        {
            // Check if Unified UI is actually running
            if (!s_bHasBeenCheckedRunning)
            {
                s_bIsUnifiedUIRunning = DependencyUtilities.IsPluginRunning("2255219025");
                s_bHasBeenCheckedRunning = true;
            }
            

            return s_bIsUnifiedUIRunning;
    }

        public static void AddUnifiedUITool()
        {
            if (!PublicTransportInstance.s_isGameLoaded)
            {
                Debug.Log("AddUnifiedUI::Game not loaded");
                return;
            }

            if (IsUnifiedUIRunning() && ModSettings.s_bAddUnifiedUIButton)
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
                        Debug.Log("Unified UI tool failed");
                        Debug.LogException(e);
                    }
                    finally
                    {
                        s_bLoadingTool = false;
                    }
                }
                else if (btnUnifiedUI != null)
                {
                    btnUnifiedUI.isVisible = true;
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

            if (ModSettings.s_bAddUnifiedUIButton)
            {
                AddUnifiedUIButton();
            }
        }

        public void AddUnifiedUIButton()
        {
            Debug.Log("AddToolbarButton - Enter");
            if (btnUnifiedUI == null)
            {
                try
                {
                    Texture2D icon = TextureResources.LoadDllResource("BusImageInverted48x48.png", 48, 48);
                    if (icon == null)
                    {
                        Debug.Log("Failed to load icon from resources");
                        return;
                    }
                    var hotkeys = new UUIHotKeys { ActivationKey = ModSettings.Hotkey };

                    btnUnifiedUI = UUIHelpers.RegisterToolButton(
                        name: "TransportTool",
                        groupName: null, // default group
                        tooltip: ITransportInfoMain.Title,
                        tool: this,
                        icon: icon,
                        hotkeys: hotkeys);

                    // Need to tell the panel to refresh so it sizes correctly for new button
                    MainPanel.Instance.Refresh();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    UIView.ForwardException(ex);
                }
            }
             
            if (btnUnifiedUI != null)
            {
                btnUnifiedUI.isVisible = true; 
            }
            Debug.Log("AddToolbarButton - Leave");
        }

        public void HideUnifiedUIButton() 
        {
            if (btnUnifiedUI != null)
            {
                btnUnifiedUI.isVisible = false;
            }
        }

        public void OnUnifiedToolbarButtonChanged()
        {
            if (ModSettings.s_bAddUnifiedUIButton)
            {
                AddUnifiedUIButton();
            }
            else
            {
                HideUnifiedUIButton();
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                btnUnifiedUI.Destroy();
                btnUnifiedUI = null;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
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
                    Debug.Log("OnEnable");
                    PublicTransportInstance.ShowPanel();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    UIView.ForwardException(ex);
                }
            }
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            try
            {
                Debug.Log("OnDisable");
                PublicTransportInstance.HidePanel();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                UIView.ForwardException(ex);
            }
            base.OnDisable();
        }
    }
}
