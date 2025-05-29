using ColossalFramework;
using System;
using System.Collections.Generic; 

namespace PublicTransportInfo
{
    public class LineIssueDetector
    {
        public ushort m_iLineId; 
        public static ushort m_followingVehicle = 0;

        // ----------------------------------------------------------------------------------------
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

        public void DespawnVehicle(ushort usVehicleId, Vehicle oVehicle)
        {
            // This is now called by ReleaseVehicleHook
            if (oVehicle.m_transportLine == m_iLineId)
            {
                if (ModSettings.GetSettings().WarnVehicleDespawed && LineIssueManager.Instance != null)
                {
                    // Add to despawned list
                    LineIssue oLineIssue = new LineIssueDespawned(m_iLineId, GetTransportType(), usVehicleId, oVehicle.GetLastFramePosition());
                    oLineIssue.m_iLineId = m_iLineId;
                    LineIssueManager.Instance.AddLineIssue(oLineIssue, true);
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
            ModSettings oSettings = ModSettings.GetSettings();
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

        public List<LineIssue> GetLineProblems()
        {
            List<LineIssue> networkProblems = new List<LineIssue>();
            
            if (ModSettings.GetSettings().WarnLineIssues)
            {
                NetManager netManager = Singleton<NetManager>.instance;
                NetNode netNode;

                List<ushort> stops = GetStopList();
                int i = 0;
                foreach (ushort usStop in stops)
                {
                    // Node issues
                    netNode = netManager.m_nodes.m_buffer[usStop];
                    Notification.Problem1 problems = netNode.m_problems;
                    if (problems != Notification.Problem1.None)
                    {
                        if (netNode.m_flags != NetNode.Flags.None && (netNode.m_flags & NetNode.Flags.Temporary) == 0)
                        {
                            LineIssue oIssue = new LineIssueBrokenStop(m_iLineId, GetTransportType(), i + 1, usStop, problems);
                            oIssue.m_iLineId = m_iLineId;
                            networkProblems.Add(oIssue);

                        }
                    }

                    // Bored count issues
                    int iBored;
                    int iStopPassengerCount = TransportManagerUtils.CalculatePassengerCount(usStop, GetTransportType(), out iBored);
                    if (iBored >= ModSettings.GetSettings().WarnBoredCountThreshold)
                    {
                        LineIssue oIssue = new LineIssueBoredCount(m_iLineId, GetTransportType(), i + 1, usStop, iBored);
                        oIssue.m_iLineId = m_iLineId;
                        networkProblems.Add(oIssue);
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
            oLineIssues.AddRange(GetLineProblems());

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
