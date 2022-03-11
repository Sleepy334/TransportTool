using ColossalFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public class LineInfo : IComparable<LineInfo>
    {
        public TransportInfo.TransportType m_eType;
        public int m_iLineId;
        public string m_sName;
        public Color32 m_color;
        public int m_iStops;
        public int m_iPassengers;
        public int m_iCapacity;
        public int m_iVehicleCount;
        public int m_iWaiting;
        public int m_iBusiest;
        public int m_iBored;
        public ushort m_usBusiestStopId;
        public int m_iBusiestStopNumber;
        public List<ushort> m_usCableCarStops = null;
        public VehicleProgressLine m_aVehicleProgress = null;

        public static int ComparatorBase(int iResult, LineInfo oLine1, LineInfo oLine2)
        {
            if (iResult == 0)
            {
                iResult = oLine2.m_iLineId - oLine1.m_iLineId;
            }
            return iResult;
        }

        public static int ComparatorName(LineInfo oLine1, LineInfo oLine2) 
        {
            return ComparatorBase(oLine1.m_sName.CompareTo(oLine2.m_sName), oLine1, oLine2);
        }

        public static int ComparatorStops(LineInfo oLine1, LineInfo oLine2)
        {
            return ComparatorBase(oLine1.m_iStops.CompareTo(oLine2.m_iStops), oLine1, oLine2);
        }

        public static int ComparatorPassengers(LineInfo oLine1, LineInfo oLine2)
        {
            return ComparatorBase(oLine1.m_iPassengers.CompareTo(oLine2.m_iPassengers), oLine1, oLine2);
        }

        public static int ComparatorWaiting(LineInfo oLine1, LineInfo oLine2)
        {
            return ComparatorBase(oLine1.m_iWaiting.CompareTo(oLine2.m_iWaiting), oLine1, oLine2);
        }

        public static int ComparatorBusiest(LineInfo oLine1, LineInfo oLine2)
        {
            return ComparatorBase(oLine1.m_iBusiest.CompareTo(oLine2.m_iBusiest), oLine1, oLine2);
        }

        public static int ComparatorBored(LineInfo oLine1, LineInfo oLine2)
        {
            return ComparatorBase(oLine1.m_iBored.CompareTo(oLine2.m_iBored), oLine1, oLine2);
        }

        public LineInfo()
        {
            m_sName = "";
            m_iStops = 0;
            m_iPassengers = 0;
            m_iCapacity = 0;
            m_iVehicleCount = 0;
            m_iWaiting = 0;
            m_iBusiest = 0;
            m_iBored = 0;
            m_usBusiestStopId = 0;
            m_iBusiestStopNumber = 0;
        }

        public void LoadInfo(int iLineId, TransportLine oLine)
        {
            m_eType = oLine.Info.m_transportType;
            m_iLineId = iLineId;
            m_sName = GetSafeLineName(iLineId, oLine);
            m_color = GetSafeLineColor(iLineId, oLine);
            LoadLineInfo(iLineId, oLine);
            LoadVehicleInfo(iLineId);
            LoadVehicleProgress();
        }

        public void UpdateInfo()
        {
            if (m_eType == TransportInfo.TransportType.CableCar)
            {
                LoadLineInfoCableCar();
                LoadVehicleInfoCableCar();

                // Cable cars dont have lines so harder to track vehicle progress
                // Can't really get stuck anyway so no need.
            } 
            else
            {
                TransportLine oLine = TransportManager.instance.m_lines.m_buffer[m_iLineId];
                m_sName = GetSafeLineName(m_iLineId, oLine);
                m_color = GetSafeLineColor(m_iLineId, oLine);
                LoadLineInfo(m_iLineId, oLine);
                LoadVehicleInfo(m_iLineId);

             
            }
        }

        public static Comparison<LineInfo> GetComparator(ListViewRowComparer.Columns eSortColumn)
        {
            switch (eSortColumn)
            {
                case ListViewRowComparer.Columns.COLUMN_NAME:
                    return LineInfo.ComparatorName;
                case ListViewRowComparer.Columns.COLUMN_STOPS:
                    return LineInfo.ComparatorStops;
                case ListViewRowComparer.Columns.COLUMN_PASSENGERS:
                    return LineInfo.ComparatorPassengers;
                case ListViewRowComparer.Columns.COLUMN_WAITING:
                    return LineInfo.ComparatorWaiting;
                case ListViewRowComparer.Columns.COLUMN_BUSIEST:
                    return LineInfo.ComparatorBusiest;
                case ListViewRowComparer.Columns.COLUMN_BORED:
                    return LineInfo.ComparatorBored;
            }

            return LineInfo.ComparatorName;
        }

        public static int CompareTo(ListViewRowComparer.Columns eSortColumn, LineInfo oFirst, LineInfo oSecond)
        {
            Comparison<LineInfo> SortComparison = GetComparator(eSortColumn);
            return SortComparison(oFirst, oSecond);
        }

        public int CompareTo(LineInfo obj)
        {
            return ComparatorName(this, obj);
        }

        private void LoadLineInfo(int iLineId, TransportLine oLine)
        {
            m_iBusiest = 0;
            m_iWaiting = 0;
            m_iBored = 0;
            m_usBusiestStopId = 0;
            m_iBusiestStopNumber = 0;

            m_iStops = oLine.CountStops((ushort)iLineId);
            for (int i = 0; i < m_iStops; i++)
            {
                ushort usStop = oLine.GetStop(i);
                int iBored;
                int iStopPassengerCount = TransportManagerUtils.CalculatePassengerCount(usStop, m_eType, out iBored);
                if (iStopPassengerCount >= m_iBusiest)
                {
                    m_iBusiest = iStopPassengerCount;
                    m_usBusiestStopId = usStop;
                    m_iBusiestStopNumber = i+1;
                }

                m_iBored += iBored;
                m_iWaiting += iStopPassengerCount;
            }
        }

        private void LoadLineInfoCableCar()
        {
            m_iBusiest = 0;
            m_iWaiting = 0;
            m_iBored = 0;
            m_usBusiestStopId = 0;
            m_iBusiestStopNumber = 0;

            if (m_usCableCarStops != null)
            {
                m_iStops = m_usCableCarStops.Count;
                for (int i = 0; i < m_iStops; i++)
                {
                    ushort usStop = m_usCableCarStops[i];
                    int iBored;
                    int iStopPassengerCount = TransportManagerUtils.CalculatePassengerCount(usStop, TransportInfo.TransportType.CableCar, out iBored);
                    if (iStopPassengerCount > m_iBusiest)
                    {
                        m_iBusiest = iStopPassengerCount;
                        m_usBusiestStopId = usStop;
                        m_iBusiestStopNumber = i + 1;
                    }

                    m_iBored += iBored;
                    m_iWaiting += iStopPassengerCount;
                }
            }
        }

        public static string GetSafeLineName(int iLineId, TransportLine oLine)
        {
            if ((oLine.m_flags & TransportLine.Flags.CustomName) == TransportLine.Flags.CustomName)
            {
                InstanceID oInstanceId = new InstanceID { TransportLine = (ushort)iLineId };
                return InstanceManager.instance.GetName(oInstanceId);
            }
            else
            {
                TransportInfo.TransportType oType = oLine.Info.m_transportType;
                return PublicTransportTypeUtils.GetDefaultLineTypeName(oType) + " Line " + oLine.m_lineNumber;
            }
        }

        public static Color32 GetSafeLineColor(int iLineId, TransportLine oLine)
        {
            if ((oLine.m_flags & TransportLine.Flags.CustomColor) == TransportLine.Flags.CustomColor)
            {
                return oLine.m_color;
            }
            else
            {
                return PublicTransportTypeUtils.GetDefaultLineColor(oLine.Info.m_transportType);
            }
        }

        private void LoadVehicleInfo(int iLineId)
        {
            m_iPassengers = 0;
            m_iCapacity = 0;

            int iCapacity;
            TransportLine oLine = TransportManager.instance.m_lines.m_buffer[iLineId];
            m_iVehicleCount = oLine.CountVehicles((ushort)iLineId);
            for (int i = 0; i < m_iVehicleCount; ++i)
            {
                ushort usVehicleId = oLine.GetVehicle(i);
                int iPassengers = GetVehiclePassengerCount(usVehicleId, out iCapacity);
                m_iCapacity += iCapacity;
                m_iPassengers += iPassengers;
            }
        }

        public void LoadVehicleInfoCableCar()
        {
            m_iPassengers = 0;
            m_iCapacity = 0;
            m_iVehicleCount = 0;

            if (m_usCableCarStops != null)
            {
                // Load passengers and capacity
                List<int> oBuildings = new List<int>();
                foreach (ushort usStop in m_usCableCarStops)
                {
                    ushort usBuildingId = NetNode.FindOwnerBuilding(usStop, 64f);
                    if (usBuildingId != 0)
                    {
                        if (!oBuildings.Contains((int)usBuildingId))
                        {
                            oBuildings.Add(usBuildingId);
                            
                            Building oCableBarBuilding = Singleton<BuildingManager>.instance.m_buildings.m_buffer[usBuildingId];
                            ushort usVehicleId = oCableBarBuilding.m_ownVehicles;
                            while (usVehicleId != 0)
                            {
                                int iCapacity;
                                int iPassengerCount = GetVehiclePassengerCount(usVehicleId, out iCapacity);
                                m_iPassengers += iPassengerCount;
                                m_iCapacity += iCapacity;
                                m_iVehicleCount++;

                                Vehicle oVehicle = VehicleManager.instance.m_vehicles.m_buffer[usVehicleId];
                                usVehicleId = oVehicle.m_nextOwnVehicle;
                            }
                        }
                    }
                }
            }
        }

        public void LoadVehicleProgress()
        {
            if (m_eType != TransportInfo.TransportType.CableCar)
            {
                if (PublicTransportInstance.m_vehicleProgress.ContainsLine((ushort)m_iLineId))
                {
                    m_aVehicleProgress = PublicTransportInstance.m_vehicleProgress.GetVehicleProgressForLine((ushort)m_iLineId);
                }
            }    
        }

        private static int GetVehiclePassengerCount(ushort usVehicleId, out int iCapacity)
        {
            int iPassengers = 0;
            iCapacity = 0;

            var VMInstance = VehicleManager.instance;
            Vehicle oVehicle = VMInstance.m_vehicles.m_buffer[usVehicleId];

            // Copied from PublicTransportVehicleWorldInfoPanel.UpdateBindings()
            ushort firstVehicle = oVehicle.GetFirstVehicle(usVehicleId);
            if (firstVehicle > 0)
            {
                oVehicle.Info.m_vehicleAI.GetBufferStatus(firstVehicle, ref VMInstance.m_vehicles.m_buffer[firstVehicle], out string _, out int current, out int max);
                if (max != 0)
                {
                    iPassengers = current;
                    iCapacity = max;
                }
            }
            
            return iPassengers;
        }
    }
}
