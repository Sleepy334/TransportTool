using System;
using UnityEngine;

namespace PublicTransportInfo
{
    public class VehicleProgress
    {
        public float m_fProgress;
        public uint m_path; // Network section
        public ushort m_pathPositionIndex; // Progress on path
        public int m_stopIndex; // Line stop index
        public float m_fTimeStamp;
        public Vector3 m_oVehiclePosition;

        public static VehicleProgress Create(float fProgress, Vehicle oVehicle, int iStopIndex)
        {
            VehicleProgress v = new VehicleProgress();
            v.m_fProgress = fProgress;
            v.m_path = oVehicle.m_path;
            v.m_pathPositionIndex = oVehicle.m_pathPositionIndex;
            v.m_stopIndex = iStopIndex;
            v.m_fTimeStamp = 0;
            v.m_oVehiclePosition = GetVehiclePosition(oVehicle);
            return v;
        }

        public bool HasMoved(VehicleProgress oSecond)
        {
            if (m_stopIndex != oSecond.m_stopIndex)
            {
                return true;
            }
            if (m_path != oSecond.m_path)
            {
                return true;
            }
            if (m_pathPositionIndex != oSecond.m_pathPositionIndex)
            {
                return true;
            }
            return false;
        }

        public int CompareTo(VehicleProgress oSecond, int iTotalStops)
        {
            int iResult = (int)((m_fProgress * 10) - (oSecond.m_fProgress * 10));
            if (iResult <= 0)
            {
                // Check stop index if different
                if (m_stopIndex != oSecond.m_stopIndex)
                {
                    // Check for line wrap first
                    if (m_stopIndex == 0 && oSecond.m_stopIndex == iTotalStops - 1)
                    {
                        return 1;
                    }
                    else
                    {
                        return m_stopIndex - oSecond.m_stopIndex;
                    }
                }

                // Check if m_path can be used
                if (m_path != oSecond.m_path)
                {
                    return 1;
                }
                if (m_pathPositionIndex != 255 && oSecond.m_pathPositionIndex != 255)
                {
                    return m_pathPositionIndex - oSecond.m_pathPositionIndex;
                }
            }

            return iResult;
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
