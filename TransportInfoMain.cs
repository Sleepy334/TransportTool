using ICities;
using PublicTransportInfo.UnifiedUI;
using UnityEngine;

namespace PublicTransportInfo
{
    public class ITransportInfoMain : IUserMod
    {
		public static string ModName => "TransportTool " + Version;

		private static string Version = "v1.2";
		public static string Title => "Transport Tool" + " " + Version;

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

			// UI Buttons
			helper.AddCheckbox("Add button to main toolbar", ModSettings.s_bAddMainToolbarButton, OnToolbarButtonChanged);
			helper.AddCheckbox("Add button to UnifiedUI toolbar", ModSettings.s_bAddUnifiedUIButton, OnUnifiedToolbarButtonChanged);
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
	}
}