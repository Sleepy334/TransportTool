using ICities;
using System.Reflection;
using System;

namespace PublicTransportInfo
{
    public class ITransportInfoMain : IUserMod
    {
		public static string ModName => "TransportTool " + Version;

        public static string Version
        {
            get
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return $"v{version.Major}.{version.Minor}.{version.Build}";
            }
        }

        public static string Title => "Transport Tool" + " " + Version;

		public static bool IsEnabled = false;

		SettingsUI? m_oSettingsUI = null;

		public string Name
		{
			get { return ModName; } 
		}

		public string Description
		{
			get { return "Transport tool for monitoring public transport usage"; }
		}

		public void OnEnabled()
		{
			IsEnabled = true;
		}

		public void OnDisabled()
		{
			IsEnabled = false;
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