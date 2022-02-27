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

        public List<LineInfo> GetLineList(PublicTransportType eType)
        {
            List<TransportInfo.TransportType> oTypes = PublicTransportTypeUtils.Convert(eType);
            List<LineInfo> list = new List<LineInfo>();

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

        public static LineInfo GetLineInfo(int iLineId, TransportLine oLine)
        {
            LineInfo oInfo = new LineInfo();
            oInfo.LoadInfo(iLineId, oLine);

            return oInfo;
        }

        public static LineInfo GetLineInfo(int iLineId)
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

        public static List<LineInfo> SortList(List<LineInfo> oList, ListViewRowComparer.Columns eSortColumn, bool bSortDesc)
        {
            if (oList != null)
            {
                switch (eSortColumn)
                {
                    case ListViewRowComparer.Columns.COLUMN_NAME:
                        {
                            oList.Sort(LineInfo.ComparatorName);
                            break;
                        }
                    case ListViewRowComparer.Columns.COLUMN_STOPS:
                        {
                            oList.Sort(LineInfo.ComparatorStops);
                            break;
                        }
                    case ListViewRowComparer.Columns.COLUMN_PASSENGERS:
                        {
                            oList.Sort(LineInfo.ComparatorPassengers);
                            break;
                        }
                    case ListViewRowComparer.Columns.COLUMN_WAITING:
                        {
                            oList.Sort(LineInfo.ComparatorWaiting);
                            break;
                        }
                    case ListViewRowComparer.Columns.COLUMN_BUSIEST:
                        {
                            oList.Sort(LineInfo.ComparatorBusiest);
                            break;
                        }
                    case ListViewRowComparer.Columns.COLUMN_BORED:
                        {
                            oList.Sort(LineInfo.ComparatorBored);
                            break;
                        }
                }

                if (bSortDesc)
                {
                    oList.Reverse();
                }
            }
            return oList;
        }
    }
}
