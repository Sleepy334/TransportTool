using ColossalFramework;
using ColossalFramework.UI;
using PublicTransportInfo.UI.ListView;
using SleepyCommon;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static RenderManager;
using static TransportInfo;

namespace PublicTransportInfo
{
    public abstract class LineInfoBase : IComparable<LineInfoBase>
    {
        public struct VehicleData {
            public ushort m_usVehicleId;
            public int m_iPassengers;
            public int m_iCapacity;
            public int m_iNextStop;
            public float fProgress;
            public float m_fDistance;
        }

        public struct LineData
        {
            public ushort m_usStop; 
            public int m_iStopNumber;
            public int m_iWaitingCount;
            public int m_iBoredCount;
        }

        public Color32 m_color;
        public int m_iPassengers;
        public int m_iCapacity;
        public int m_iVehicleCount;
        public int m_iWaiting;
        public int m_iBusiest;
        public int m_iBored;
        public ushort m_usBusiestStopId;
        public int m_iBusiestStopNumber;
        public LineIssue.IssueLevel m_eLevel;
        public string m_lineIssueTooltip = string.Empty;

        protected List<LineData> m_stopPassengerCount;
        protected List<VehicleData> m_vehicleDatas;

        // ----------------------------------------------------------------------------------------
        public abstract TransportType GetTransportType();
        public abstract int GetLineId();
        public abstract int GetStopCount();
        public abstract List<ushort> GetStops();
        public abstract LineIssueDetector? GetLineIssueDetector();
        protected abstract void LoadVehicleInfo(); 
        public abstract bool IsWorldInfoPanelVisible();
        public abstract void ShowStopWorldInfoPanel(ushort stopNodeId);

        // ----------------------------------------------------------------------------------------
        public LineInfoBase()
        {
            m_iPassengers = 0;
            m_iCapacity = 0;
            m_iVehicleCount = 0;
            m_iWaiting = 0;
            m_iBusiest = 0;
            m_iBored = 0;
            m_usBusiestStopId = 0;
            m_iBusiestStopNumber = 0;
            m_stopPassengerCount = new List<LineData>();
            m_vehicleDatas = new List<VehicleData>();
        }

        public virtual string GetLineName()
        {
            return TransportManagerUtils.GetSafeLineName(GetTransportType(), GetLineId());
        }

        public virtual StopInfo GetStopInfo()
        {
            StopInfo info = new StopInfo();
            info.m_transportType = GetTransportType();
            info.m_currentLineId = GetLineId();
            info.m_stopNumber = m_iBusiestStopNumber;
            info.m_currentStopId = m_usBusiestStopId;
            return info;
        }

        public virtual ushort GetBusiestStop()
        {
            return m_usBusiestStopId;
        }

        public virtual ushort GetNextStop(ushort stopId, out int stopNumber)
        {
            ushort uiNextStop =  TransportLine.GetNextStop(stopId);
            stopNumber = GetStops().IndexOf(uiNextStop) + 1;
            return uiNextStop;
        }

        protected static int GetVehiclePassengerCount(ushort usVehicleId, out int iCapacity)
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

        public virtual void LoadInfo()
        {
            LoadLineInfo();
            LoadVehicleInfo();

            // Get issue level for this line
            m_eLevel = LineIssueManager.Instance.GetLineWarningLevel((ushort) GetLineId(), out m_lineIssueTooltip);
        }

        protected virtual void LoadLineInfo()
        {
            m_iBusiest = 0;
            m_iWaiting = 0;
            m_iBored = 0;
            m_usBusiestStopId = 0;
            m_iBusiestStopNumber = 0;
            m_stopPassengerCount.Clear();

            List<ushort> stops = GetStops();
            int i = 0;
            foreach (ushort usStop in stops)
            {
                int iBored;
                int iStopPassengerCount = TransportManagerUtils.CalculatePassengerCount(usStop, GetTransportType(), out iBored);
                if (iStopPassengerCount >= m_iBusiest)
                {
                    m_iBusiest = iStopPassengerCount;
                    m_usBusiestStopId = usStop;
                    m_iBusiestStopNumber = i + 1;
                }

                LineData oData = new LineData();
                oData.m_usStop = usStop;
                oData.m_iStopNumber = i + 1;
                oData.m_iWaitingCount = iStopPassengerCount;
                oData.m_iBoredCount = iBored;
                m_stopPassengerCount.Add(oData);

                m_iBored += iBored;
                m_iWaiting += iStopPassengerCount;
                i++;
            }
        }

        public delegate int GetLineDataValue(LineData oData);

        public string GetPassengersDescription()
        {
            return m_iPassengers + "/" + m_iCapacity;
        }

        public int GetUsage()
        {
            double dCapacityPercent = 0;
            if (m_vehicleDatas.Count > 0)
            {
                foreach (VehicleData vehicle in m_vehicleDatas)
                {
                    dCapacityPercent += (double)vehicle.m_iPassengers / (double)vehicle.m_iCapacity;
                }

                dCapacityPercent = (dCapacityPercent / (double)m_vehicleDatas.Count) * 100;
            }
            return (int) Math.Round(dCapacityPercent);
        }

        public int GetWaitingCount(LineData oData)
        {
            return oData.m_iWaitingCount;
        }

        public int GetBoredCount(LineData oData)
        {
            return oData.m_iBoredCount;
        }

        public string GetWaitingTooltip()
        {
            return GetLineDataTooltip(Localization.Get("OverviewWaiting"), GetWaitingCount);
        }

        public string GetBoredTooltip()
        {
            return GetLineDataTooltip(Localization.Get("OverviewBored"), GetBoredCount);
        }

        public string GetLineDataTooltip(string sValueNameLocalized, GetLineDataValue hLineDataValue)
        {
            string sTooltip = "";

            int iSTOPS_TO_SHOW = ModSettings.GetSettings().TooltipRowLimit;
            if (iSTOPS_TO_SHOW > 0 && m_stopPassengerCount != null)
            {
                m_stopPassengerCount.Sort((x, y) =>
                {
                    if (hLineDataValue(y) == hLineDataValue(x))
                    {
                        return x.m_iStopNumber - y.m_iStopNumber;
                    }
                    else
                    {
                        return hLineDataValue(y) - hLineDataValue(x);
                    }

                });

                List<string> listValue = new List<string>();
                List<string> listStopNumber = new List<string>();
                for (int i = 0; i < Math.Min(m_stopPassengerCount.Count, iSTOPS_TO_SHOW); i++)
                {
                    LineData oData = m_stopPassengerCount[i];
                    listValue.Add(hLineDataValue(oData).ToString());
                    listStopNumber.Add(oData.m_iStopNumber.ToString());
                }

                listValue = Utils.PadToWidth(listValue, true);
                listStopNumber = Utils.PadToWidth(listStopNumber, true);

                sTooltip += GetLineName() + "\r\n";

                for (int i = 0; i < listValue.Count; i++)
                {
                    if (sTooltip.Length > 0)
                    {
                        sTooltip += "\r\n";
                    }
                    sTooltip += Localization.Get("txtStop") + ": " + listStopNumber[i] + " | " + sValueNameLocalized + ": " + listValue[i];
                }

                if (m_stopPassengerCount.Count > iSTOPS_TO_SHOW)
                {
                    int iValueRemainer = 0;
                    for (int i = iSTOPS_TO_SHOW; i < m_stopPassengerCount.Count; i++)
                    {
                        iValueRemainer += hLineDataValue(m_stopPassengerCount[i]);
                    }
                    sTooltip += "\r\n\r\nAnd " + (m_stopPassengerCount.Count - iSTOPS_TO_SHOW) + " other stops (" + iValueRemainer + ")";
                }
            }

            return sTooltip;
        }

        public void GetStopData(int iStopsToShow, out List<string> listStopNumber, out List<string> listWaiting, out List<string> listBored, out List<string> listDistance)
        {
            listStopNumber = new List<string>();
            listWaiting = new List<string>();
            listBored = new List<string>();
            listDistance = new List<string>();

            if (iStopsToShow > 0 && m_stopPassengerCount != null)
            {
                TransportLine oLine = Singleton<TransportManager>.instance.m_lines.m_buffer[GetLineId()];

                m_stopPassengerCount.Sort((x, y) =>
                {
                    return x.m_iStopNumber - y.m_iStopNumber;
                });

                for (int i = 0; i < Math.Min(m_stopPassengerCount.Count, iStopsToShow); i++)
                {
                    LineData oData = m_stopPassengerCount[i];
                    listStopNumber.Add(oData.m_iStopNumber.ToString());
                    listWaiting.Add(oData.m_iWaitingCount.ToString());
                    listBored.Add(oData.m_iBoredCount.ToString());

                    float fMin;
                    float fMax;
                    float fTotal;
                    oLine.GetStopProgress(oData.m_usStop, out fMin, out fMax, out fTotal);
                    listDistance.Add((fMax / 1000f).ToString("N1"));
                }

                listStopNumber = Utils.PadToWidth(listStopNumber, true);
                listWaiting = Utils.PadToWidth(listWaiting, true);
                listBored = Utils.PadToWidth(listBored, true);
                listDistance = Utils.PadToWidth(listDistance, true);
            }
        }

        public string GetStopsTooltip()
        {
            string sTooltip = "";

            int iSTOPS_TO_SHOW = ModSettings.GetSettings().TooltipRowLimit;
            if (iSTOPS_TO_SHOW > 0 && m_stopPassengerCount != null)
            {
                List<string> listStopNumber;
                List<string> listWaiting;
                List<string> listBored;
                List<string> listDistance;
                GetStopData(iSTOPS_TO_SHOW, out listStopNumber, out listWaiting, out listBored, out listDistance);

                if (listDistance.Count > 0)
                {
                    if (GetTransportType() == TransportType.CableCar)
                    {
                        sTooltip += GetLineName() + "\r\n";
                    }
                    else
                    {
                        sTooltip += GetLineName() + "(" + listDistance[0] + "km)\r\n";
                    } 
                }
                else
                {
                    sTooltip += GetLineName() + "\r\n";
                }

                for (int i = 0; i < listStopNumber.Count; i++)
                {
                    if (sTooltip.Length > 0)
                    {
                        sTooltip += "\r\n";
                    }
                    sTooltip += listStopNumber[i];
                    if (GetTransportType() != TransportType.CableCar)
                    {
                        sTooltip += " | " + listDistance[i] + "km";
                    }
                    sTooltip += " | " + Localization.Get("OverviewWaiting") + ": " + listWaiting[i];
                    sTooltip += " | " + Localization.Get("OverviewBored") + ": " + listBored[i];
                }

                if (m_stopPassengerCount.Count > iSTOPS_TO_SHOW)
                {
                    int iWaitingRemainder = 0;
                    int iBoredRemainder = 0;
                    for (int i = iSTOPS_TO_SHOW; i < m_stopPassengerCount.Count; i++)
                    {
                        iWaitingRemainder += m_stopPassengerCount[i].m_iWaitingCount;
                        iBoredRemainder += m_stopPassengerCount[i].m_iBoredCount;
                    }
                    sTooltip += "\r\n\r\nAnd " + (m_stopPassengerCount.Count - iSTOPS_TO_SHOW) + " other stops (" + 
                        Localization.Get("OverviewWaiting") + ": " + iWaitingRemainder + " | " + 
                        Localization.Get("OverviewBored") + ": " + iBoredRemainder + ")";
                }
            }

            return sTooltip;
        }

        public abstract int CompareVehicleDataProgress(VehicleData x, VehicleData y);

        public string GetVehicleTooltip()
        {
            string sTooltip = ""; 
            
            // SHow all vehicles for this tooltip.
            if (m_vehicleDatas != null && m_stopPassengerCount != null)
            {
                // Load vehicle progress
                List<ushort> stops = GetStops();
                for (int i = 0; i < m_vehicleDatas.Count; ++i)
                {
                    VehicleData oVD = m_vehicleDatas[i];
                    float fCurrent;
                    float fTotal;
                    oVD.fProgress = LineIssueDetector.GetVehicleProgress(oVD.m_usVehicleId, stops[0], out fCurrent, out fTotal, 0);
                    oVD.m_fDistance = fCurrent;
                    m_vehicleDatas[i] = oVD;
                }

                // Sort by line progress
                m_vehicleDatas.Sort(CompareVehicleDataProgress);

                List<string> listPassengers = new List<string>();
                List<string> listVehicleNames = new List<string>();
                List<string> listVehicleIds = new List<string>();
                List<string> listVehicleProgress = new List<string>();
                List<string> listStatus = new List<string>();
                for (int i = 0; i < m_vehicleDatas.Count; i++)
                {
                    VehicleData oVD = m_vehicleDatas[i];
                    listPassengers.Add(oVD.m_iPassengers + "/" + oVD.m_iCapacity);
                    listVehicleIds.Add(oVD.m_usVehicleId.ToString());
                    listVehicleProgress.Add((oVD.m_fDistance / 1000f).ToString("N1") + "km");

                    // Vehicle name
                    string sVehicleName = Singleton<VehicleManager>.instance.GetVehicleName(oVD.m_usVehicleId);
                    if (sVehicleName == null)
                    {
                        sVehicleName = GetTransportType().ToString();
                    }
                    listVehicleNames.Add(sVehicleName);

                    string sStatus = "";
                    Vehicle oVehicle = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[oVD.m_usVehicleId];
                    if (oVehicle.m_blockCounter > 0)
                    {
                        sStatus = Localization.Get("txtBlocked") + " (" + oVehicle.m_blockCounter + ")";
                    }
                    else
                    {
                        InstanceID oInstanceId = new InstanceID();
                        oInstanceId.Vehicle = oVD.m_usVehicleId;
                        sStatus = oVehicle.Info.m_vehicleAI.GetLocalizedStatus(oVD.m_usVehicleId, ref oVehicle, out oInstanceId);
                    }
                    listStatus.Add(sStatus);
                }

                // Pad numbers
                listPassengers = Utils.PadToWidth(listPassengers, true);
                listVehicleIds = Utils.PadToWidth(listVehicleIds, true);
                listVehicleProgress = Utils.PadToWidth(listVehicleProgress, true);
                listStatus = Utils.PadToWidth(listStatus, false);
                listVehicleNames = Utils.PadToWidth(listVehicleNames, false);
                
                sTooltip += GetLineName() + "\r\n";

                List<string> listStopNumber;
                List<string> listWaiting;
                List<string> listBored;
                List<string> listDistance;
                GetStopData(m_stopPassengerCount.Count, out listStopNumber, out listWaiting, out listBored, out listDistance);
                
                // Build tooltip
                int iStopIndex = 0;
                for (int i = 0; i < m_vehicleDatas.Count; i++)
                {
                    VehicleData oVD = m_vehicleDatas[i];

                    // Add any stops before NextStop
                    int iNextStop = oVD.m_iNextStop;
                    LineData oCurrentStop = m_stopPassengerCount[iStopIndex];
                    while (iNextStop > oCurrentStop.m_iStopNumber)
                    {
                        if (GetTransportType() == TransportType.CableCar)
                        {
                            sTooltip += "\r\n" + listStopNumber[iStopIndex] + ". (" + listWaiting[iStopIndex] + ")";
                        }
                        else
                        {
                            sTooltip += "\r\n" + listStopNumber[iStopIndex] + ". " + listDistance[iStopIndex] + "km (" + listWaiting[iStopIndex] + ")";
                        }
                        
                        iStopIndex++;
                        oCurrentStop = m_stopPassengerCount[iStopIndex];
                    }

                    // Add vehicle
                    if (sTooltip.Length > 0)
                    {
                        sTooltip += "\r\n";
                    }
                    sTooltip += listVehicleNames[i];
                    sTooltip += " (" + listVehicleIds[i] + ")";
                    if (GetTransportType() != TransportType.CableCar)
                    {
                        sTooltip += " | " + listVehicleProgress[i];
                    }
                    sTooltip += " | " + listPassengers[i];
                    sTooltip += " | " + listStatus[i];
                }

                // Add last stops
                for (int i = iStopIndex; i < m_stopPassengerCount.Count; ++i)
                {
                    if (GetTransportType() == TransportType.CableCar)
                    {
                        sTooltip += "\r\n" + listStopNumber[i] + ". (" + listWaiting[i] + ")";
                    }
                    else
                    {
                        sTooltip += "\r\n" + listStopNumber[i] + ". " + listDistance[i] + "km (" + listWaiting[i] + ")";
                    }
                }
            }

            return sTooltip;
        }

        public virtual string GetPassengersTooltip()
        {
            return "";
        }

        // ----------------------------------------------------------------------------------------
        public static int ComparatorBase(int iResult, LineInfoBase oLine1, LineInfoBase oLine2)
        {
            if (iResult == 0)
            {
                iResult = oLine2.GetLineId() - oLine1.GetLineId();
            }
            return iResult;
        }

        public static int ComparatorName(LineInfoBase oLine1, LineInfoBase oLine2)
        {
            return ComparatorBase(oLine1.GetLineName().CompareTo(oLine2.GetLineName()), oLine1, oLine2);
        }

        public static int ComparatorStops(LineInfoBase oLine1, LineInfoBase oLine2)
        {
            return ComparatorBase(oLine1.GetStopCount().CompareTo(oLine2.GetStopCount()), oLine1, oLine2);
        }

        public static int ComparatorVehicles(LineInfoBase oLine1, LineInfoBase oLine2)
        {
            return ComparatorBase(oLine1.m_iVehicleCount.CompareTo(oLine2.m_iVehicleCount), oLine1, oLine2);
        }

        public static int ComparatorPassengers(LineInfoBase oLine1, LineInfoBase oLine2)
        {
            return ComparatorBase(oLine1.m_iPassengers.CompareTo(oLine2.m_iPassengers), oLine1, oLine2);
        }

        public static int ComparatorUsage(LineInfoBase oLine1, LineInfoBase oLine2)
        {
            return ComparatorBase(oLine1.GetUsage().CompareTo(oLine2.GetUsage()), oLine1, oLine2);
        }

        public static int ComparatorWaiting(LineInfoBase oLine1, LineInfoBase oLine2)
        {
            return ComparatorBase(oLine1.m_iWaiting.CompareTo(oLine2.m_iWaiting), oLine1, oLine2);
        }

        public static int ComparatorBusiest(LineInfoBase oLine1, LineInfoBase oLine2)
        {
            return ComparatorBase(oLine1.m_iBusiest.CompareTo(oLine2.m_iBusiest), oLine1, oLine2);
        }

        public static int ComparatorBored(LineInfoBase oLine1, LineInfoBase oLine2)
        {
            return ComparatorBase(oLine1.m_iBored.CompareTo(oLine2.m_iBored), oLine1, oLine2);
        }

        public static Comparison<LineInfoBase> GetComparator(ListViewRowComparer.Columns eSortColumn)
        {
            switch (eSortColumn)
            {
                case ListViewRowComparer.Columns.COLUMN_NAME:
                    return LineInfoBase.ComparatorName;
                case ListViewRowComparer.Columns.COLUMN_STOPS:
                    return LineInfoBase.ComparatorStops;
                case ListViewRowComparer.Columns.COLUMN_VEHICLES:
                    return LineInfoBase.ComparatorVehicles;
                case ListViewRowComparer.Columns.COLUMN_PASSENGERS:
                    return LineInfoBase.ComparatorPassengers;
                case ListViewRowComparer.Columns.COLUMN_VEHICLE_USAGE:
                    return LineInfoBase.ComparatorUsage;
                case ListViewRowComparer.Columns.COLUMN_WAITING:
                    return LineInfoBase.ComparatorWaiting;
                case ListViewRowComparer.Columns.COLUMN_BUSIEST:
                    return LineInfoBase.ComparatorBusiest;
                case ListViewRowComparer.Columns.COLUMN_BORED:
                    return LineInfoBase.ComparatorBored;
            }

            return LineInfoBase.ComparatorName;
        }

        public static int CompareTo(ListViewRowComparer.Columns eSortColumn, LineInfoBase oFirst, LineInfoBase oSecond)
        {
            Comparison<LineInfoBase> SortComparison = GetComparator(eSortColumn);
            return SortComparison(oFirst, oSecond);
        }

        public int CompareTo(LineInfoBase obj)
        {
            return ComparatorName(this, obj);
        }
    }
}
