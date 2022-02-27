using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using PublicTransportInfo.UnifiedUI;
using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace PublicTransportInfo
{
    public class ModSettings
    {
        [XmlIgnore]
        const string SETTINGS_FILE_NAME = "TransportToolSettings";

        [XmlIgnore]
        private static readonly string SettingsFileName = "TransportToolSettings.xml";

        [XmlIgnore]
        private static readonly string UserSettingsDir = ColossalFramework.IO.DataLocation.localApplicationData;

        [XmlIgnore]
        private static readonly string SettingsFile = Path.Combine(UserSettingsDir, SettingsFileName);

        [XmlIgnore]
        public static bool s_bAddUnifiedUIButton = true;

        [XmlIgnore]
        public static bool s_bAddMainToolbarButton = true;

        [XmlIgnore]
        public static int s_iBoredThreshold = 200;

        [XmlElement("UnifiedUIButton")]
        public bool AddUnifiedUIButton
        {
            get => s_bAddUnifiedUIButton;

            set => s_bAddUnifiedUIButton = value;
        }

        [XmlElement("MainToolbarButton")]
        public bool MainToolbarButton
        {
            get => s_bAddMainToolbarButton;

            set => s_bAddMainToolbarButton = value;
        }

        [XmlElement("BoredThreshold")]
        public int BoredThreshold
        {
            get => s_iBoredThreshold;

            set => s_iBoredThreshold = value;
        }

        static ModSettings()
        {
            if (GameSettings.FindSettingsFileByName(SETTINGS_FILE_NAME) == null)
            {
                GameSettings.AddSettingsFile(new SettingsFile[] { new SettingsFile() { fileName = SETTINGS_FILE_NAME } });
            }
        }

        [XmlIgnore]
        public static SavedInputKey Hotkey = new SavedInputKey(
            "TransportTool_Hotkey", SETTINGS_FILE_NAME,
            key: KeyCode.I, control: true, shift: false, alt: false, true);


        public void OnSettingsUI(UIHelper helper)
        {
            /*
            Load();

            try
            {
                Debug.Log(Environment.StackTrace);

                helper.AddGroup(ITransportInfoMain.Title);

                // UI Buttons
                helper.AddCheckbox("Add button to main toolbar", s_bAddUnifiedUIButton, OnToolbarButtonChanged);
                helper.AddCheckbox("Add button to UnifiedUI toolber", s_bAddMainToolbarButton, OnUnifiedToolbarButtonChanged);

                var keymappingsPanel = helper.AddKeymappingsPanel();
                keymappingsPanel.AddKeymapping("Hotkey", Hotkey);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                UIView.ForwardException(ex);
            }
            */
        }

        internal static void Load()
        {
            Debug.Log("Loading settings: " + SettingsFile); 
            try
            {
                // Attempt to read new settings file (in user settings directory).    
                string fileName = SettingsFile;
                if (!File.Exists(fileName))
                {
                    // No settings file in user directory; use application directory instead. If still no settings file, set default values.
                    fileName = SettingsFileName;

                    if (!File.Exists(fileName))
                    {
                        Debug.Log("no settings file found");

                        s_bAddUnifiedUIButton = true;
                        s_bAddMainToolbarButton = true;

                        return;
                    }
                }

                // Read settings file.
                using (StreamReader reader = new StreamReader(fileName))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings));
                    if (!(xmlSerializer.Deserialize(reader) is ModSettings settingsFile))
                    {
                        Debug.Log("couldn't deserialize settings file");
                    }

                    Debug.Log("User Setting Configuration successful loaded.");
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Save settings to XML file.
        /// </summary>
        internal static void Save()
        {
            Debug.Log("Saving settings: " + SettingsFile); 
            try
            {
                // Pretty straightforward.
                using (StreamWriter writer = new StreamWriter(SettingsFile))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings)); 
                    xmlSerializer.Serialize(writer, new ModSettings());
                }

                // Cleaning up after ourselves - delete any old config file in the application direcotry.
                if (File.Exists(SettingsFileName))
                {
                    File.Delete(SettingsFileName);
                }

                Debug.Log("User Setting Configuration successful saved.");
            }
            catch (Exception e)
            {
                Debug.Log("Saving settings file failed."); 
                Debug.LogException(e);
            }
        }
    }
}
