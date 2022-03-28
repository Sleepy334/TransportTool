using ColossalFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public class VehicleProgressLine
    {
        public const int iSIMULATION_TIMER2_DAY_LENGTH = 10; // in game seconds

        public ushort m_iLineId; 
        public Dictionary<ushort, VehicleProgress> m_vehicleProgress;
        public static ushort m_followingVehicle = 0;

        public VehicleProgressLine(ushort usLineId)
        {
            m_iLineId = usLineId;
            m_vehicleProgress = new Dictionary<ushort, VehicleProgress>();
        }

        public int GetCount()
        {
            return m_vehicleProgress.Count;
        }

        public VehicleProgress? GetProgress(ushort usVehicleId)
        {
            if (m_vehicleProgress.ContainsKey(usVehicleId))
            {
                return m_vehicleProgress[usVehicleId];
            }
            else
            {
                return null;
            }
        }

        public void DespawnVehicle(ushort usVehicleId, Vehicle oVehicle)
        {
            // Check the vehicle hasn't been removed by user before flagging as despawned
            // Despawned vehicles have their flags wiped.
            if (m_vehicleProgress != null && m_vehicleProgress.ContainsKey(usVehicleId))
            {
                if (PublicTransportInstance.GetSettings().WarnVehicleDespawed)
                {
                    Debug.Log("Line: " + m_iLineId + "Despawned vehicle: " + usVehicleId + " flags: " + oVehicle.m_flags + " flags2 " + oVehicle.m_flags2);

                    // Add to despawned list
                    VehicleProgress oProgress = m_vehicleProgress[usVehicleId];
                    LineIssue oLineIssue = new LineIssueDespawned(m_iLineId, usVehicleId, oProgress.m_oVehiclePosition);
                    oLineIssue.m_iLineId = m_iLineId;
                    PublicTransportInstance.GetLineIssueManager().AddLineIssue(oLineIssue);
                }

                // Now remove from list
                m_vehicleProgress.Remove(usVehicleId);
            }
        }

        public void LoadVehicleProgress()
        {
            float fSimulationTimer2 = Utils.GetSimulationTimestamp();
            TransportLine oLine = TransportManager.instance.m_lines.m_buffer[m_iLineId];

            // Load vehicle ID's.
            var instance = VehicleManager.instance;
            List<ushort> oVehicles = new List<ushort>();

            int iVehicleCount = oLine.CountVehicles(m_iLineId);
            for (int i = 0; i < iVehicleCount; ++i)
            {
                ushort usVehicleId = oLine.GetVehicle(i);
                Vehicle oVehicle = instance.m_vehicles.m_buffer[usVehicleId];

                // Check this vehicle is still active
                if ((oVehicle.m_flags & Vehicle.Flags.GoingBack) != Vehicle.Flags.GoingBack)
                { 
                    oVehicles.Add(usVehicleId);
                }
            }

            // Check all vehicles still exist
            List<ushort> oVehiclesToDelete = new List<ushort>();
            foreach (KeyValuePair<ushort, VehicleProgress> oVehicleProgress in m_vehicleProgress)
            {
                if (!oVehicles.Contains(oVehicleProgress.Key))
                {
                    oVehiclesToDelete.Add(oVehicleProgress.Key);
                }
            }

            // Remove deleted vehicles from list
            foreach (ushort usVehicleId in oVehiclesToDelete)
            {
                if (m_vehicleProgress != null && m_vehicleProgress.ContainsKey(usVehicleId))
                {
                    // Now remove from list
                    m_vehicleProgress.Remove(usVehicleId);
                }
            }

            List<ushort> oStops = GetStopList();

            foreach (ushort usVehicleId in oVehicles) 
            {
                Vehicle oVehicle = instance.m_vehicles.m_buffer[usVehicleId];
                int iStopIndex = oStops.IndexOf(oVehicle.m_targetBuilding);

                float fCurrent;
                float fTotal;
                float fProgress = GetVehicleProgress(usVehicleId, oLine.m_stops, out fCurrent, out fTotal);

                VehicleProgress oNewProgress = VehicleProgress.Create(fProgress, oVehicle, iStopIndex);
                if (oNewProgress.m_path == 0)
                {
                    // Skip these as they aren't valid
                    break;
                }

                if (m_vehicleProgress.ContainsKey(usVehicleId))
                {
                    // Existing vehicle
                    VehicleProgress oProgress = m_vehicleProgress[usVehicleId];
                    if (oNewProgress.HasMoved(oProgress))
                    {
                        // Progress has been made (Forward/Backwards, dont care just moved)
                        oNewProgress.m_fTimeStamp = fSimulationTimer2;
                        m_vehicleProgress[usVehicleId] = oNewProgress;
                    }

                    if (ITransportInfoMain.Debug && m_followingVehicle != 0 && m_followingVehicle == usVehicleId)
                    {
                        // Vehicle moved
                        string sText = "Type: " + oLine.Info.m_transportType.ToString() + " Line: " + LineInfoBase.GetSafeLineName(m_iLineId, oLine) + "\r\n";
                        sText += "Vehicles: " + DebugList(oVehicles) + "\r\n";
                        sText += "Stops: " + oStops.Count + "\r\n";
                        sText += "Vehicle: " + usVehicleId + "\r\n";
                        sText += "Node: " + oNewProgress.m_stopIndex + " Progress: " + fProgress + " m_path: " + oNewProgress.m_path + " m_pathPositionIndex: " + oNewProgress.m_pathPositionIndex + "\r\n";
                        sText += "Node: " + oProgress.m_stopIndex + " Progress: " + oProgress.m_fProgress + "m_path: " + oProgress.m_path + " m_pathPositionIndex: " + oProgress.m_pathPositionIndex + "\r\n";
                        Debug.Log(sText);
                    }
                }
                else
                {
                    // New vehicle
                    oNewProgress.m_fTimeStamp = fSimulationTimer2;
                    m_vehicleProgress.Add(usVehicleId, oNewProgress);
                }
            }
        }

        private static float GetVehicleProgress(ushort usVehicleId, ushort usFirstStop, out float fCurrent, out float fTotal)
        {
            var instance = VehicleManager.instance;
            Vehicle oVehicle = instance.m_vehicles.m_buffer[usVehicleId];
            TransportManagerUtils.GetProgressStatus(usVehicleId, usFirstStop, ref oVehicle, out fCurrent, out fTotal);
            return fTotal != 0 ? (float)Math.Round((fCurrent / fTotal * 100), 1) : 0;
        }

        public ushort GetMostStuckVehicleId()
        {
            int iDays;
            return GetMostStuckVehicle(out iDays);
        }

        public ushort GetMostStuckVehicle(out int iDays)
        {
            float fCurrentTime = Utils.GetSimulationTimestamp();
            float fTimeSpan = 0;
            ushort usVehicleId = 0;
            foreach (KeyValuePair<ushort, VehicleProgress> oVehicleProgress in m_vehicleProgress)
            {
                float fNewTimeSpan = Utils.GetTimeSpan(fCurrentTime, oVehicleProgress.Value.m_fTimeStamp);
                if (fNewTimeSpan >= fTimeSpan)
                {
                    fTimeSpan = fNewTimeSpan;
                    usVehicleId = oVehicleProgress.Key;
                }
            }

            iDays = (int)(fTimeSpan / iSIMULATION_TIMER2_DAY_LENGTH);
            return usVehicleId;
        }

        public List<LineIssue> GetStuckVehicles()
        {
            int iMinStuckDays = Math.Min(PublicTransportInstance.GetSettings().WarnVehicleStuckDays, PublicTransportInstance.GetSettings().WarnVehicleMovingSlowlyDays);
            return GetStuckVehicles(iMinStuckDays);
        }

        public List<LineIssue> GetStuckVehicles(int iStuckDays)
        {
            List<LineIssue> stuckVehicles = new List<LineIssue>();
            float fCurrentTime = Utils.GetSimulationTimestamp();
            

            foreach (KeyValuePair<ushort, VehicleProgress> oVehicleProgress in m_vehicleProgress)
            {
                float fTimeSpan = Utils.GetTimeSpan(fCurrentTime, oVehicleProgress.Value.m_fTimeStamp);
                int iDays = (int)(fTimeSpan / iSIMULATION_TIMER2_DAY_LENGTH);
                if (iDays >= iStuckDays)
                {
                    LineIssue oStuckVehicle = new LineIssueStuckVehicle(oVehicleProgress.Key, oVehicleProgress.Value.m_fTimeStamp);
                    oStuckVehicle.m_iLineId = m_iLineId;
                    stuckVehicles.Add(oStuckVehicle);
                }
            }

            return stuckVehicles;
        }

        public List<LineIssue> GetNetworkProblems()
        {
            List<LineIssue> networkProblems = new List<LineIssue>();
            
            if (PublicTransportInstance.GetSettings().WarnLineIssues)
            {
                NetManager netManager = Singleton<NetManager>.instance;
                NetNode netNode;

                List<ushort> stops = GetStopList();
                int i = 0;
                foreach (ushort usStop in stops)
                {
                    netNode = netManager.m_nodes.m_buffer[usStop];

                    Notification.Problem problems = netNode.m_problems;

                    if (problems != Notification.Problem.None)
                    {
                        if (netNode.m_flags != NetNode.Flags.None && (netNode.m_flags & NetNode.Flags.Temporary) == 0)
                        {
                            LineIssue oIssue = new LineIssueBrokenStop(i + 1, usStop, problems);
                            oIssue.m_iLineId = m_iLineId;
                            networkProblems.Add(oIssue);

                        }
                    }

                    i++;
                }
            }
            
            return networkProblems;
        }

        public List<LineIssue> GetLineIssues()
        {
            List<LineIssue> oLineIssues = new List<LineIssue>();
            oLineIssues.AddRange(GetStuckVehicles());
            oLineIssues.AddRange(GetNetworkProblems());

            return oLineIssues;
        }

        public string DebugList(List<ushort> oList)
        {
            string sText = "";
            foreach (ushort usValue in oList)
            {
                sText += usValue + ", ";
            }
            return sText;
        }

        public List<ushort> GetStopList()
        {
            List<ushort> oList = new List<ushort>();
            TransportLine oLine = TransportManager.instance.m_lines.m_buffer[m_iLineId];
            int iStopCount = oLine.CountStops(m_iLineId);
            for (int i = 0; i < iStopCount; i++)
            {
                oList.Add(oLine.GetStop(i));
            }

            return oList;
        }
    }
}
