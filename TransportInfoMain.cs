using ColossalFramework.UI;
using ICities;
using PublicTransportInfo.UnifiedUI;
using System;
using UnityEngine;

namespace PublicTransportInfo
{
    public class ITransportInfoMain : IUserMod
    {
		public static string ModName => "TransportTool " + Version;

		private static string Version = "v1.3";
		public static string Title => "Transport Tool" + " " + Version;

		private UITextField txtBored = null;

		public string Name
		{
			get { return ModName; }
		}

		public string Description
		{
			get { return "Transport tool for monitoring public transport usage"; }
		}

        // Sets up a settings user interface
        public void OnSettingsUI(UIHelper helper)
        {
			ModSettings.Load();

			// Title
			helper.AddGroup(Title);
			helper.AddSpace(10);

			// UI Buttons
			UIHelperBase oButtonGroup = helper.AddGroup("Buttons");
			oButtonGroup.AddCheckbox("Add button to main toolbar", ModSettings.s_bAddMainToolbarButton, OnToolbarButtonChanged);
			oButtonGroup.AddCheckbox("Add button to UnifiedUI toolbar", ModSettings.s_bAddUnifiedUIButton, OnUnifiedToolbarButtonChanged);

			helper.AddSpace(10);
			// Bored
			UIHelperBase oBoredGroup = helper.AddGroup("Bored Threshold");
			txtBored = (UITextField)oBoredGroup.AddTextfield("[0..255] 0 = Just Arrived, 255 = Sick of waiting time to go.", ModSettings.s_iBoredThreshold.ToString(), OnBoredTextChanged, OnBoredTextSubmitted);	
			if (txtBored == null)
            {
				Debug.Log("Failed to add txtBored");
            }
		}

		public void OnToolbarButtonChanged(bool bIsChecked) 
        {
			Debug.Log("Checkbox changed" + bIsChecked);
			ModSettings.s_bAddMainToolbarButton = bIsChecked;
			ModSettings.Save();

			if (PublicTransportInstance.s_isGameLoaded)
			{
				if (ModSettings.s_bAddMainToolbarButton)
				{
					PublicTransportInstance.AddToolbarButton();
				}
				else
				{
					PublicTransportInstance.HideMainToolbarButton();
				}
			}
		}

		public void OnUnifiedToolbarButtonChanged(bool bIsChecked) 
		{
			Debug.Log("Checkbox changed" + bIsChecked);
			ModSettings.s_bAddUnifiedUIButton = bIsChecked;
			ModSettings.Save();

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
			if (Int32.TryParse(sText, out int numValue) && (0 <= numValue && numValue <= 255))
            {
				ModSettings.s_iBoredThreshold = numValue;
				ModSettings.Save();
			}
			else
            {
				if (txtBored != null)
                {
					txtBored.text = ModSettings.s_iBoredThreshold.ToString();
				} else
                {
					Debug.Log("txtBored is null");
                }
			}

		}
	}
}