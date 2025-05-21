using ColossalFramework;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public class LineIssueBrokenStop : LineIssue
    {
        public ushort m_usStop;
        public int m_iStopNumber;
        Notification.Problem1 m_eProblem;

        public LineIssueBrokenStop(ushort iLineId, TransportInfo.TransportType eType, int iStopNumber, ushort usStop, Notification.Problem1 eProblem) : base(iLineId, eType)
        {
            m_usStop = usStop;
            m_iStopNumber = iStopNumber;
            m_eProblem = eProblem;
        }

        public override IssueType GetIssueType()
        {
            return IssueType.ISSUE_TYPE_BROKEN_STOP;
        }

        public override IssueLevel GetLevel()
        { 
            if (m_eProblem != Notification.Problem1.None)
            {
                return IssueLevel.ISSUE_WARNING;
            }
            else
            {
                return IssueLevel.ISSUE_NONE;
            }
        }

        public override ushort GetVehicleId()
        {
            return 0;
        }

        public override string GetIssueLocation()
        {
            TransportLine oLine = TransportManager.instance.m_lines.m_buffer[m_iLineId];
            return "Stop: " + m_iStopNumber;
        }

        public override string GetIssueDescription()
        {
            if (GetLevel() == IssueLevel.ISSUE_NONE)
            {
                return base.GetIssueDescription();
            }
            else
            {
                return m_eProblem.ToString();
            }
        }

        public override string GetIssueTooltip()
        {
            return $"{GetTransportType()}:{GetLineDescription()} - {Localization.Get("txtStop")}{m_iStopNumber} - {m_eProblem}";
        }

        public override Vector3 GetPosition()
        {
            NetNode netNode = NetManager.instance.m_nodes.m_buffer[m_usStop];
            return netNode.m_position;
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

        public override void Update()
        {
            IssueLevel eLevel = GetLevel();
            
            // Check node is still part of line
            List<ushort> oList = GetStopList();
            if (oList.Contains(m_usStop))
            {
                // Does it still have an issue
                NetNode netNode = NetManager.instance.m_nodes.m_buffer[m_usStop];
                if (netNode.m_flags != 0 && netNode.m_problems.IsNotNone)
                {
                    m_eProblem = netNode.m_problems;
                }
                else
                {
                    m_eProblem = Notification.Problem1.None;
                }
            }
            else
            {
                m_eProblem = Notification.Problem1.None;
            }
            
            // Issue has changed restart deletion time stamp
            if (eLevel != GetLevel())
            {
                UpdateTimeStamp();
            }

            if (!IsResolved() && GetLevel() == IssueLevel.ISSUE_NONE)
            {
                SetResolved();
            }
        }

        public override void ShowIssue()
        {
            // Close the vehicle panel if open so we can move elsewhere.
            WorldInfoPanel.Hide<PublicTransportVehicleWorldInfoPanel>();
            PublicTransportVehicleButton.cameraController.ClearTarget();

            InstanceID oInstanceId = new InstanceID { TransportLine = (ushort)m_iLineId };
            Vector3 oStopPosition = Singleton<NetManager>.instance.m_nodes.m_buffer[m_usStop].m_position;
            ModSettings oSettings = ModSettings.GetSettings();
            PublicTransportVehicleButton.cameraController.SetTarget(oInstanceId, oStopPosition, oSettings.ZoomInOnTarget);

            // Open transport line panel
            WorldInfoPanel.Show<PublicTransportWorldInfoPanel>(oStopPosition, oInstanceId);
        }

        public virtual bool Equals(LineIssueBrokenStop oSecond)
        {
            return base.Equals(oSecond) && m_usStop == oSecond.m_usStop;
        }
    }
}
