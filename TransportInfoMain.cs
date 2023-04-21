using ICities;
using PublicTransportInfo.Util;

namespace PublicTransportInfo
{
    public class ITransportInfoMain : IUserMod
    {
		public static string ModName => "TransportTool " + Version;

		private static string Version = "v2.5.2";
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

			Localization.LoadAllLanguageFiles();
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