using ColossalFramework;
using ICities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public class LineInfoLoader
    {
        public LineInfoLoader()
        {
        }

        public List<LineInfoBase> GetLineList(PublicTransportType eType)
        {
            List<LineInfoBase> list = new List<LineInfoBase>(); 
            
            if (eType == PublicTransportType.CableCar)
            {
                // CableCar dont have lines so we need a separate method.
                FastList<ushort> oServiceBuildings = BuildingManager.instance.GetServiceBuildings(ItemClass.Service.PublicTransport);
                foreach (ushort usBuildingId in oServiceBuildings)
                {
                    Building oBuilding = BuildingManager.instance.m_buildings.m_buffer[usBuildingId];
                    if (oBuilding.Info.m_class.m_subService == ItemClass.SubService.PublicTransportCableCar &&
                        oBuilding.Info.name == "Cable Car Station End" &&
                        TransportManagerUtils.IsFirstStation(usBuildingId))
                    {
                        LineInfoCableCar lineInfo = new LineInfoCableCar(usBuildingId);
                        lineInfo.m_sName = "CableCar Line " + (list.Count + 1);
                        lineInfo.m_color = PublicTransportTypeUtils.GetDefaultLineColor(eType);
                        lineInfo.UpdateInfo();
                        list.Add(lineInfo);
                    }

                    /*
                    if (oBuilding.Info.m_class.m_subService == ItemClass.SubService.PublicTransportTaxi)
                    {
                        string sBuildingInfo = "";
                        int iPassengers = 0;
                        //sBuildingInfo = oBuilding.Info.name + "\r\n";
                        ushort usVehicleId = oBuilding.m_ownVehicles;
                        while (usVehicleId != 0)
                        {
                            Vehicle oVehicle = VehicleManager.instance.m_vehicles.m_buffer[usVehicleId];
                            iPassengers += oVehicle.m_touristCount + oVehicle.m_transferSize;
                            //sBuildingInfo += "Vehicle:" + usVehicleId + " Tourists:" + oVehicle.m_touristCount + " Transfer: " + oVehicle.m_transferSize + "\r\n";
                            usVehicleId = oVehicle.m_nextOwnVehicle;
                        }
                        sBuildingInfo += oBuilding.Info.name + " Passengers: " + iPassengers + "\r\n";
                        Debug.Log(sBuildingInfo);
                    }
                    */
                }
            } 
            else
            {
                List<TransportInfo.TransportType> oTypes = PublicTransportTypeUtils.Convert(eType);
                uint iSize = TransportManager.instance.m_lines.m_size;
                for (int i = 0; i < iSize; i++)
                {
                    TransportLine oLine = TransportManager.instance.m_lines.m_buffer[i];
                    TransportInfo oInfo = oLine.Info;
                    if (oLine.Complete && oTypes.Contains(oInfo.m_transportType))
                    {
                        list.Add(GetLineInfo(i, oLine));
                    }
                }
            }

            return list;
        }

        public List<LineInfoBase> GetAllLinesList()
        {
            List<LineInfoBase> list = new List<LineInfoBase>();

            uint iSize = TransportManager.instance.m_lines.m_size;
            for (int i = 0; i < iSize; i++)
            {
                TransportLine oLine = TransportManager.instance.m_lines.m_buffer[i];
                TransportInfo oInfo = oLine.Info;

                if (oLine.Complete)
                {
                    list.Add(GetLineInfo(i, oLine));
                }
            }

            // Add cable car lines
            List<LineInfoBase> oCableCarLines = GetLineList(PublicTransportType.CableCar);
            list.AddRange(oCableCarLines);
            
            return list;
        }

        public static List<PublicTransportType> GetLineTypes()
        {
            List<PublicTransportType> list = new List<PublicTransportType>();

            uint iSize = TransportManager.instance.m_lines.m_size;
            for (int i = 0; i < iSize; i++)
            {
                TransportLine oLine = TransportManager.instance.m_lines.m_buffer[i];
                TransportInfo oInfo = oLine.Info;
                TransportInfo.TransportType eType = oInfo.m_transportType;
                PublicTransportType ePTT = PublicTransportTypeUtils.Convert(eType);
                if (oLine.Complete)
                {
                    if (!list.Contains(ePTT))
                    {
                        list.Add(ePTT);
                    }
                }
            }


            // See if we have any cable car lines
            FastList<ushort> oServiceBuildings = BuildingManager.instance.GetServiceBuildings(ItemClass.Service.PublicTransport);
            foreach (ushort usBuildingId in oServiceBuildings)
            {
                Building oBuilding = BuildingManager.instance.m_buildings.m_buffer[usBuildingId];
                if (oBuilding.Info.m_class.m_subService == ItemClass.SubService.PublicTransportCableCar &&
                    oBuilding.Info.name == "Cable Car Station End" &&
                    TransportManagerUtils.IsFirstStation(usBuildingId))
                {
                    list.Add(PublicTransportType.CableCar);
                    break;
                }
            }
            

            list.Sort();
            return list;
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

        public static LineInfoBase GetLineInfo(int iLineId, TransportLine oLine)
        {
            LineInfoTransportLine oInfo = new LineInfoTransportLine(oLine.Info.m_transportType);
            oInfo.m_iLineId = iLineId;
            oInfo.UpdateInfo();

            return oInfo;
        }

        public static LineInfoBase GetLineInfo(int iLineId)
        {
            TransportLine oLine = TransportManager.instance.m_lines.m_buffer[iLineId];
            return GetLineInfo(iLineId, oLine);
        }

        public string TruncatePad(string text, int iCharacters)
        {
            if (text.Length > iCharacters) 
            { 
                text = text.Substring(0, iCharacters);
            }
            while (text.Length < iCharacters)
            {
                text += " ";
            }
            return text + "|";
        }

        public static List<LineInfoBase> SortList(List<LineInfoBase> oList, ListViewRowComparer.Columns eSortColumn, bool bSortDesc)
        {
            if (oList != null)
            {
                Comparison<LineInfoBase> SortComparison = LineInfoBase.GetComparator(eSortColumn);
                oList.Sort(SortComparison);

                if (bSortDesc)
                {
                    oList.Reverse();
                }

                return oList;
            }
            else
            {
                return new List<LineInfoBase>();
            }
        }
    }
}
