using ColossalFramework;
using System;
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

        public static int CalculatePassengerCount(ushort stop, TransportLine oLine, out int iBored)
        {
            int iBoredThreshold = ModSettings.s_iBoredThreshold;
            iBored = 0;
                
            if(stop == 0)
            {
                return 0;
            }

            ushort nextStop = TransportLine.GetNextStop(stop);
            if (nextStop == 0)
            {
                return 0;
            }

            TransportInfo info = oLine.Info;
            TransportInfo.TransportType transportType = info.m_transportType;
            float num = (transportType != 0 && transportType != TransportInfo.TransportType.EvacuationBus && transportType != TransportInfo.TransportType.TouristBus) ? 64f : 32f;
            CitizenManager instance = Singleton<CitizenManager>.instance;
            NetManager instance2 = Singleton<NetManager>.instance;
            Vector3 position = instance2.m_nodes.m_buffer[stop].m_position;
            Vector3 position2 = instance2.m_nodes.m_buffer[nextStop].m_position;
            int num2 = Mathf.Max((int)((position.x - num) / 8f + 1080f), 0);
            int num3 = Mathf.Max((int)((position.z - num) / 8f + 1080f), 0);
            int num4 = Mathf.Min((int)((position.x + num) / 8f + 1080f), 2159);
            int num5 = Mathf.Min((int)((position.z + num) / 8f + 1080f), 2159);
            int num6 = 0;

            for (int i = num3; i <= num5; i++)
            {
                for (int j = num2; j <= num4; j++)
                {
                    ushort num7 = instance.m_citizenGrid[i * 2160 + j];
                    int num8 = 0;
                    while (num7 != 0)
                    {
                        ushort nextGridInstance = instance.m_instances.m_buffer[num7].m_nextGridInstance;
                        if ((instance.m_instances.m_buffer[num7].m_flags & CitizenInstance.Flags.WaitingTransport) != 0)
                        {
                            Vector3 a = instance.m_instances.m_buffer[num7].m_targetPos;
                            float num9 = Vector3.SqrMagnitude(a - position);
                            if (num9 < num * num)
                            {
                                CitizenInfo info2 = instance.m_instances.m_buffer[num7].Info;
                                if (info2.m_citizenAI.TransportArriveAtSource(num7, ref instance.m_instances.m_buffer[num7], position, position2))
                                {
                                    num6++;
                                    byte waitCounter = instance.m_instances.m_buffer[num7].m_waitCounter;
                                    if (waitCounter >= iBoredThreshold)
                                    {
                                        iBored++;
                                    }
                                }
                            }
                        }

                        num7 = nextGridInstance;
                        if (++num8 > 65536)
                        {
                           Debug.Log("Invalid list detected!\n" + Environment.StackTrace);
                            break;
                        }
                    }
                }
            }

            return num6;
        }
    }
}
