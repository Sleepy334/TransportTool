using ICities;
using PublicTransportInfo.UnifiedUI;
using UnityEngine;

namespace PublicTransportInfo
{
    public class PublicTransportThreading : ThreadingExtensionBase
    {
        private bool _processed = false;
        private int m_SimulationTicks = 0;

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (PublicTransportInstance.s_mainPanel == null)
            {
                PublicTransportInstance.Create();
            }

            // Unified UI also handles the Keyboard shortcut for us so don't respond here if we are using it.
            if (!UnifiedUITool.HasButtonBeenAdded())
            {
                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKey(KeyCode.I))
                {
                    Debug.Log("PublicTransportInfo::OnUpdate");

                    // cancel if they key input was already processed in a previous frame
                    if (_processed) return;

                    _processed = true;

                    PublicTransportInstance.ShowPanel();
                }
                else
                {
                    // not both keys pressed: Reset processed state
                    _processed = false;
                }
            }
        }

        public override void OnAfterSimulationTick()
        {
            if (PublicTransportInstance.s_mainPanel != null && PublicTransportInstance.s_mainPanel.isVisible)
            {
                m_SimulationTicks++;
                if (m_SimulationTicks > 200)
                {
                    m_SimulationTicks = 0;
                    PublicTransportInstance.UpdatePanel();
                }
            }
        }
    }
}
