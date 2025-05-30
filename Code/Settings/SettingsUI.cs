﻿using ColossalFramework.UI;
using ICities;
using SleepyCommon;
using UnityEngine;

namespace PublicTransportInfo
{
    public class SettingsUI
    {
        SettingsSlider? m_oStuckSlider = null;
		SettingsSlider? m_oSlowSlider = null;
		SettingsSlider? m_oBoredCountSlider = null;
		UICheckBox? m_playSound = null;

		public SettingsUI()
        {
        }
        
        public void OnSettingsUI(UIHelper helper)
        {
			// Add tabstrip.
			ExtUITabstrip tabStrip = ExtUITabstrip.Create(helper);
			UIHelper tabGeneral = tabStrip.AddTabPage(Localization.Get("tabGeneral"), true);
			UIHelper tabMainPanel = tabStrip.AddTabPage(Localization.Get("tabMainPanel"), true);
			UIHelper tabLineIssues = tabStrip.AddTabPage(Localization.Get("tabLineIssues"), true);

			// general tab
			SetupGeneralTab(tabGeneral);

			// Main Panel
			SetupMainPanelTab(tabMainPanel);

			// Line Issues
			SetupLineIssuesTab(tabLineIssues);

			tabStrip.selectedIndex = 0;
		}

		private void SetupGeneralTab(UIHelper helperGeneral)
        {
			ModSettings oSettings = ModSettings.GetSettings();

			UIHelper groupLocalisation = (UIHelper)helperGeneral.AddGroup(Localization.Get("GROUP_LOCALISATION"));
			groupLocalisation.AddDropdown(Localization.Get("dropdownLocalization"), Localization.GetLoadedLanguages(), Localization.GetLanguageIndexFromCode(oSettings.PreferredLanguage), OnLocalizationDropDownChanged);

			// UI Buttons
			UIHelperBase oButtonGroup = helperGeneral.AddGroup(Localization.Get("groupToolbarButtons"));
			oButtonGroup.AddCheckbox(Localization.Get("btnAddMainToolbar"), oSettings.MainToolbarButton, OnToolbarButtonChanged);
			oButtonGroup.AddCheckbox(Localization.Get("btnAddUnifiedUI"), oSettings.AddUnifiedUIButton, OnUnifiedToolbarButtonChanged);

			// Keyboard shortcut
			UIHelper groupKeyboardShortcuts = (UIHelper)helperGeneral.AddGroup(Localization.Get("groupKeyboardShortcuts"));
			UIPanel panel = (UIPanel)groupKeyboardShortcuts.self;
			UIKeymappingsPanel keymappings = panel.gameObject.AddComponent<UIKeymappingsPanel>();
			keymappings.AddKeymapping(Localization.Get("keyOpenMainPanel"), ModSettings.Hotkey); // Automatically saved
			
			UIKeymappingsPanel keymappingsLineIssue = panel.gameObject.AddComponent<UIKeymappingsPanel>();
			keymappingsLineIssue.AddKeymapping(Localization.Get("keyOpenIssuesPanel"), ModSettings.LineIssueHotkey); // Automatically saved
        }

		private void SetupMainPanelTab(UIHelper helperMainPanel)
		{
			ModSettings oSettings = ModSettings.GetSettings();

			// Zoom
			UIHelper oBehaviourGroup = (UIHelper)helperMainPanel.AddGroup(Localization.Get("groupBehaviour"));
			oBehaviourGroup.AddCheckbox(Localization.Get("chkZoomInOnTarget"), oSettings.ZoomInOnTarget, OnZoomInChanged);
            oBehaviourGroup.AddCheckbox(Localization.Get("chkDisableTransparency"), oSettings.DisableTransparency, OnDisableTransparencyChanged);
			oBehaviourGroup.AddCheckbox(Localization.Get("chkActivatePublicTransportInfoView"), oSettings.ActivatePublicTransportInfoView, (bChecked) => { oSettings.ActivatePublicTransportInfoView = bChecked; oSettings.Save(); });

            // Bored Slider
            UIHelper oBoredGroup = (UIHelper)helperMainPanel.AddGroup(Localization.Get("groupBoredThreshold"));
			SettingsSlider oBoredSlider = SettingsSlider.CreateSettingsStyle(oBoredGroup, LayoutDirection.Horizontal, Localization.Get("sliderBored"), 400, 200, 0f, 255f, 1f, (float)oSettings.BoredThreshold, 0, OnBoredValueChanged);
			UIPanel pnlBoredGroup = (UIPanel)oBoredGroup.self;
			UILabel lblHint = pnlBoredGroup.AddUIComponent<UILabel>();
			lblHint.text = Localization.Get("txtBoredHint");

			// Tooltips
			UIHelper oTooltipGroup = (UIHelper)helperMainPanel.AddGroup(Localization.Get("groupTooltips"));
			SettingsSlider.CreateSettingsStyle(oTooltipGroup, LayoutDirection.Horizontal, Localization.Get("sliderFontSize"), 400, 200, 8f, 32f, 1f, (float)oSettings.TooltipFontSize, 0, OnTooltipFontSizeChanged);
			SettingsSlider.CreateSettingsStyle(oTooltipGroup, LayoutDirection.Horizontal, Localization.Get("sliderTooltipRowLimit"), 400, 200, 0f, 100f, 1f, (float)oSettings.TooltipRowLimit, 0, OnTooltipRowLimitValueChanged);
		}

		private void SetupLineIssuesTab(UIHelper helperLineIssues)
		{
			ModSettings oSettings = ModSettings.GetSettings();

			// General
            UIHelper groupGeneral = (UIHelper)helperLineIssues.AddGroup(Localization.Get("groupLineIssuesGeneral"));
            SettingsSlider.CreateSettingsStyle(groupGeneral, LayoutDirection.Horizontal, Localization.Get("txtDeleteresolvedDelay"), 400, 200, 0f, 30f, 1f, (float)oSettings.DeleteResolvedDelay, 0, OnDeleteResolvedChanged);

            // Issues
            UIHelper oFlagsGroup = (UIHelper)helperLineIssues.AddGroup(Localization.Get("groupIssues"));
			oFlagsGroup.AddCheckbox(Localization.Get("chkFlagMoveSlowly"), oSettings.WarnVehicleMovesSlowly, OnWarnVehicleMovesSlowly);
			m_oSlowSlider = SettingsSlider.CreateSettingsStyle(oFlagsGroup, LayoutDirection.Horizontal, Localization.Get("sliderSlowVehicleThreshold"), 400, 200, 1f, 255f, 1f, (float)oSettings.WarnVehicleMovingSlowlyThreshold, 0, OnSlowValueChanged);
            m_oSlowSlider.m_panel.padding.left = 30;

            // Warnings group
            UIHelper oWarnGroup = (UIHelper)helperLineIssues.AddGroup(Localization.Get("groupWarnings"));
			
			UICheckBox oWarnDespawn = (UICheckBox)oWarnGroup.AddCheckbox(Localization.Get("chkWarnDespawn"), oSettings.WarnVehicleDespawed, OnWarnVehicleDespawed);
			
			oWarnGroup.AddCheckbox(Localization.Get("chkWarnBlocked"), oSettings.WarnVehicleStopsMoving, OnWarnVehicleStopsMoving);
			m_oStuckSlider = SettingsSlider.CreateSettingsStyle(oWarnGroup, LayoutDirection.Horizontal, Localization.Get("sliderBlockedThreshold"), 400, 200, 1f, 255f, 1f, (float)oSettings.WarnVehicleBlockedThreshold, 0, OnStuckValueChanged);
            m_oStuckSlider.m_panel.padding.left = 30;

            oWarnGroup.AddCheckbox(Localization.Get("chkLineIssues"), oSettings.WarnLineIssues, OnWarnLineIssues);
			
			oWarnGroup.AddCheckbox(Localization.Get("chkBoredCount"), oSettings.WarnBoredCountExceedsThreshold, OnWarnBoredCount);
			m_oBoredCountSlider = SettingsSlider.CreateSettingsStyle(oWarnGroup, LayoutDirection.Horizontal, Localization.Get("sliderBoredCountThreshold"), 400, 200, 1f, 500f, 1f, (float)oSettings.WarnBoredCountThreshold, 0, OnWarnBoredCountThreshold);
			m_oBoredCountSlider.m_panel.padding.left = 30;

            m_playSound = (UICheckBox)oWarnGroup.AddCheckbox(Localization.Get("chkPlaySound"), oSettings.PlaySoundForWarnings, OnPlaySoundForWarnings);
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

		public void OnLocalizationDropDownChanged(int value)
		{
			ModSettings oSettings = ModSettings.GetSettings();
			oSettings.PreferredLanguage = Localization.GetLoadedCodes()[value];
			oSettings.Save();
		}
		public void OnWarnBoredCount(bool bChecked)
		{
			ModSettings oSettings = ModSettings.GetSettings();
			oSettings.WarnBoredCountExceedsThreshold = bChecked;
			oSettings.Save();

			if (m_oBoredCountSlider != null)
            {
				m_oBoredCountSlider.Enable(bChecked);
			}
		}

		public void OnWarnBoredCountThreshold(float value)
		{
			ModSettings oSettings = ModSettings.GetSettings();
			oSettings.WarnBoredCountThreshold = (int)value;
			oSettings.Save();
		}

		private void UpdatePlaySoundEnabled()
        {
			if (m_playSound != null)
			{
				ModSettings oSettings = ModSettings.GetSettings();
				m_playSound.isEnabled = (oSettings.WarnVehicleStopsMoving || oSettings.WarnVehicleDespawed || oSettings.WarnLineIssues);
			} 
		}

		public void OnZoomInChanged(bool bChecked)
		{
			ModSettings oSettings = ModSettings.GetSettings();
			oSettings.ZoomInOnTarget = bChecked;
			oSettings.Save();
		}

        public void OnDisableTransparencyChanged(bool bChecked)
        {
            ModSettings oSettings = ModSettings.GetSettings();
            oSettings.DisableTransparency = bChecked;
            oSettings.Save();
        }
        

        public void OnTooltipFontSizeChanged(float fValue)
		{
			ModSettings oSettings = ModSettings.GetSettings();
			oSettings.TooltipFontSize = (int)fValue;
			PublicTransportInstance.InvalidateFont(); 

			oSettings.Save();
		}

		public void OnTooltipRowLimitValueChanged(float fValue)
		{
			ModSettings oSettings = ModSettings.GetSettings();
			oSettings.TooltipRowLimit = (int)fValue;
			oSettings.Save();
		}

		public void OnWarnLineIssues(bool bIsChecked)
		{
			ModSettings oSettings = ModSettings.GetSettings();
			oSettings.WarnLineIssues = bIsChecked;
			oSettings.Save();
			UpdatePlaySoundEnabled();
		}

		public void OnWarnVehicleDespawed(bool bIsChecked)
		{
			ModSettings oSettings = ModSettings.GetSettings();
			oSettings.WarnVehicleDespawed = bIsChecked;
			oSettings.Save();
			UpdatePlaySoundEnabled();
		}

		public void OnWarnVehicleStopsMoving(bool bIsChecked)
		{
			ModSettings oSettings = ModSettings.GetSettings();
			oSettings.WarnVehicleStopsMoving = bIsChecked;
			oSettings.Save();
			m_oStuckSlider?.Enable(bIsChecked);
			UpdatePlaySoundEnabled();
		}

		public void OnWarnVehicleMovesSlowly(bool bIsChecked)
		{
			ModSettings oSettings = ModSettings.GetSettings();
			oSettings.WarnVehicleMovesSlowly = bIsChecked;
			oSettings.Save();
			if (m_oSlowSlider != null)
            {
				m_oSlowSlider.Enable(bIsChecked);
			}
		}

		public void OnPlaySoundForWarnings(bool bIsChecked)
		{
			ModSettings oSettings = ModSettings.GetSettings();
			oSettings.PlaySoundForWarnings = bIsChecked;
			oSettings.Save();
		}

		public void OnToolbarButtonChanged(bool bIsChecked)
		{
			ModSettings oSettings = ModSettings.GetSettings();
			oSettings.MainToolbarButton = bIsChecked;
			oSettings.Save();

			if (TransportToolMod.Instance.IsLoaded)
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
			ModSettings oSettings = ModSettings.GetSettings();
			oSettings.AddUnifiedUIButton = bIsChecked;
			oSettings.Save();

			if (TransportToolMod.Instance.IsLoaded)
			{
				if (UnifiedUIButton.Exists)
				{
                    UnifiedUIButton.OnUnifiedToolbarButtonChanged(); 
				}
				else
				{
                    UnifiedUIButton.Add();
                }
			}
		}

		public void OnBoredValueChanged(float fValue)
		{
			ModSettings oSettings = ModSettings.GetSettings();
			oSettings.BoredThreshold = (int)fValue;
			oSettings.Save();
		}

		public void OnSlowValueChanged(float fValue)
		{
			ModSettings oSettings = ModSettings.GetSettings();
			oSettings.WarnVehicleMovingSlowlyThreshold = (int)fValue;
			oSettings.Save();
		}

		public void OnStuckValueChanged(float fValue)
		{
			ModSettings oSettings = ModSettings.GetSettings();
			oSettings.WarnVehicleBlockedThreshold = (int)fValue;
			oSettings.Save();
		}

        public void OnDeleteResolvedChanged(float fValue)
        {
            ModSettings oSettings = ModSettings.GetSettings();
            oSettings.DeleteResolvedDelay = (int)fValue;
            oSettings.Save();
        }
        
    }
}
