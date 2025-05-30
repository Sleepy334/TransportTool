﻿using ColossalFramework;
using ColossalFramework.UI;
using SleepyCommon;
using System;
using System.Collections.Generic;
using static TransportInfo;

namespace PublicTransportInfo
{
    public class LineInfoTransportLine :  LineInfoBase
    {
        public int m_iLineId = 0;
        private TransportType m_eType;
        private List<ushort>? m_stops = null;

        public LineInfoTransportLine(TransportType transportType) : 
            base()
        {
            m_eType = transportType;
        }

        public override TransportType GetTransportType()
        {
            return m_eType;
        }
        public override int GetLineId()
        {
            return m_iLineId;
        }

        public override int GetStopCount()
        {
            return GetStops().Count;
        }

        public override List<ushort> GetStops()
        {
            if (m_stops == null)
            {
                m_stops = new List<ushort>();

                TransportLine oLine = TransportManager.instance.m_lines.m_buffer[m_iLineId];
                if (oLine.m_flags != 0)
                {
                    // Enumerate stops
                    int iLoopCount = 0;
                    ushort firstStop = oLine.m_stops;
                    ushort stop = firstStop;
                    while (stop != 0)
                    {
                        m_stops.Add(stop);

                        stop = TransportLine.GetNextStop(stop);
                        if (stop == firstStop)
                        {
                            break;
                        }

                        if (++iLoopCount >= 32768)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                            break;
                        }
                    }
                }
            }
            
            return m_stops;
        }

        public override LineIssueDetector? GetLineIssueDetector()
        {
            if (LineIssueManager.Instance != null && LineIssueManager.Instance.ContainsLine((ushort)m_iLineId))
            {
                return LineIssueManager.Instance.GetLineIssueDetector((ushort)m_iLineId);
            }
            else
            {
                return null;
            }
        }

        public override void LoadInfo()
        {
            m_color = TransportManagerUtils.GetSafeLineColor(m_iLineId);
            m_stops = null;
            base.LoadInfo();
        }

        protected override void LoadVehicleInfo()
        {
            m_vehicleDatas.Clear();
            m_iPassengers = 0;
            m_iCapacity = 0;

            int iCapacity;
            TransportLine oLine = TransportManager.instance.m_lines.m_buffer[m_iLineId];
            m_iVehicleCount = oLine.CountVehicles((ushort)m_iLineId);
            for (int i = 0; i < m_iVehicleCount; ++i)
            {
                ushort usVehicleId = oLine.GetVehicle(i);
                int iPassengers = GetVehiclePassengerCount(usVehicleId, out iCapacity);
                m_iCapacity += iCapacity;
                m_iPassengers += iPassengers;

                // Store vehicle data
                Vehicle oVehicle = VehicleManager.instance.m_vehicles.m_buffer[usVehicleId];
                VehicleData oData = new VehicleData();
                oData.m_usVehicleId = usVehicleId;
                oData.m_iPassengers = iPassengers;
                oData.m_iCapacity = iCapacity;
                oData.m_iNextStop = GetStops().IndexOf(oVehicle.m_targetBuilding) + 1;
                m_vehicleDatas.Add(oData);
            }
        }
        
        public override string GetPassengersTooltip()
        {
            TransportLine oLine = Singleton<TransportManager>.instance.m_lines.m_buffer[m_iLineId];
            int iTouristCount = (int)oLine.m_passengers.m_touristPassengers.m_averageCount;
            int iResidentCount = (int)oLine.m_passengers.m_residentPassengers.m_averageCount;
            int total = iTouristCount + iResidentCount;
            return Localization.Get("txtWeeklyPassengers") + total + "\r\n" + Localization.Get("txtResidents") + ": " + iResidentCount + "\r\n" + Localization.Get("txtTourists") + ": " + iTouristCount;
        }

        public override bool IsWorldInfoPanelVisible()
        {
            return TransportManagerUtils.IsPublicTransportWorldInfoPanelVisible();
        }

        public override void ShowStopWorldInfoPanel(ushort stopNodeId)
        {
            NetNode node = Singleton<NetManager>.instance.m_nodes.m_buffer[stopNodeId];
            if (node.m_flags != 0)
            {
                WorldInfoPanel.Show<PublicTransportWorldInfoPanel>(node.m_position, new InstanceID { TransportLine = (ushort)GetLineId() });

                // Bring to front
                MainPanel.Instance.SendToBack();
            }
        }

        public override int CompareVehicleDataProgress(VehicleData x, VehicleData y)
        {
            if (x.m_iNextStop < y.m_iNextStop)
            {
                return -1;
            }
            else if (x.m_iNextStop > y.m_iNextStop)
            {
                return 1;
            }
            if (x.m_fDistance < y.m_fDistance)
            {
                return -1;
            }
            else if (x.m_fDistance > y.m_fDistance)
            {
                return 1;
            }
            else
            {
                return x.m_usVehicleId - y.m_usVehicleId;
            }
        }
    }
}
