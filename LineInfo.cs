using System;
using UnityEngine;

namespace PublicTransportInfo
{
    public class LineInfo : IComparable<LineInfo>
    {
        public int m_iLineId;
        public string m_sName;
        public Color32 m_color;
        public int m_iStops;
        public int m_iPassengers;
        public int m_iCapacity;
        public int m_iWaiting;
        public int m_iBusiest;
        public ushort m_usBusiestStopId;

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

        public LineInfo()
        {
            m_sName = "";
            m_iStops = 0;
            m_iPassengers = 0;
            m_iCapacity = 0;
            m_iWaiting = 0;
            m_iBusiest = 0;
            m_usBusiestStopId = 0;
        }

        public void LoadInfo(int iLineId, TransportLine oLine)
        {
            m_iLineId = iLineId;
            m_sName = GetSafeLineName(iLineId, oLine);
            m_color = GetSafeLineColor(iLineId, oLine);
            LoadLineInfo(iLineId, oLine);
            LoadVehicleInfo(iLineId);
        }

        public static int CompareTo(ListViewRowComparer.Columns eSortColumn, LineInfo oFirst, LineInfo oSecond)
        {
            int iResult = 0;
            switch (eSortColumn)
            {
                case ListViewRowComparer.Columns.COLUMN_NAME: // Name
                    {
                        iResult = ComparatorName(oFirst, oSecond);
                        break;
                    }
                case ListViewRowComparer.Columns.COLUMN_STOPS: // Stops
                    {
                        iResult = ComparatorStops(oFirst, oSecond);
                        break;
                    }
                case ListViewRowComparer.Columns.COLUMN_PASSENGERS: // Passengers
                    {
                        iResult = ComparatorPassengers(oFirst, oSecond);
                        break;
                    }
                case ListViewRowComparer.Columns.COLUMN_WAITING: // Waiting
                    {
                        iResult = ComparatorWaiting(oFirst, oSecond);
                        break;
                    }
                case ListViewRowComparer.Columns.COLUMN_BUSIEST: // Busiest
                    {
                        iResult = ComparatorBusiest(oFirst, oSecond);
                        break;
                    }
            }

            return iResult;
        }
        

        private void LoadLineInfo(int iLineId, TransportLine oLine)
        {
            m_iBusiest = 0;
            m_iWaiting = 0;
            m_usBusiestStopId = 0;

            m_iStops = oLine.CountStops((ushort)iLineId);
            for (int i = 0; i < m_iStops; i++)
            {
                ushort usStop = oLine.GetStop(i);
                int iStopCount = oLine.CalculatePassengerCount(usStop);
                if (iStopCount > m_iBusiest)
                {
                    m_iBusiest = iStopCount;
                    m_usBusiestStopId = usStop;
                }

                m_iWaiting += iStopCount;
            }
        }

        private static string GetSafeLineName(int iLineId, TransportLine oLine)
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

        private static Color32 GetSafeLineColor(int iLineId, TransportLine oLine)
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

            int iCapacity = 0;
            TransportLine oLine = TransportManager.instance.m_lines.m_buffer[iLineId];
            int iVehicleCount = oLine.CountVehicles((ushort)iLineId);
            for (int i = 0; i < iVehicleCount; ++i)
            {
                ushort usVehicleId = oLine.GetVehicle(i);
                int iPassengers = GetVehiclePassengerCount(usVehicleId, out iCapacity);
                m_iCapacity += iCapacity;
                m_iPassengers += iPassengers;
            }
        }

        private static int GetVehicleCapacity(ushort usVehicleId)
        {
            int iCapacity = 0;

            var instance = VehicleManager.instance;
            Vehicle oVehicle = instance.m_vehicles.m_buffer[usVehicleId];
            VehicleInfo oVehicleInfo = oVehicle.Info;
            VehicleAI oVehicleAI = oVehicleInfo.m_vehicleAI;
            if (oVehicleAI != null)
            {
                if (oVehicleAI is BusAI)
                {
                    iCapacity = ((BusAI)oVehicleAI).m_passengerCapacity;
                }
                else if (oVehicleAI is PassengerTrainAI)
                {
                    iCapacity = ((PassengerTrainAI)oVehicleAI).m_passengerCapacity;
                }
                else if (oVehicleAI is TramAI)
                {
                    iCapacity = ((TramAI)oVehicleAI).m_passengerCapacity;
                }
                else if (oVehicleAI is TrolleybusAI)
                {
                    iCapacity = ((TrolleybusAI)oVehicleAI).m_passengerCapacity;
                }
                else if (oVehicleAI is PassengerFerryAI)
                {
                    iCapacity = ((PassengerFerryAI)oVehicleAI).m_passengerCapacity;
                }
                else if (oVehicleAI is CableCarAI)
                {
                    iCapacity = ((CableCarAI)oVehicleAI).m_passengerCapacity;
                }
                else if (oVehicleAI is PassengerHelicopterAI)
                {
                    iCapacity = ((PassengerHelicopterAI)oVehicleAI).m_passengerCapacity;
                }
                else if (oVehicleAI is PassengerBlimpAI)
                {
                    iCapacity = ((PassengerBlimpAI)oVehicleAI).m_passengerCapacity;
                }
                else if (oVehicleAI is PassengerPlaneAI)
                {
                    iCapacity = ((PassengerPlaneAI)oVehicleAI).m_passengerCapacity;
                }
                else
                {
                    Debug.Log("Vehicle " + usVehicleId + " type not handled");
                }
                return iCapacity;
            }


            return iCapacity;
        }

        private static int GetVehiclePassengerCount(ushort usVehicleId, out int iCapacity)
        {
            int iPassengers = 0;
            iCapacity = 0;

            var instance = VehicleManager.instance;
            Vehicle oVehicle = instance.m_vehicles.m_buffer[usVehicleId];
            VehicleInfo oVehicleInfo = oVehicle.Info;
            iPassengers += oVehicle.m_transferSize;
            iCapacity += GetVehicleCapacity(usVehicleId);

            int iMaxTrailers = 100;// Doesnt seem to work...oVehicleInfo.m_maxTrailerCount;
            int iTrailerCount = 0;
            ushort usTrailingVehicle = oVehicle.m_trailingVehicle;

            while (usTrailingVehicle != 0 && iTrailerCount < iMaxTrailers)
            {
                iTrailerCount++;
                Vehicle oTrailer = instance.m_vehicles.m_buffer[usTrailingVehicle];
                iPassengers += oTrailer.m_transferSize;
                iCapacity += GetVehicleCapacity(usTrailingVehicle);
                usTrailingVehicle = oTrailer.m_trailingVehicle;
            }

            return iPassengers;
        }

        public int CompareTo(LineInfo obj)
        {
            return ComparatorName(this, obj);
        }

    }
}
