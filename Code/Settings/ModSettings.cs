using ColossalFramework;
using SleepyCommon;
using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace PublicTransportInfo
{
    public class ModSettings
    {
        

        private static ModSettings s_settings = null;

        public static ModSettings GetSettings()
        {
            if (s_settings == null)
            {
                s_settings = ModSettings.Load();
            }
            return s_settings;
        }

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

        [XmlElement("WarnBoredCountExceedsThreshold")]
        public bool WarnBoredCountExceedsThreshold
        {
            get;
            set;
        } = true;

        [XmlElement("WarnBoredCountThreshold")]
        public int WarnBoredCountThreshold
        {
            get;
            set;
        } = 50;

        [XmlElement("PlaySoundForWarnings")]
        public bool PlaySoundForWarnings
        {
            get;
            set;
        } = true;
        
        public int WarnVehicleMovingSlowlyThreshold
        {
            get;
            set;
        } = 150;

        public int WarnVehicleBlockedThreshold
        {
            get;
            set;
        } = 220;

        public bool WarnLineIssues
        {
            get;
            set;
        } = true;

        public int TooltipRowLimit
        {
            get;
            set;
        } = 20;

        public int TooltipFontSize
        {
            get;
            set;
        } = 16;
        public bool LineIssueLocationSaved
        {
            get;
            set;
        } = false;

        public Vector3 LineIssueLocation
        {
            get;
            set;
        }

        public bool ZoomInOnTarget
        {
            get;
            set;
        } = false;

        public bool DisableTransparency
        {
            get;
            set;
        } = false;

        public int DeleteResolvedDelay
        {
            get;
            set;
        } = 10;

        public string PreferredLanguage
        {
            get
            {
                return Localization.PreferredLanguage;
            }
            set
            {
                Localization.PreferredLanguage = value;
            }
        }

        public bool ActivatePublicTransportInfoView
        {
            get;
            set;
        } = true;

        public bool HighlightCitizenDestination { get; set; } = true;
        
        // ----------------------------------------------------------------------------------------
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

        [XmlIgnore]
        public static SavedInputKey LineIssueHotkey = new SavedInputKey(
            "TransportTool_LineIssue_Hotkey", SETTINGS_FILE_NAME,
            key: KeyCode.Alpha1, control: true, shift: false, alt: false, true);

        public static ModSettings Load()
        {
#if DEBUG
            CDebug.Log("Loading settings: " + SettingsFile);
#endif
            try
            {
                // Read settings file.
                if (File.Exists(SettingsFile))
                {
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
            }
            catch (Exception e)
            {
                CDebug.Log(e);
            }

            return new ModSettings();
        }

        /// <summary>
        /// Save settings to XML file.
        /// </summary>
        public void Save()
        {
            try
            {
                // Pretty straightforward.
                using (StreamWriter writer = new StreamWriter(SettingsFile))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings));
                    xmlSerializer.Serialize(writer, this);
                }
            }
            catch (Exception ex)
            {
                CDebug.Log("Saving settings file failed.", ex);
            }
        }

        public bool TrackVehicles()
        {
            return WarnVehicleMovesSlowly || WarnVehicleStopsMoving || WarnVehicleDespawed;
        }

        public int GetBlockedVehicleMinThreshold()
        {
            int iWarnValue = 255;
            if (WarnVehicleStopsMoving)
            {
                iWarnValue = WarnVehicleBlockedThreshold;
            }
            if (WarnVehicleMovesSlowly)
            {
                iWarnValue = Math.Min(iWarnValue, WarnVehicleMovingSlowlyThreshold);
            }
            return iWarnValue;
        }
    }
}
