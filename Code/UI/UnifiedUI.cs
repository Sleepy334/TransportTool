using ColossalFramework.UI;
using SleepyCommon;
using System.Reflection;
using UnifiedUI.Helpers;
using UnityEngine;

namespace PublicTransportInfo
{
    public class UnifiedUIButton
    {
        private static UUICustomButton? m_button = null;
        private static UISprite? m_sprite = null;
        private static UITextureAtlas? m_atlas = null;

        private static UnifiedUIButton? s_instance = null;

        // ----------------------------------------------------------------------------------------
        public static UnifiedUIButton Instance
        {
            get
            {
                CDebug.Log("Instance");
                if (s_instance == null)
                {
                    s_instance = new UnifiedUIButton();
                }

                return s_instance;
            }
        }

        public static bool Exists
        {
            get
            {
                return s_instance != null;
            }
        }

        public bool HasBeenAdded()
        {
            return m_button != null;
        }

        public static void Add()
        {
            if (m_button is null && ModSettings.GetSettings().AddUnifiedUIButton && DependencyUtils.IsUnifiedUIRunning())
            {
                Texture2D? icon = SleepyCommon.TextureResources.LoadDllResource(Assembly.GetExecutingAssembly(), "BusImageInverted48x48.png", 32, 32);
                if (icon is null)
                {
                    CDebug.Log("Failed to load icon from resources");
                    return;
                }

                m_button = UUIHelpers.RegisterCustomButton(TransportToolMod.Instance.ModName, null, TransportToolMod.Instance.Name, icon, OnToggle, null, null);

                // Overlay a sprite over the button so we can change the icon
                if (m_atlas == null)
                {
                    m_atlas = PublicTransportInstance.LoadResources();
                }

                m_sprite = m_button.Button.AddUIComponent<UISprite>();
                m_sprite.atlas = m_atlas;
                m_sprite.relativePosition = default;
                m_sprite.size = m_button.Button.size;
                m_sprite.spriteName = "BusImageInverted48x48";
            }
        }

        public static void Remove()
        {
            if (m_button is not null)
            {
                UUIHelpers.Destroy(m_button.Button);
                m_button = null;
            }
        }

        public static void OnToggle(bool bToggle)
        {
            MainPanel.TogglePanel();
        }

        public static void OnUnifiedToolbarButtonChanged()
        {
            if (ModSettings.GetSettings().AddUnifiedUIButton)
            {
                Add();
            }
            else
            {
                Remove();
            }
        }

        public static void SetIcon(string sIconName)
        {
            if (m_sprite != null && m_atlas != null)
            {
                m_sprite.spriteName = sIconName;
            }
        }

        public static void ShowWarningLevel(LineIssue.IssueLevel eLevel)
        {
            if (eLevel == LineIssue.IssueLevel.ISSUE_WARNING)
            {
                SetIcon("BusImageWarning");
            }
            else if (eLevel == LineIssue.IssueLevel.ISSUE_INFORMATION)
            {
                SetIcon("BusInformationIcon");
            }
            else
            {
                SetIcon("BusImageInverted48x48");
            }
        }

        public void Enable()
        {
            if (m_button != null && !m_button.Button.enabled)
            {
                m_button.Button.SimulateClick();
            }
        }

        public void Disable()
        {
            if (m_button != null && m_button.Button.enabled)
            {
                m_button.Button.SimulateClick();
            }
        }
    }
}