using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using static TransportInfo;

namespace PublicTransportInfo
{
    public class LineInfoCableCar : LineInfoBase
    {
        public int m_iLineNumber;
        private ushort m_usFirstStopBuildingId = 0;
        private List<ushort> m_buildings = null;

        public LineInfoCableCar(ushort usBuildingId) : base()
        {
            m_usFirstStopBuildingId = usBuildingId;
            m_buildings = new List<ushort>();
        }

        public override TransportType GetTransportType()
        {
            return TransportType.CableCar;
        }

        public override string GetLineName()
        {
            return "Cable Car Line " + m_iLineNumber;
        }

        public override int GetStopCount()
        {
            return GetStops().Count;
        }

        public override List<ushort> GetStops()
        {
            return TransportManagerUtils.GetStationStops(m_usFirstStopBuildingId);
        }

        public override LineIssueDetector? GetLineIssueDetector()
        {
            return null;
        }

        protected override void LoadVehicleInfo()
        {
            m_iPassengers = 0;
            m_iCapacity = 0;
            m_iVehicleCount = 0;
            m_vehicleDatas.Clear();
            m_buildings.Clear();

            // Load passengers and capacity
            List<ushort> stops = GetStops();
            foreach (ushort usStop in stops)
            {
                ushort usBuildingId = NetNode.FindOwnerBuilding(usStop, 64f);
                if (usBuildingId != 0)
                {
                    if (!m_buildings.Contains(usBuildingId))
                    {
                        m_buildings.Add(usBuildingId);

                        Building oCableBarBuilding = Singleton<BuildingManager>.instance.m_buildings.m_buffer[usBuildingId];
                        ushort usVehicleId = oCableBarBuilding.m_ownVehicles;
                        while (usVehicleId != 0)
                        {
                            int iCapacity;
                            int iPassengerCount = GetVehiclePassengerCount(usVehicleId, out iCapacity);
                            m_iPassengers += iPassengerCount;
                            m_iCapacity += iCapacity;
                            m_iVehicleCount++;

                            // Store vehicle data
                            Vehicle oVehicle = VehicleManager.instance.m_vehicles.m_buffer[usVehicleId];
                            VehicleData oData = new VehicleData();
                            oData.m_usVehicleId = usVehicleId;
                            oData.m_iPassengers = iPassengerCount;
                            oData.m_iCapacity = iCapacity;
                            oData.m_iNextStop = stops.IndexOf(oVehicle.m_targetBuilding) + 1;
                            m_vehicleDatas.Add(oData);

                            // Update for next car
                            usVehicleId = oVehicle.m_nextOwnVehicle;
                        }
                    }
                }
            }
        }

        public override void ShowBusiestStop()
        {
            ushort usBuildingId = NetNode.FindOwnerBuilding(m_usBusiestStopId, 64f);
            if (usBuildingId != 0)
            {
                InstanceID oInstanceId = new InstanceID { Building = usBuildingId };
                Vector3 oStopPosition = Singleton<NetManager>.instance.m_nodes.m_buffer[m_usBusiestStopId].m_position;
                ModSettings oSettings = ModSettings.GetSettings();
                PublicTransportVehicleButton.cameraController.SetTarget(oInstanceId, oStopPosition, oSettings.ZoomInOnTarget);

                WorldInfoPanel.Show<CityServiceWorldInfoPanel>(oStopPosition, oInstanceId);
            }
        }

        public override string GetPassengersTooltip()
        {
            BuildingManager instance = Singleton<BuildingManager>.instance;

            string sTooltip = "";
            if (m_buildings.Count > 0)
            {
                int iBuilding = 1;
                foreach (ushort usBuildingId in m_buildings)
                {
                    Building building2 = instance.m_buildings.m_buffer[usBuildingId];
                    BuildingInfo info = building2.Info;
                    BuildingAI buildingAI = info.m_buildingAI;
                    int iWeeklyPassengers = ((CableCarStationAI)buildingAI).GetPassengerCount(usBuildingId, ref building2);
                    if (sTooltip.Length > 0)
                    {
                        sTooltip += "\r\n";
                    }
                    sTooltip += "Building " + iBuilding + " | Weekly Passengers: " + iWeeklyPassengers;
                    iBuilding++;
                }
            }
            
            return sTooltip;
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
            else
            {
                return x.m_iPassengers - y.m_iPassengers;
            }
        }
    }
}
