using ICities;
using System.Diagnostics;
using UnityEngine;

namespace PublicTransportInfo
{
    public class LineIssueThread : ThreadingExtensionBase
    {
        const int iUPDATE_RATE = 2000;
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

            // Update panel
            if (SimulationManager.instance.SimulationPaused)
            {
                if (m_watch.IsRunning)
                {
                    m_watch.Stop();
                    m_LastElapsedTime = 0;
                }
            }
            else
            {
                if (!m_watch.IsRunning)
                {
                    m_watch.Start();
                    m_LastElapsedTime = m_watch.ElapsedMilliseconds;
                }

                if (LineIssueManager.Instance != null &&  m_watch.ElapsedMilliseconds - m_LastElapsedTime > iUPDATE_RATE)
                {
#if DEBUG
                    long lStartTime = m_watch.ElapsedMilliseconds;
#endif
                    // Need to update vehicle progress every time.
                    LineIssueManager.Instance.UpdateData();
                    m_LastElapsedTime = m_watch.ElapsedMilliseconds;

#if DEBUG
                    long lStopTime = m_watch.ElapsedMilliseconds;
                    //Debug.Log("Execution Time: " + (lStopTime - lStartTime) + "ms");
#endif
                }
            }
        }
    }
}
