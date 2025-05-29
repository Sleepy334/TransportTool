using SleepyCommon;
using System.Reflection;
using System;
using UnityEngine;

namespace PublicTransportInfo
{
    public class TransportToolMod : UserModBase
    {
        private GameObject? m_keyboardShortcutGameObject = null;
        private static PublicTransportInstance? instance = null;
        private SettingsUI? m_oSettingsUI = null;

        // ----------------------------------------------------------------------------------------
        public static TransportToolMod Instance
        {
            get
            {
                return (TransportToolMod)UserModBase.BaseInstance;
            }
        }

        public TransportToolMod() : 
            base()
        {
        }

        public override string ModName 
		{ 
			get
			{
				return "Transport Tool";
			}
		}

        public override string Version
        {
            get
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return $"v{version.Major}.{version.Minor}.{version.Build}";
            }
        }

		public override string Description
		{
			get { return "Transport tool for monitoring public transport usage"; }
		}

		public override void OnLevelLoaded()
		{
            if (m_keyboardShortcutGameObject is null)
            {
                m_keyboardShortcutGameObject = new GameObject("KeyboardShortcuts");
                m_keyboardShortcutGameObject.AddComponent<KeyboardShortcuts>();
            }

            PublicTransportInstance.Create();
            LineIssueManager.Init();
            Patcher.PatchAll();
        }

        public override void OnLevelUnloading()
        {
            IsLoaded = false;

            try
            {
                // Remove keyboard handler
                if (m_keyboardShortcutGameObject is not null)
                {
                    GameObject.Destroy(m_keyboardShortcutGameObject.gameObject);
                    m_keyboardShortcutGameObject = null;
                }

                if (instance != null)
                {
                    PublicTransportInstance.Destroy();

                    GameObject.Destroy(instance.gameObject);
                    instance = null;
                }
            }
            catch (Exception e)
            {
                CDebug.Log(e);
            }

            base.OnLevelUnloading();
        }

        // called when unloading finished
        public override void OnReleased()
        {
            base.OnReleased();
            Patcher.UnpatchAll();
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