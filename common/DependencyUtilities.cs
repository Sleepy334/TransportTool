using ColossalFramework.Plugins;
using UnityEngine;

namespace PublicTransportInfo
{
    public static class DependencyUtilities
    {
        private static bool s_bHasTransportManagerBeenCheckedRunning = false;
        private static bool s_bIsTransportManagerRunning = false;

        private static bool s_bHasUnifiedUIBeenCheckedRunning = false;
        private static bool s_bIsUnifiedUIRunning = false;

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

        public static bool IsTransportLinesManagerRunning()
        {
            const string sTRANSPORT_LINES_MANAGER_ID = "1312767991";

            // Only cache result once map is loaded
            if (PublicTransportLoading.isGameLoaded)
            {
                if (!s_bHasTransportManagerBeenCheckedRunning)
                {
                    s_bIsTransportManagerRunning = IsPluginRunning(sTRANSPORT_LINES_MANAGER_ID);
                    s_bHasTransportManagerBeenCheckedRunning = true;
                }

                return s_bIsTransportManagerRunning;
            }
            else
            {
                return IsPluginRunning(sTRANSPORT_LINES_MANAGER_ID);
            }
        }

        public static bool IsUnifiedUIRunning()
        {
            const string sUNIFIED_UI_ID = "2255219025";

            // Only cache result once map is loaded
            if (PublicTransportLoading.isGameLoaded)
            {
                if (!s_bHasUnifiedUIBeenCheckedRunning)
                {
                    s_bIsUnifiedUIRunning = IsPluginRunning(sUNIFIED_UI_ID);
                    s_bHasUnifiedUIBeenCheckedRunning = true;
                }

                return s_bIsUnifiedUIRunning;
            }
            else
            {
                return IsPluginRunning(sUNIFIED_UI_ID);
            }
        }
    }
}
