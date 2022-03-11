using ICities;
using System;
using UnifiedUI.Helpers;
using UnityEngine;

namespace PublicTransportInfo
{
    public class PublicTransportLoading : ILoadingExtension
    {
        private static PublicTransportInstance? instance = null;
        public static bool isGameLoaded = false;

        // called when level loading begins
        public void OnCreated(ILoading loading)
        {
        }

        // called when level is loaded
        public void OnLevelLoaded(LoadMode mode)
        {
            Debug.Log("OnLevelLoaded"); 
            
            isGameLoaded = true;

            if (instance != null)
            {
                Debug.Log("DestroyImmediate"); 
                GameObject.DestroyImmediate(instance.gameObject);
                instance = null;
            }

            instance = new GameObject("PublicTransportInfo").AddComponent<PublicTransportInstance>();
        }

        // called when unloading begins
        public void OnLevelUnloading()
        {
            Debug.Log("OnLevelUnloading");  
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
        public void OnReleased()
        {
            PublicTransportInstance.Destroy();
        }
    }
}
