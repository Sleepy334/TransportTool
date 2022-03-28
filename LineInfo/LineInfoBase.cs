using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
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
        }

        public int m_iLineId;
        public string m_sName;
        public Color32 m_color;
        public int m_iPassengers;
        public int m_iCapacity;
        public int m_iVehicleCount;
        public int m_iWaiting;
        public int m_iBusiest;
        public int m_iBored;
        public ushort m_usBusiestStopId;
        public int m_iBusiestStopNumber;
        protected List<KeyValuePair<int, int>> m_stopPassengerCount;
        protected List<VehicleData> m_vehicleDatas;

        public LineInfoBase()
        {
            m_sName = "";
            m_iPassengers = 0;
            m_iCapacity = 0;
            m_iVehicleCount = 0;
            m_iWaiting = 0;
            m_iBusiest = 0;
            m_iBored = 0;
            m_usBusiestStopId = 0;
            m_iBusiestStopNumber = 0;
            m_stopPassengerCount = new List<KeyValuePair<int, int>>();
            m_vehicleDatas = new List<VehicleData>();
        }

        public abstract TransportType GetTransportType();
        public abstract int GetStopCount();
        public abstract List<ushort> GetStops();
        public abstract VehicleProgressLine? GetVehicleProgress();
        public abstract void ShowBusiestStop();
        protected abstract void LoadVehicleInfo();

        public static int ComparatorBase(int iResult, LineInfoBase oLine1, LineInfoBase oLine2)
        {
            if (iResult == 0)
            {
                iResult = oLine2.m_iLineId - oLine1.m_iLineId;
            }
            return iResult;
        }

        public static int ComparatorName(LineInfoBase oLine1, LineInfoBase oLine2)
        {
            return ComparatorBase(oLine1.m_sName.CompareTo(oLine2.m_sName), oLine1, oLine2);
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

        public virtual void UpdateInfo()
        {
            LoadLineInfo();
            LoadVehicleInfo();
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
                if (iStopPassengerCount > m_iBusiest)
                {
                    m_iBusiest = iStopPassengerCount;
                    m_usBusiestStopId = usStop;
                    m_iBusiestStopNumber = i + 1;
                }

                m_stopPassengerCount.Add(new KeyValuePair<int, int>(i + 1, iStopPassengerCount));
                m_iBored += iBored;
                m_iWaiting += iStopPassengerCount;
                i++;
            }
        }

        public string GetWaitingTooltip(UILabel lblColumn)
        {
            const int iSTOPS_TO_SHOW = 10;
            
            string sTooltip = "";
            if (m_stopPassengerCount != null)
            {
                m_stopPassengerCount.Sort((x, y) =>
                {
                    if (y.Value == x.Value)
                    {
                        return x.Key - y.Key;
                    }
                    else
                    {
                        return y.Value - x.Value;
                    }
                    
                });
                
                List<string> listWaiting = new List<string>();
                List<string> listStopNumber = new List<string>();
                for (int i = 0; i < Math.Min(m_stopPassengerCount.Count, iSTOPS_TO_SHOW); i++)
                {
                    KeyValuePair<int, int> kvp = m_stopPassengerCount[i];
                    listWaiting.Add(kvp.Value.ToString());
                    listStopNumber.Add(kvp.Key.ToString());
                }

                UIFontRenderer? oRenderer = null;
                float units = 1;
                if (lblColumn is not null)
                {
                    oRenderer = lblColumn.ObtainRenderer();
                    units = lblColumn.GetUIView().PixelsToUnits();
                }
                if (oRenderer is not null)
                {
                    listWaiting = Utils.PadToWidthFront(oRenderer, units, listWaiting);
                    listStopNumber = Utils.PadToWidthFront(oRenderer, units, listStopNumber);
                }

                for (int i = 0; i < listWaiting.Count; i++)
                {
                    sTooltip += "Waiting: " + listWaiting[i] + " | Stop: " + listStopNumber[i] + "\r\n";
                }

                if (m_stopPassengerCount.Count > iSTOPS_TO_SHOW)
                {
                    int iWaitingRemainer = 0;
                    for (int i = iSTOPS_TO_SHOW; i < m_stopPassengerCount.Count; i++)
                    {
                        iWaitingRemainer += m_stopPassengerCount[i].Value;
                    }
                    sTooltip += "\r\nAnd " + (m_stopPassengerCount.Count - iSTOPS_TO_SHOW) + " other stops (" + iWaitingRemainer + ")";
                }
            }

            return sTooltip;
        }

        public string GetVehicleTooltip(UILabel lblColumn)
        {
            const int iVEHICLES_TO_SHOW = 10;

            string sTooltip = "";
            if (m_vehicleDatas != null)
            {
                // Sort by fullest vehicles
                m_vehicleDatas.Sort((x, y) =>
                {
                    return y.m_iPassengers - x.m_iPassengers;
                });

                // Load fullest vehicles into new array
                int iVehiclesToShow = Math.Min(m_vehicleDatas.Count, iVEHICLES_TO_SHOW);
                List<VehicleData> oFullestVehicles = new List<VehicleData>();
                oFullestVehicles.AddRange(m_vehicleDatas.GetRange(0, iVehiclesToShow));

                // Sort by line progress
                oFullestVehicles.Sort((x, y) =>
                {
                    return x.m_iNextStop - y.m_iNextStop;
                });

                List<string> listPassengers = new List<string>();
                List<string> listStopNumber = new List<string>();
                List<string> listVehicleNames = new List<string>();
                for (int i = 0; i < oFullestVehicles.Count; i++)
                {
                    VehicleData oVD = oFullestVehicles[i];
                    listPassengers.Add(oVD.m_iPassengers + "/" + oVD.m_iCapacity);
                    listStopNumber.Add(oVD.m_iNextStop.ToString());
                    string sVehicleName = Singleton<VehicleManager>.instance.GetVehicleName(oVD.m_usVehicleId);
                    if (sVehicleName == null)
                    {
                        sVehicleName = GetTransportType().ToString();
                    }
                    listVehicleNames.Add(sVehicleName);
                }

                // Pad numbers
                UIFontRenderer? oRenderer = null;
                float units = 1;
                if (lblColumn is not null)
                {
                    oRenderer = lblColumn.ObtainRenderer();
                    units = lblColumn.GetUIView().PixelsToUnits();
                }
                if (oRenderer is not null)
                {
                    listPassengers = Utils.PadToWidthFront(oRenderer, units, listPassengers);
                    listStopNumber = Utils.PadToWidthFront(oRenderer, units, listStopNumber);
                    listVehicleNames = Utils.PadToWidthBack(oRenderer, units, listVehicleNames);
                }

                // Build tooltip
                for (int i = 0; i < listPassengers.Count; i++)
                {
                    sTooltip += listPassengers[i] + " | Next stop: " + listStopNumber[i] + " | " + listVehicleNames[i] + "\r\n";
                }
                if (m_vehicleDatas.Count > iVehiclesToShow)
                {
                    int iPassengerRemainder = 0;
                    int iCapacityRemainder = 0;
                    for (int i = iVehiclesToShow; i < m_vehicleDatas.Count; i++)
                    {
                        iPassengerRemainder += m_vehicleDatas[i].m_iPassengers;
                        iCapacityRemainder += m_vehicleDatas[i].m_iCapacity;
                    }
                    sTooltip += "\r\nAnd " + (m_vehicleDatas.Count - oFullestVehicles.Count) + " other vehicles (" + iPassengerRemainder + "/" + iCapacityRemainder + ")";
                }
            }

            return sTooltip;
        }

        public virtual string GetPassengersTooltip()
        {
            return "";
        }
    }
}
