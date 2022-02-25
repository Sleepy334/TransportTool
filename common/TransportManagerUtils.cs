using ColossalFramework;
using UnityEngine;

namespace PublicTransportInfo
{
    public class TransportManagerUtils
    {
        public static void HighlightLine(ushort m_lineID)
        {
            if (m_lineID < Singleton<TransportManager>.instance.m_lines.m_buffer.Length && m_lineID != 0)
            {
                Singleton<SimulationManager>.instance.AddAction(() =>
                {
                    Singleton<TransportManager>.instance.m_lines.m_buffer[m_lineID].m_flags |= TransportLine.Flags.Highlighted;
                });
            }
        }

        public static void ClearLineHighlight(ushort m_lineID)
        {
            if (m_lineID < Singleton<TransportManager>.instance.m_lines.m_buffer.Length && m_lineID != 0)
            {
                Singleton<SimulationManager>.instance.AddAction(() =>
                {
                    Singleton<TransportManager>.instance.m_lines.m_buffer[m_lineID].m_flags &= ~TransportLine.Flags.Highlighted;
                });
            }
        }

        public static void ChangeLineVisibility(int m_lineID, bool r)
        {
            Debug.Log("ChangeLineVisibility" + m_lineID);
            if (m_lineID < Singleton<TransportManager>.instance.m_lines.m_buffer.Length && m_lineID != 0)
            {
                Singleton<SimulationManager>.instance.AddAction(() =>
                {
                    if (r)
                    {
                        Singleton<TransportManager>.instance.m_lines.m_buffer[m_lineID].m_flags &= ~TransportLine.Flags.Hidden;
                    }
                    else
                    {
                        Singleton<TransportManager>.instance.m_lines.m_buffer[m_lineID].m_flags |= TransportLine.Flags.Hidden;
                    }
                });
            }
        }

        public static void HideLines()
        {

            uint iSize = TransportManager.instance.m_lines.m_size;
            for (int i = 0; i < iSize; i++)
            {
                TransportLine oLine = TransportManager.instance.m_lines.m_buffer[i];
                if (oLine.Complete)
                {
                    ChangeLineVisibility(i, false);
                }
            }
        }
    }
}
