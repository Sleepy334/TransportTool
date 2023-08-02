using ColossalFramework;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public enum PublicTransportType
    {
        Bus,
        Trolleybus,
        Tram,
        Metro,
        Train,
        Ship,
        Airplane,
        Monorail,
        CableCar,
        Taxi,
        Tours,
    }

    public class PublicTransportTypeUtils
    {
        public static PublicTransportType Convert(TransportInfo.TransportType eTransportType)
        {
            switch (eTransportType)
            {
                case TransportInfo.TransportType.Bus: return PublicTransportType.Bus;
                case TransportInfo.TransportType.Trolleybus: return PublicTransportType.Trolleybus;
                case TransportInfo.TransportType.Tram: return PublicTransportType.Tram;
                case TransportInfo.TransportType.Metro: return PublicTransportType.Metro;
                case TransportInfo.TransportType.Train: return PublicTransportType.Train;
                case TransportInfo.TransportType.Ship: return PublicTransportType.Ship;
                case TransportInfo.TransportType.Airplane: return PublicTransportType.Airplane;
                case TransportInfo.TransportType.Helicopter: return PublicTransportType.Airplane;
                case TransportInfo.TransportType.HotAirBalloon: return PublicTransportType.Airplane;
                case TransportInfo.TransportType.Monorail: return PublicTransportType.Monorail;
                case TransportInfo.TransportType.CableCar: return PublicTransportType.CableCar;
                case TransportInfo.TransportType.Taxi: return PublicTransportType.Taxi;
                case TransportInfo.TransportType.Pedestrian: return PublicTransportType.Tours;
                case TransportInfo.TransportType.TouristBus: return PublicTransportType.Tours;
                default: return PublicTransportType.Bus;
            }
        }

        public static Color32 GetDefaultLineColor(PublicTransportType eTransportType) 
        {
            List<TransportInfo.TransportType> oTypes = Convert(eTransportType);
            if (oTypes.Count > 0)
            {
                return GetDefaultLineColor(oTypes[0]);
            }
            else
            {
                return new Color32(255, 255, 255, 255);
            }
        }

        public static Color32 GetDefaultLineColor(TransportInfo.TransportType eTransportType)
        {
            switch (eTransportType)
            {
                case TransportInfo.TransportType.Bus: return new Color32(42, 141, 190, 255);
                case TransportInfo.TransportType.Trolleybus: return new Color32(255, 130, 0, 255);
                case TransportInfo.TransportType.Tram: return new Color32(119, 0, 219, 255);
                case TransportInfo.TransportType.Metro: return new Color32(0, 184, 0, 255);
                case TransportInfo.TransportType.Train: return new Color32(219, 85, 0, 255);
                case TransportInfo.TransportType.Ship: return new Color32(202, 184, 91, 255);
                case TransportInfo.TransportType.Airplane: return new Color32(170, 85, 154, 255);
                case TransportInfo.TransportType.Helicopter: return new Color(.671f, .333f, .604f, 1);
                case TransportInfo.TransportType.HotAirBalloon: return new Color32(0xd8, 0x01, 0xaa, 255);
                case TransportInfo.TransportType.Monorail: return new Color32(217, 50, 88, 255);
                case TransportInfo.TransportType.CableCar: return new Color32(28, 95, 225, 255);
                case TransportInfo.TransportType.Taxi: return new Color32(86, 186, 137, 255);
                case TransportInfo.TransportType.Pedestrian: return new Color32(83, 157, 48, 255);
                case TransportInfo.TransportType.TouristBus: return new Color32(110, 152, 251, 255);
                default: return new Color32(53, 121, 188, 255);
            }
        }
        public static string GetDefaultLineTypeName(TransportInfo.TransportType eTransportType)
        {
            switch (eTransportType)
            {
                case TransportInfo.TransportType.Airplane: return "Blimp";
                default: return eTransportType.ToString();
            }
        }

        public static List<TransportInfo.TransportType> Convert(PublicTransportType eTransportType)
        {
            List<TransportInfo.TransportType> oList = new List<TransportInfo.TransportType>();

            switch (eTransportType)
            {
                case PublicTransportType.Bus: oList.Add(TransportInfo.TransportType.Bus); break;
                case PublicTransportType.Trolleybus: oList.Add(TransportInfo.TransportType.Trolleybus); break;
                case PublicTransportType.Tram: oList.Add(TransportInfo.TransportType.Tram); break;
                case PublicTransportType.Metro: oList.Add(TransportInfo.TransportType.Metro); break;
                case PublicTransportType.Train: oList.Add(TransportInfo.TransportType.Train); break;
                case PublicTransportType.Ship: oList.Add(TransportInfo.TransportType.Ship); break;
                case PublicTransportType.Airplane:
                    {
                        oList.Add(TransportInfo.TransportType.Airplane);
                        oList.Add(TransportInfo.TransportType.Helicopter);
                        oList.Add(TransportInfo.TransportType.HotAirBalloon);
                        break;
                    }
                case PublicTransportType.Monorail: oList.Add(TransportInfo.TransportType.Monorail); break;
                case PublicTransportType.CableCar: oList.Add(TransportInfo.TransportType.CableCar); break;
                case PublicTransportType.Taxi: oList.Add(TransportInfo.TransportType.Taxi); break;
                case PublicTransportType.Tours:
                    {
                        oList.Add(TransportInfo.TransportType.TouristBus); 
                        oList.Add(TransportInfo.TransportType.Pedestrian);
                        break;
                    }
                default: break;
            }

            return oList;
        }

        public static string GetVehicleTypeIcon(TransportInfo.TransportType type)
        {
            switch (type)
            {
                case TransportInfo.TransportType.Bus:
                    return "SubBarPublicTransportBus";
                case TransportInfo.TransportType.Trolleybus:
                    return "SubBarPublicTransportTrolleybus";
                case TransportInfo.TransportType.Tram:
                    return "SubBarPublicTransportTram";
                case TransportInfo.TransportType.Metro:
                    return "SubBarPublicTransportMetro";
                case TransportInfo.TransportType.Train:
                    return "SubBarPublicTransportTrain";
                case TransportInfo.TransportType.Ship:
                    return "SubBarPublicTransportShip";
                case TransportInfo.TransportType.Airplane:
                case TransportInfo.TransportType.Helicopter:
                    return "SubBarPublicTransportPlane";
                case TransportInfo.TransportType.CableCar:
                    return "SubBarPublicTransportCableCar";
                case TransportInfo.TransportType.Monorail:
                    return "SubBarPublicTransportMonorail";
                case TransportInfo.TransportType.Taxi:
                    return "SubBarPublicTransportTaxi";
                case TransportInfo.TransportType.TouristBus:
                    return "InfoIconTours";
                default:
                    return "ToolbarIconPublicTransportFocused";
            }
        }
    }
}
