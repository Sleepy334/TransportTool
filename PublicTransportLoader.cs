using ICities;
using System;
using UnifiedUI.Helpers;
using UnityEngine;

namespace PublicTransportInfo
{
    public class PublicTransportLoader : LoadingExtensionBase
    {
        private static PublicTransportInstance? instance = null;
        public static bool isGameLoaded = false;

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
                    Debug.Log("DestroyImmediate");
                    GameObject.DestroyImmediate(instance.gameObject);
                    instance = null;
                }

                instance = new GameObject("PublicTransportInfo").AddComponent<PublicTransportInstance>();

                if (PublicTransportInstance.s_mainPanel == null)
                {
                    PublicTransportInstance.Create();
                }

                LineIssueManager.Init();
                Patcher.PatchAll();
            }
        }

        // called when unloading begins
        public override void OnLevelUnloading()
        {
            Debug.Log("OnLevelUnloading");
            base.OnLevelUnloading();
            isGameLoaded = false;

            try
            {
                if (instance != null)
                {
                    GameObject.Destroy(instance.gameObject);
                }
            }
            catch (Exception e)
            {
                PublicTransportInfo.Debug.Log(e);
            }
        }

        // called when unloading finished
        public override void OnReleased()
        {
            base.OnReleased();
            PublicTransportInstance.Destroy();
            Patcher.UnpatchAll();
        }
    }
}
