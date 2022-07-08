using ICities;
using System.Diagnostics;
using UnityEngine;

namespace PublicTransportInfo
{
    public class PublicTransportThreading : ThreadingExtensionBase
    {
        const int iUPDATE_RATE = 2000;

        private bool _processed = false;
        private long m_LastElapsedTime = 0;
        private Stopwatch? m_watch = null;

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (!PublicTransportLoader.isGameLoaded)
            {
                return;
            }

            if (m_watch == null)
            {
                m_watch = new Stopwatch();
            }

            // Unified UI also handles the Main panel Keyboard shortcut for us so don't respond here if we are using it.
            if (!UnifiedUITool.HasButtonBeenAdded() && ModSettings.Hotkey.IsPressed())
            {
                // cancel if they key input was already processed in a previous frame
                if (_processed)
                {
                    return;
                }
                _processed = true;

                PublicTransportInstance.ToggleMainPanel(); 
            }
            else if (ModSettings.LineIssueHotkey.IsPressed())
            {
                // cancel if they key input was already processed in a previous frame
                if (_processed)
                {
                    return;
                }
                _processed = true;

                PublicTransportInstance.ToggleLineIssuePanel();
            } 
            else
            {
                _processed = false;
            }

            // Update panel
            if (SimulationManager.instance.SimulationPaused)
            {
                if (m_watch.IsRunning)
                {
                    Debug.Log("Simulation stopped");
                    m_watch.Stop();
                    m_LastElapsedTime = 0;
                }
            } 
            else 
            {    
                if (!m_watch.IsRunning)
                {
                    Debug.Log("Simulation started");
                    m_watch.Start();
                    m_LastElapsedTime = m_watch.ElapsedMilliseconds;
                }

                if (m_watch.ElapsedMilliseconds - m_LastElapsedTime > iUPDATE_RATE)
                {
#if DEBUG
                    long lStartTime = m_watch.ElapsedMilliseconds;
#endif
                    // Only update panel if visible
                    if (PublicTransportInstance.s_mainPanel != null && PublicTransportInstance.s_mainPanel.isVisible)
                    {
                        PublicTransportInstance.UpdateMainPanel();
                    }
                    if (PublicTransportInstance.s_LineIssuePanel2 != null && PublicTransportInstance.s_LineIssuePanel2.isVisible)
                    {
                        PublicTransportInstance.UpdateLineIssuePanel();
                    }
                    m_LastElapsedTime = m_watch.ElapsedMilliseconds;
 #if DEBUG
                    long lStopTime = m_watch.ElapsedMilliseconds;
                    Debug.Log("Execution Time: " + (lStopTime - lStartTime) + "ms");
#endif
                }
            }
        }
    }
}
