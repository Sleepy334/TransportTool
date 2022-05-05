using ColossalFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public class LineIssueDetector
    {
        public ushort m_iLineId; 
        public static ushort m_followingVehicle = 0;

        public LineIssueDetector(ushort usLineId)
        {
            m_iLineId = usLineId;
        }

        public int GetCount()
        {
            TransportLine oLine = TransportManager.instance.m_lines.m_buffer[m_iLineId];
            return oLine.CountVehicles((ushort)m_iLineId);
        }

        public TransportInfo.TransportType GetTransportType()
        {
            TransportLine oLine = TransportManager.instance.m_lines.m_buffer[m_iLineId];
            return oLine.Info.m_transportType;
        }

        public List<ushort> GetVehicles()
        {
            List<ushort> list = new List<ushort>();

            TransportLine oLine = TransportManager.instance.m_lines.m_buffer[m_iLineId];
            int iVehicleCount = oLine.CountVehicles((ushort)m_iLineId);
            for (int i = 0; i < iVehicleCount; ++i)
            {
                list.Add(oLine.GetVehicle(i));
            }
            return list;
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

        public void DespawnVehicle(ushort usVehicleId, Vehicle oVehicle)
        {
            // This is now called by ReleaseVehicleHook
            if (oVehicle.m_transportLine == m_iLineId)
            {
                if (PublicTransportInstance.GetSettings().WarnVehicleDespawed)
                {
                    // Add to despawned list
                    LineIssue oLineIssue = new LineIssueDespawned(m_iLineId, GetTransportType(), usVehicleId, GetVehiclePosition(oVehicle));
                    oLineIssue.m_iLineId = m_iLineId; 
                    PublicTransportInstance.GetLineIssueManager().AddLineIssue(oLineIssue, true);
                }
            }
        }

        public static float GetVehicleProgress(ushort usVehicleId, ushort usFirstStop, out float fCurrent, out float fTotal, int iDecimals = 1)
        {
            var instance = VehicleManager.instance;
            Vehicle oVehicle = instance.m_vehicles.m_buffer[usVehicleId];
            if (TransportManagerUtils.GetProgressStatus(usVehicleId, usFirstStop, ref oVehicle, out fCurrent, out fTotal))
            {
                return fTotal != 0 ? (float)Math.Round((fCurrent / fTotal * 100), iDecimals) : 0;
            }
            else
            {
                return -1.0f; // invalid
            }
        }

        public List<LineIssue> GetBlockedVehicles()
        {
            List<LineIssue> blockedVehicles = new List<LineIssue>();
            ModSettings oSettings = PublicTransportInstance.GetSettings();
            if (oSettings.WarnVehicleMovesSlowly || oSettings.WarnVehicleStopsMoving)
            {
                int iWarnValue = oSettings.GetBlockedVehicleMinThreshold();
                List<ushort> oVehicles = GetVehicles();
                foreach (ushort usVehicle in oVehicles) 
                {
                    Vehicle oVehicle = VehicleManager.instance.m_vehicles.m_buffer[usVehicle];
                    ushort blocked = oVehicle.m_blockCounter;
                    if (blocked >= iWarnValue)
                    {
                        LineIssue oBlockedVehicle = new LineIssueBlocked(m_iLineId, GetTransportType(), usVehicle);
                        oBlockedVehicle.m_iLineId = m_iLineId;
                        blockedVehicles.Add(oBlockedVehicle);
                    }
                }
            }

            return blockedVehicles;
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
                            LineIssue oIssue = new LineIssueBrokenStop(m_iLineId, GetTransportType(), i + 1, usStop, problems);
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
            oLineIssues.AddRange(GetBlockedVehicles());
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
