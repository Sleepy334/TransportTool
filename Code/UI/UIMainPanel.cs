using ColossalFramework.UI;
using UnityEngine;

namespace PublicTransportInfo
{
    public abstract class UIMainPanel<T> : UIPanel where T : UIPanel
    {
        private static T? s_mainPanel = null;

        // ----------------------------------------------------------------------------------------
        public static T Instance
        {
            get
            {
                if (s_mainPanel == null)
                {
                    s_mainPanel = (T) UIView.GetAView().AddUIComponent(typeof(T));
                    if (s_mainPanel is null)
                    {
                        Debug.Log("Error: creating Panel.");
                    }
                }

                return s_mainPanel;
            }
        }

        public static bool Exists
        {
            get
            {
                return s_mainPanel != null;
            }
        }

        public static void TogglePanel()
        {
            if (Exists)
            {
                if (Instance.isVisible)
                {
                    Instance.Hide();
                }
                else
                {
                    Instance.Show();
                }
            }
            else
            {
                Instance.Show();
            }
        }

        public static bool IsVisible()
        {
            return s_mainPanel is not null && 
                    s_mainPanel.isVisible;
        }

        public static void Destroy()
        {
            if (s_mainPanel is not null)
            {
                Object.Destroy(s_mainPanel.gameObject);
                s_mainPanel = null;
            }
        }

        public bool HandleEscape()
        {
            if (isVisible)
            {
                Hide();
                return true;
            }
            return false;
        }
    }
}