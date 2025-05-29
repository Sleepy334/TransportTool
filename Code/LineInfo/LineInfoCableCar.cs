using ColossalFramework;
using ColossalFramework.UI;
using System.Collections.Generic;
using static TransportInfo;

namespace PublicTransportInfo
{
    public class LineInfoCableCar : LineInfoBase
    {
        public int m_iLineNumber;
        private ushort m_usFirstStopBuildingId = 0;
        private List<ushort>? m_buildings = null;

        public LineInfoCableCar(ushort usBuildingId) : base()
        {
            m_usFirstStopBuildingId = usBuildingId;
            m_buildings = new List<ushort>();
        }

        public override TransportType GetTransportType()
        {
            return TransportType.CableCar;
        }

        public override int GetLineId()
        {
            return m_iLineNumber;
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

        public override ushort GetNextStop(ushort stopId, out int stopNumber)
        {
            List<ushort> stops = GetStops();
            
            if (stops.Count > 0) 
            {
                int iIndex = stops.IndexOf(stopId);
                if (iIndex != -1)
                {
                    int next = (iIndex + 1) % stops.Count;
                    stopNumber = next + 1;
                    return stops[next];
                }
            }

            stopNumber = 0;
            return stopId;
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

        public override bool IsWorldInfoPanelVisible()
        {
            return TransportManagerUtils.IsCityServiceWorldInfoPanelVisible();
        }

        public override void ShowStopWorldInfoPanel(ushort stopNodeId)
        {
            ushort usBuildingId = NetNode.FindOwnerBuilding(stopNodeId, 64f);
            if (usBuildingId != 0)
            {
                Building building = BuildingManager.instance.m_buildings.m_buffer[usBuildingId];

                WorldInfoPanel.Show<CityServiceWorldInfoPanel>(building.m_position, new InstanceID { Building = usBuildingId });

                // Bring to front
                MainPanel.Instance.SendToBack();
            } 
        }
    }
}
