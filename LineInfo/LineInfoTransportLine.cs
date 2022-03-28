using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using static TransportInfo;

namespace PublicTransportInfo
{
    public class LineInfoTransportLine :  LineInfoBase
    {
        private TransportType m_eType;
        private int m_iStops;

        public LineInfoTransportLine(TransportType transportType) : base()
        {
            m_eType = transportType;
            m_iStops = 0;
        }

        public override TransportType GetTransportType()
        {
            return m_eType;
        }
        public override int GetStopCount()
        {
            return m_iStops;
        }
        public override List<ushort> GetStops()
        {
            List<ushort> list = new List<ushort>();
            TransportLine oLine = TransportManager.instance.m_lines.m_buffer[m_iLineId];
            m_iStops = oLine.CountStops((ushort)m_iLineId);
            for (int i = 0; i < m_iStops; i++)
            {
                list.Add(oLine.GetStop(i));
            }
            return list;
        }

        public override VehicleProgressLine? GetVehicleProgress()
        {
            LineIssueManager oManager = PublicTransportInstance.GetLineIssueManager();
            if (oManager != null && oManager.ContainsLine((ushort)m_iLineId))
            {
                return oManager.GetVehicleProgressForLine((ushort)m_iLineId);
            }
            else
            {
                return null;
            }
            
        }

        public override void UpdateInfo()
        {
            TransportLine oLine = TransportManager.instance.m_lines.m_buffer[m_iLineId];
            m_sName = GetSafeLineName(m_iLineId, oLine);
            m_color = GetSafeLineColor(m_iLineId, oLine);
            base.UpdateInfo();
        }

        protected override void LoadVehicleInfo()
        {
            m_vehicleDatas.Clear();
            m_iPassengers = 0;
            m_iCapacity = 0;

            List<ushort> stops = GetStops();
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
                oData.m_iNextStop = stops.IndexOf(oVehicle.m_targetBuilding) + 1;
                m_vehicleDatas.Add(oData);
            }
        }

        public override void ShowBusiestStop()
        {
            // Turn on public transport mode so you can see the lines
            Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.Transport, InfoManager.SubInfoMode.Default);
            UIView.library.Hide("PublicTransportInfoViewPanel");

            // Zoom in on busiest stop
            InstanceID oInstanceId = new InstanceID { TransportLine = (ushort)m_iLineId };
            Vector3 oStopPosition = Singleton<NetManager>.instance.m_nodes.m_buffer[m_usBusiestStopId].m_position;
            PublicTransportVehicleButton.cameraController.SetTarget(oInstanceId, oStopPosition, true);

            // Open transport line panel
            WorldInfoPanel.Show<PublicTransportWorldInfoPanel>(oStopPosition, oInstanceId);
        }

        public override string GetPassengersTooltip()
        {
            TransportLine oLine = Singleton<TransportManager>.instance.m_lines.m_buffer[m_iLineId];
            int iTouristCount = (int)oLine.m_passengers.m_touristPassengers.m_averageCount;
            int iResidentCount = (int)oLine.m_passengers.m_residentPassengers.m_averageCount;
            int total = iTouristCount + iResidentCount;
            return "Weekly passengers: " + total + "\r\nResidents: " + iResidentCount + "\r\nTourists: " + iTouristCount;
        }
    }
}
