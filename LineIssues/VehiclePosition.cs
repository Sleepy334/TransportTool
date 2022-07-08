using System;
using UnityEngine;

namespace PublicTransportInfo
{
    public class VehiclePosition
    {
        public float m_fTimeStamp;
        public Vector3 m_oVehiclePosition;

        public static VehiclePosition Create(Vehicle oVehicle, float fTimeStamp)
        {
            VehiclePosition v = new VehiclePosition();
            v.m_fTimeStamp = fTimeStamp;
            v.m_oVehiclePosition = GetVehiclePosition(oVehicle);
            return v;
        }

        public static Vector3 GetVehiclePosition(Vehicle oVehicle)
        {
            switch (oVehicle.m_lastFrame)
            {
                case 0: return oVehicle.m_frame0.m_position;
                case 1: return oVehicle.m_frame1.m_position;
                case 2: return oVehicle.m_frame2.m_position;
                case 3: return oVehicle.m_frame3.m_position;
            }

            return oVehicle.m_frame0.m_position;
        }
    }
}
