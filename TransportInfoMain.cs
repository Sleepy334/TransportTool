using ColossalFramework.UI;
using ICities;
using System;
using UnityEngine;

namespace PublicTransportInfo
{
    public class ITransportInfoMain : IUserMod
    {
		public static string ModName => "TransportTool " + Version;

		private static string Version = "v1.6.0";
		public static string Title => "Transport Tool" + " " + Version;

		private UISlider m_sliderBored = null;
		private UILabel m_lblBored = null;

		private UISlider m_sliderStuck = null;
		private UILabel m_lblStuck = null;

		SettingsUI m_oSettingsUI = null;

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
			if (m_oSettingsUI == null)
            {
				m_oSettingsUI = new SettingsUI();

			}
			m_oSettingsUI.OnSettingsUI(helper);
		}
	}
}