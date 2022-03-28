using ColossalFramework;
using ColossalFramework.UI;
using ICities;
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

        [XmlElement("AddUnifiedUIButton")]
        public bool AddUnifiedUIButton
        {
            get;
            set;
        } = true;

        [XmlElement("MainToolbarButton")]
        public bool MainToolbarButton
        {
            get;
            set;
        } = true;

        [XmlElement("BoredThreshold")]
        public int BoredThreshold
        {
            get;
            set;
        } = 200;

        [XmlElement("WarnVehicleDespawed")]
        public bool WarnVehicleDespawed
        {
            get;
            set;
        } = true;

        [XmlElement("WarnVehicleStopsMoving")]
        public bool WarnVehicleStopsMoving
        {
            get;
            set;
        } = true;

        [XmlElement("WarnVehicleMovesSlowly")]
        public bool WarnVehicleMovesSlowly
        {
            get;
            set;
        } = true;

        [XmlElement("PlaySoundForWarnings")]
        public bool PlaySoundForWarnings
        {
            get;
            set;
        } = true;

        public int WarnVehicleStuckDays
        {
            get;
            set;
        } = 20;

        public int WarnVehicleMovingSlowlyDays
        {
            get;
            set;
        } = 10;

        public bool WarnLineIssues
        {
            get;
            set;
        } = true;

        public bool DeleteLineIssuesOnClosing
        {
            get;
            set;
        } = true;

        
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

        public static ModSettings Load()
        {
            Debug.Log("Loading settings: " + SettingsFile); 
            try
            {
                // Read settings file.
                using (StreamReader reader = new StreamReader(SettingsFile))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings));
                    ModSettings? oSettings = xmlSerializer.Deserialize(reader) as ModSettings;
                    if (oSettings != null)
                    {
                        return oSettings;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            return new ModSettings();
        }

        /// <summary>
        /// Save settings to XML file.
        /// </summary>
        public void Save()
        {
            Debug.Log("Saving settings: " + SettingsFile); 
            try
            {
                // Pretty straightforward.
                using (StreamWriter writer = new StreamWriter(SettingsFile))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings)); 
                    xmlSerializer.Serialize(writer, PublicTransportInstance.GetSettings());
                }

                // Cleaning up after ourselves - delete any old config file in the application direcotry.
                if (File.Exists(SettingsFileName))
                {
                    File.Delete(SettingsFileName);
                }

                Debug.Log("User Setting Configuration successful saved."); 
            }
            catch (Exception ex)
            {
                Debug.Log("Saving settings file failed.", ex); 
            }
        }

        public bool TrackVehicles()
        {
            return WarnVehicleMovesSlowly || WarnVehicleStopsMoving || WarnVehicleDespawed;
        }
    }
}
