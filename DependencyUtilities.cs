using ColossalFramework.Plugins;
using UnityEngine;

namespace PublicTransportInfo
{
    public static class DependencyUtilities
    {
        public static void SearchPlugins()
        {
            string sPlugins = "";
            foreach (PluginManager.PluginInfo oPlugin in  PluginManager.instance.GetPluginsInfo())
            {
                sPlugins += oPlugin.name + " " + oPlugin.GetHashCode() + "\r\n";
            }
            PublicTransportInfo.Debug.Log(sPlugins);
        }

        public static bool IsPluginRunning(string sPluginId)
        {
            bool bRunning = false;
            foreach (PluginManager.PluginInfo oPlugin in PluginManager.instance.GetPluginsInfo())
            {
                if (oPlugin.name == sPluginId && oPlugin.isEnabled)
                {
                    bRunning = true;
                    break;
                };
            }
            return bRunning;
        }
    }
}
