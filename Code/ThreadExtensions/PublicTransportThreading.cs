using ICities;
using System.Diagnostics;
using UnityEngine;

namespace PublicTransportInfo.ThreadExtensions
{
    public class PublicTransportThreading : ThreadingExtensionBase
    {
        private bool m_processed = false;

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (!PublicTransportLoader.isGameLoaded)
            {
                return;
            }

            // Unified UI also handles the Main panel Keyboard shortcut for us so don't respond here if we are using it.
            if (!UnifiedUITool.HasButtonBeenAdded() && ModSettings.Hotkey.IsPressed())
            {
                // cancel if they key input was already processed in a previous frame
                if (m_processed)
                {
                    return;
                }
                m_processed = true;

                PublicTransportInstance.ToggleMainPanel();
            }
            else if (ModSettings.LineIssueHotkey.IsPressed())
            {
                // cancel if they key input was already processed in a previous frame
                if (m_processed)
                {
                    return;
                }
                m_processed = true;

                PublicTransportInstance.ToggleLineIssuePanel();
            }
            else
            {
                m_processed = false;
            }
        }
    }
}
