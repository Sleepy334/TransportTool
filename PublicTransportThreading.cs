﻿using ICities;
using System.Diagnostics;
using UnityEngine;

namespace PublicTransportInfo
{
    public class PublicTransportThreading : ThreadingExtensionBase
    {
        const int iUPDATE_RATE = 2000;

        private bool _processed = false;
        private long m_LastElapsedTime = 0;
        private Stopwatch m_watch = null;

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (PublicTransportInstance.s_mainPanel == null)
            {
                PublicTransportInstance.Create();
            }

            if (m_watch == null)
            {
                m_watch = new Stopwatch();
            }

            // Unified UI also handles the Keyboard shortcut for us so don't respond here if we are using it.
            if (!UnifiedUITool.HasButtonBeenAdded())
            {
                if (ModSettings.Hotkey.IsPressed())
                {
                    Debug.Log("OnUpdate");

                    // cancel if they key input was already processed in a previous frame
                    if (_processed) return;

                    _processed = true;

                    PublicTransportInstance.TogglePanel();
                }
                else
                {
                    // not both keys pressed: Reset processed state
                    _processed = false;
                }
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
                    long lStartTime = m_watch.ElapsedMilliseconds;

                    // Need to update vehicle progress every time.
                    PublicTransportInstance.UpdateVehicleProgress();
                    
                    // Only update panel if visible
                    if (PublicTransportInstance.s_mainPanel != null && PublicTransportInstance.s_mainPanel.isVisible)
                    {
                        PublicTransportInstance.UpdatePanel();
                    }
                    
                    m_LastElapsedTime = m_watch.ElapsedMilliseconds;

                    long lStopTime = m_watch.ElapsedMilliseconds;
                    //Debug.Log("Execution Time: " + (lStopTime - lStartTime) + "ms");
                }
            }
        }
    }
}