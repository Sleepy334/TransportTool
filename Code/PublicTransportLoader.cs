using ICities;
using System;
using UnityEngine;

namespace PublicTransportInfo
{
    public class PublicTransportLoader : LoadingExtensionBase
    {
        private static PublicTransportInstance? instance = null;
        public static bool isGameLoaded = false;
        private GameObject? m_keyboardShortcutGameObject = null;

        // ----------------------------------------------------------------------------------------
        private static bool ActiveInMode(LoadMode mode)
        {
            switch (mode)
            {
                case LoadMode.NewGame:
                case LoadMode.NewGameFromScenario:
                case LoadMode.LoadGame:
                    return true;

                default:
                    return false;
            }
        }

        // called when level is loaded
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (ActiveInMode(mode))
            {
                isGameLoaded = true;

                if (instance != null)
                {
                    GameObject.DestroyImmediate(instance.gameObject);
                    instance = null;
                }

                instance = new GameObject("PublicTransportInfo").AddComponent<PublicTransportInstance>();

                if (m_keyboardShortcutGameObject is null)
                {
                    m_keyboardShortcutGameObject = new GameObject("KeyboardShortcuts");
                    m_keyboardShortcutGameObject.AddComponent<KeyboardShortcuts>();
                }

                PublicTransportInstance.Create();
                LineIssueManager.Init();
                Patcher.PatchAll();
            }
        }

        // called when unloading begins
        public override void OnLevelUnloading()
        {
            Debug.Log("OnLevelUnloading");
            isGameLoaded = false;

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
                PublicTransportInfo.Debug.Log(e);
            }

            base.OnLevelUnloading();
        }

        // called when unloading finished
        public override void OnReleased()
        {
            base.OnReleased();
            Patcher.UnpatchAll();
        }
    }
}
