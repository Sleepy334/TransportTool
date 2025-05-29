using ColossalFramework;
using ColossalFramework.UI;
using SleepyCommon;
using System;
using System.Collections.Generic;
using UnityEngine;
using static TransportInfo;

namespace PublicTransportInfo
{
    public class TransportManagerUtils
    {
        public static string GetSafeLineName(TransportType type, int iLineId)
        {
            if (type == TransportType.CableCar)
            {
                return "Cable Car Line " + iLineId;
            }
            else
            {
                return GetSafeLineName(iLineId);
            }
        }

        public static string GetSafeLineName(int iLineId)
        {
            TransportLine oLine = TransportManager.instance.m_lines.m_buffer[iLineId];
            if ((oLine.m_flags & TransportLine.Flags.CustomName) == TransportLine.Flags.CustomName)
            {
                InstanceID oInstanceId = new InstanceID { TransportLine = (ushort)iLineId };
                return InstanceManager.instance.GetName(oInstanceId);
            }
            else
            {
                return PublicTransportTypeUtils.GetDefaultLineTypeName(oLine.Info.m_transportType) + " Line " + oLine.m_lineNumber;
            }
        }

        public static Color32 GetSafeLineColor(int iLineId)
        {
            TransportLine oLine = TransportManager.instance.m_lines.m_buffer[iLineId];
            if ((oLine.m_flags & TransportLine.Flags.CustomColor) == TransportLine.Flags.CustomColor)
            {
                return oLine.m_color;
            }
            else
            {
                return PublicTransportTypeUtils.GetDefaultLineColor(oLine.Info.m_transportType);
            }
        }

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
            CDebug.Log("ChangeLineVisibility" + m_lineID);
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

        public static int CalculatePassengerCount(ushort stop, TransportInfo.TransportType transportType, out int iBored)
        {
            int iBoredThreshold = ModSettings.GetSettings().BoredThreshold;
            iBored = 0;

            if (stop == 0)
            {
                return 0;
            }

            ushort nextStop = TransportLine.GetNextStop(stop);
            if (nextStop == 0)
            {
                return 0;
            }

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
                            CDebug.Log("Invalid list detected!\n" + Environment.StackTrace);
                            break;
                        }
                    }
                }
            }

            return num6;
        }

        public static ushort FindNextCableCarStop(NetNode oNode, ushort prevStop)
        {
            NetManager instance = Singleton<NetManager>.instance;
            for (int i = 0; i < 8; i++)
            {
                ushort segment = instance.m_nodes.m_buffer[prevStop].GetSegment(i);
                if (segment != 0 && instance.m_segments.m_buffer[segment].m_startNode == prevStop)
                {
                    return instance.m_segments.m_buffer[segment].m_endNode;
                }
            }

            ushort num = NetNode.FindOwnerBuilding(prevStop, 64f);
            if (num != 0)
            {
                ushort num2 = Singleton<BuildingManager>.instance.m_buildings.m_buffer[num].m_netNode;
                int num3 = 0;
                while (num2 != 0)
                {
                    if (num2 != prevStop)
                    {
                        NetInfo info = instance.m_nodes.m_buffer[num2].Info;
                        if (info.m_class.m_layer == ItemClass.Layer.PublicTransport && 
                            info.m_class.m_service == ItemClass.Service.PublicTransport && 
                            info.m_class.m_subService == ItemClass.SubService.PublicTransportCableCar)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                ushort segment2 = instance.m_nodes.m_buffer[num2].GetSegment(j);
                                if (segment2 != 0 && instance.m_segments.m_buffer[segment2].m_startNode == num2)
                                {
                                    return num2;
                                }
                            }
                        }
                    }

                    num2 = instance.m_nodes.m_buffer[num2].m_nextBuildingNode;
                    if (++num3 > 32768)
                    {
                        CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                        break;
                    }
                }
            }

            return 0;
        }

        public static List<ushort> GetStationStops(ushort usBuildingId)
        {
            List<ushort> stops = new List<ushort>();

            NetManager instance = Singleton<NetManager>.instance;
            ushort usStationStop = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)usBuildingId].m_netNode;
            ushort usOrigninalStop = usStationStop;
            int iLoop = 0;

            while (usStationStop != 0 && iLoop < 100)
            {
                stops.Add(usStationStop);
                NetNode oStationNode = instance.m_nodes.m_buffer[(int)usStationStop];
                usStationStop = TransportManagerUtils.FindNextCableCarStop(oStationNode, usStationStop);
                iLoop++;

                // If we loop back to the starting stop then stop looking.
                if (usStationStop == usOrigninalStop)
                {
                    break;
                }
            }

            return stops;
        }

        public static bool IsFirstStation(ushort usBuildingId)
        {
            NetManager instance = Singleton<NetManager>.instance;
            ushort usStationStop = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)usBuildingId].m_netNode;
            NetNode oStationNode = instance.m_nodes.m_buffer[(int)usStationStop];

            for (int i = 0; i < 8; ++i)
            {
                ushort usSegment = oStationNode.GetSegment(i);
                if (usSegment != 0)
                {
                    NetSegment oSegment = instance.m_segments.m_buffer[usSegment];
                    if (oSegment.m_startNode == usStationStop)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void GetStopProgress(ushort targetID, ushort usFirstStop, out float min, out float max, out float total)
        {
            min = 0f;
            max = 0f;
            total = 0f;
            if (usFirstStop == 0)
            {
                return;
            }

            NetManager instance = Singleton<NetManager>.instance;
            ushort num = usFirstStop;
            int num2 = 0;
            while (num != 0)
            {
                ushort num3 = 0;
                for (int i = 0; i < 8; i++)
                {
                    ushort segment = instance.m_nodes.m_buffer[num].GetSegment(i);
                    if (segment != 0 && instance.m_segments.m_buffer[segment].m_startNode == num)
                    {
                        num3 = instance.m_segments.m_buffer[segment].m_endNode;
                        if (num3 == targetID)
                        {
                            min = total;
                        }

                        total += instance.m_segments.m_buffer[segment].m_averageLength;
                        if (num3 == targetID)
                        {
                            max = total;
                        }

                        break;
                    }
                }

                num = num3;
                if (num == usFirstStop)
                {
                    break;
                }

                if (++num2 >= 32768)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
        }

        public static bool GetProgressStatus(ushort vehicleID, ushort usFirstStop, ref Vehicle data, out float current, out float max)
        {
            ushort transportLine = data.m_transportLine;
            ushort targetBuilding = data.m_targetBuilding;
            if ((int)transportLine != 0 && (int)targetBuilding != 0)
            {
                float min;
                float max1;
                float total;
                Singleton<TransportManager>.instance.m_lines.m_buffer[(int)transportLine]
                    .GetStopProgress(targetBuilding, out min, out max1, out total);
                uint path = data.m_path;
                bool valid;
                if ((int)path == 0 || (data.m_flags & Vehicle.Flags.WaitingPath) !=
                    ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
                        Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 |
                        Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped |
                        Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed |
                        Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing |
                        Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack |
                        Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting |
                        Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel |
                        Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic |
                        Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding |
                        Vehicle.Flags.LeftHandDrive))
                {
                    current = min;
                    valid = false;
                }
                else
                {
                    current = BusAI.GetPathProgress(path, (int)data.m_pathPositionIndex, min, max1, out valid);
                }

                max = total;
                return valid;
            }
            current = 0.0f;
            max = 0.0f;
            return true;
        }

        public static List<ushort> GetCompleteTransportLines()
        {
            uint iSize = TransportManager.instance.m_lines.m_size;

            // Build a list of lin id's
            List<ushort> aLines = new List<ushort>();
            for (ushort i = 0; i < iSize; i++)
            {
                TransportLine oLine = TransportManager.instance.m_lines.m_buffer[i];
                if (oLine.Complete)
                {
                    aLines.Add(i);
                }
            }

            return aLines;
        }

        public static string GetSlowOrStuckTooltipText(ushort usVehicleId, int iDays)
        {
            string sName = Singleton<VehicleManager>.instance.GetVehicleName(usVehicleId);
            return "Vehicle " + sName + " (id:" + usVehicleId + ") has not moved for " + iDays + " in-game days.";
        }

        public static UIComponent? GetPublicTransportWorldInfoPanel()
        {
            return UIView.library.Get(typeof(PublicTransportWorldInfoPanel).Name);
        }

        public static UIComponent? GetCityServiceWorldInfoPanel()
        {
            return UIView.library.Get(typeof(CityServiceWorldInfoPanel).Name);
        }

        public static bool IsPublicTransportWorldInfoPanelVisible()
        {
            UIComponent? panel = GetPublicTransportWorldInfoPanel();
            if (panel != null)
            {
                return panel.isVisible;
            }
            return false;
        }

        public static bool IsCityServiceWorldInfoPanelVisible()
        {
            UIComponent? panel = GetCityServiceWorldInfoPanel();
            if (panel != null)
            {
                return panel.isVisible;
            }
            return false;
        }
    }
}
