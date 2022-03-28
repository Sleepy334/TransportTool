﻿using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace PublicTransportInfo
{
    public class SettingsUI
    {
		SettingsSlider? m_oStuckSlider = null;
		SettingsSlider? m_oSlowSlider = null;
		UICheckBox? m_playSound = null;

		public SettingsUI()
        {
        }
        
        public void OnSettingsUI(UIHelper helper)
        {
			ModSettings oSettings = PublicTransportInstance.GetSettings();

			UIComponent oMainPanel = (UIComponent)helper.self;

			// Add tabstrip.
			UITabstrip tabStrip = oMainPanel.AddUIComponent<UITabstrip>();
			tabStrip.relativePosition = new Vector3(0, 0);
			tabStrip.width = oMainPanel.width;
			tabStrip.height = 40;

			// Tab container (the panels underneath each tab).
			UITabContainer tabContainer = oMainPanel.AddUIComponent<UITabContainer>();
			tabContainer.relativePosition = new Vector3(0, 40);
			tabContainer.width = oMainPanel.width;
			tabContainer.height = oMainPanel.width - tabStrip.height;
			tabStrip.tabPages = tabContainer;

			UIPanel tabGeneral = AddTab(tabStrip, "General", true);
			UIPanel tabLineIssues = AddTab(tabStrip, "Line Issues", true);

			// general tab
			UIHelper helperGeneral = new UIHelper(tabGeneral);
			SetupGeneralTab(helperGeneral);

			// Line Issues
			UIHelper helperLineIssues = new UIHelper(tabLineIssues);
			SetupLineIssuesTab(helperLineIssues);
			tabLineIssues.isVisible = false;

			tabStrip.selectedIndex = 0;
		}

		private void SetupGeneralTab(UIHelper helperGeneral)
        {
			ModSettings oSettings = PublicTransportInstance.GetSettings();

			// UI Buttons
			UIHelperBase oButtonGroup = helperGeneral.AddGroup("Toolbar Buttons");
			oButtonGroup.AddCheckbox("Add button to main toolbar", oSettings.MainToolbarButton, OnToolbarButtonChanged);
			oButtonGroup.AddCheckbox("Add button to UnifiedUI toolbar", oSettings.AddUnifiedUIButton, OnUnifiedToolbarButtonChanged);

			// Keyboard shortcut
			UIHelper group = (UIHelper)helperGeneral.AddGroup("Keyboard Shortcut");
			UIPanel panel = (UIPanel)group.self;
			UIKeymappingsPanel keymappings = panel.gameObject.AddComponent<UIKeymappingsPanel>();
			keymappings.AddKeymapping("Activation Shortcut", ModSettings.Hotkey); // Automatically saved

			// Bored Slider
			UIHelper oBoredGroup = (UIHelper)helperGeneral.AddGroup("Bored Threshold");
			SettingsSlider oBoredSlider = SettingsSlider.Create(oBoredGroup, "Bored", 0f, 255f, 1f, (float)oSettings.BoredThreshold, OnBoredValueChanged);
			UIPanel pnlBoredGroup = (UIPanel)oBoredGroup.self;
			UILabel lblHint = pnlBoredGroup.AddUIComponent<UILabel>();
			lblHint.text = "[0..255] 0 = Just Arrived, 255 = Bored, time to go.";
		}

		private void SetupLineIssuesTab(UIHelper helperLineIssues)
		{
			ModSettings oSettings = PublicTransportInstance.GetSettings();

			// General
			UIHelper oLineIssuePanelGroup = (UIHelper)helperLineIssues.AddGroup("General");
			oLineIssuePanelGroup.AddCheckbox("Delete issues when closing Line Issue panel", oSettings.DeleteLineIssuesOnClosing, OnDeleteLineIssuesOnClosing);

			// Issues
			UIHelper oFlagsGroup = (UIHelper)helperLineIssues.AddGroup("Issues");
			oFlagsGroup.AddCheckbox("Flag if vehicles move slowly", oSettings.WarnVehicleMovesSlowly, OnWarnVehicleMovesSlowly);
			m_oSlowSlider = SettingsSlider.Create(oFlagsGroup, "       Days before vehicle is slow", 3f, 20f, 1f, (float)oSettings.WarnVehicleMovingSlowlyDays, OnSlowValueChanged);

			// Warnings group
			UIHelper oWarnGroup = (UIHelper)helperLineIssues.AddGroup("Warnings");
			UICheckBox oWarnDespawn = (UICheckBox)oWarnGroup.AddCheckbox("Warn if vehicles despawn", oSettings.WarnVehicleDespawed, OnWarnVehicleDespawed);
			oWarnGroup.AddCheckbox("Warn if vehicles stop moving (stuck)", oSettings.WarnVehicleStopsMoving, OnWarnVehicleStopsMoving);
			m_oStuckSlider = SettingsSlider.Create(oWarnGroup, "       Days before vehicle is stuck", 10f, 50, 1f, (float)oSettings.WarnVehicleStuckDays, OnStuckValueChanged);
			oWarnGroup.AddCheckbox("Warn if transport line issues detected", oSettings.WarnLineIssues, OnWarnLineIssues);
			m_playSound = (UICheckBox)oWarnGroup.AddCheckbox("Play sound when warnings are detected", oSettings.PlaySoundForWarnings, OnPlaySoundForWarnings);
			UpdatePlaySoundEnabled();
		}

		internal static UIPanel AddTab(UITabstrip tabStrip, string tabName, bool autoLayout = false)
		{
			const float Margin = 7.5f;

			// Create tab.
			int iTabCount = tabStrip.tabCount;
			UIButton tabButton = tabStrip.AddTab(tabName);

			// Sprites.
			tabButton.normalBgSprite = "SubBarButtonBase";
			tabButton.disabledBgSprite = "SubBarButtonBaseDisabled";
			tabButton.focusedBgSprite = "SubBarButtonBaseFocused";
			tabButton.hoveredBgSprite = "SubBarButtonBaseHovered";
			tabButton.pressedBgSprite = "SubBarButtonBasePressed";

			// Tooltip.
			tabButton.tooltip = tabName;
			tabButton.text = string.Empty;

			// Name label.
			UILabel tabLabel = tabButton.AddUIComponent<UILabel>();
			tabLabel.autoSize = true;
			tabLabel.textScale = 0.8f;
			tabLabel.text = tabName;
			tabLabel.PerformLayout();

			// Force tab size.
			tabButton.autoSize = false;
			tabButton.width = Mathf.Max(80f, tabLabel.width + Margin * 2f);

			// Centre name label.
			tabLabel.relativePosition = new Vector2((tabButton.width - tabLabel.width) / 2f, (tabButton.height - tabLabel.height) / 2f);

			// Get tab root panel.
			UIPanel rootPanel = tabStrip.tabContainer.components[iTabCount] as UIPanel;

			// Panel setup.
			rootPanel.autoLayout = autoLayout;
			rootPanel.autoLayoutDirection = LayoutDirection.Vertical;
			rootPanel.autoLayoutPadding.top = 5;
			rootPanel.autoLayoutPadding.left = 10;
			//rootPanel.backgroundSprite = "InfoviewPanel";
			//rootPanel.color = Color.red;

			return rootPanel;
		}

		private void UpdatePlaySoundEnabled()
        {
			if (m_playSound != null)
			{
				ModSettings oSettings = PublicTransportInstance.GetSettings();
				m_playSound.isEnabled = (oSettings.WarnVehicleStopsMoving || oSettings.WarnVehicleDespawed || oSettings.WarnLineIssues);
			} 
		}

		public void OnDeleteLineIssuesOnClosing(bool bIsChecked)
		{
			Debug.Log("DeleteLineIssuesOnClosing" + bIsChecked);
			ModSettings oSettings = PublicTransportInstance.GetSettings();
			oSettings.DeleteLineIssuesOnClosing = bIsChecked;
			oSettings.Save();
		}

		public void OnWarnLineIssues(bool bIsChecked)
		{
			Debug.Log("OnWarnLineIssues" + bIsChecked);
			ModSettings oSettings = PublicTransportInstance.GetSettings();
			oSettings.WarnLineIssues = bIsChecked;
			oSettings.Save();
			UpdatePlaySoundEnabled();
		}

		public void OnWarnVehicleDespawed(bool bIsChecked)
		{
			Debug.Log("OnWarnVehicleDespawed" + bIsChecked);
			ModSettings oSettings = PublicTransportInstance.GetSettings();
			oSettings.WarnVehicleDespawed = bIsChecked;
			oSettings.Save();
			UpdatePlaySoundEnabled();
		}

		public void OnWarnVehicleStopsMoving(bool bIsChecked)
		{
			Debug.Log("OnWarnVehicleStopsMoving" + bIsChecked);
			ModSettings oSettings = PublicTransportInstance.GetSettings();
			oSettings.WarnVehicleStopsMoving = bIsChecked;
			oSettings.Save();
			m_oStuckSlider?.Enable(bIsChecked);
			UpdatePlaySoundEnabled();
		}

		public void OnWarnVehicleMovesSlowly(bool bIsChecked)
		{
			Debug.Log("OnWarnVehicleMovesSlowly" + bIsChecked);
			ModSettings oSettings = PublicTransportInstance.GetSettings();
			oSettings.WarnVehicleMovesSlowly = bIsChecked;
			oSettings.Save();
			if (m_oSlowSlider != null)
            {
				m_oSlowSlider.Enable(bIsChecked);
			}
		}

		public void OnPlaySoundForWarnings(bool bIsChecked)
		{
			Debug.Log("OnPlaySoundForWarnings" + bIsChecked);
			ModSettings oSettings = PublicTransportInstance.GetSettings();
			oSettings.PlaySoundForWarnings = bIsChecked;
			oSettings.Save();
		}

		public void OnToolbarButtonChanged(bool bIsChecked)
		{
			Debug.Log("Checkbox changed" + bIsChecked);
			ModSettings oSettings = PublicTransportInstance.GetSettings();
			oSettings.MainToolbarButton = bIsChecked;
			oSettings.Save();

			if (PublicTransportInstance.s_isGameLoaded)
			{
				if (oSettings.MainToolbarButton)
				{
					PublicTransportInstance.ShowToolbarButton();
				}
				else
				{
					PublicTransportInstance.HideMainToolbarButton();
				}
			}
		}

		public void OnUnifiedToolbarButtonChanged(bool bIsChecked)
		{
			PublicTransportInfo.Debug.Log("Checkbox changed" + bIsChecked);
			ModSettings oSettings = PublicTransportInstance.GetSettings();
			oSettings.AddUnifiedUIButton = bIsChecked;
			oSettings.Save();

			if (PublicTransportInstance.s_isGameLoaded)
			{
				if (UnifiedUITool.Instance == null)
				{
					UnifiedUITool.AddUnifiedUITool();
				}
				else
				{
					UnifiedUITool.Instance.OnUnifiedToolbarButtonChanged();
				}
			}
		}

		public void OnBoredTextChanged(string text)
		{
		}

		public void OnBoredTextSubmitted(string sText)
		{
		}

		public void OnBoredValueChanged(float fValue)
		{
			ModSettings oSettings = PublicTransportInstance.GetSettings();
			oSettings.BoredThreshold = (int)fValue;
			oSettings.Save();
		}

		public void OnStuckValueChanged(float fValue)
		{
			ModSettings oSettings = PublicTransportInstance.GetSettings();
			oSettings.WarnVehicleStuckDays = (int)fValue;
			oSettings.Save();
		}

		public void OnSlowValueChanged(float fValue)
		{
			ModSettings oSettings = PublicTransportInstance.GetSettings();
			oSettings.WarnVehicleMovingSlowlyDays = (int)fValue;
			oSettings.Save();
		}
	}
}
