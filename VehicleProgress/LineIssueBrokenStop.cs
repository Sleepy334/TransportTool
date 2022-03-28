using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PublicTransportInfo
{
    public class LineIssueBrokenStop : LineIssue
    {
        public ushort m_usStop;
        public int m_iStopNumber;
        Notification.Problem m_eProblem;

        public LineIssueBrokenStop(int iStopNumber, ushort usStop, Notification.Problem eProblem) : base()
        {
            m_usStop = usStop;
            m_iStopNumber = iStopNumber;
            m_eProblem = eProblem;
        }

        public override IssueLevel GetLevel()
        { 
            if (m_eProblem != Notification.Problem.None)
            {
                return IssueLevel.ISSUE_WARNING;
            }
            else
            {
                return IssueLevel.ISSUE_NONE;
            }
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
                return "Issue: Problem resolved";
            }
            else
            {
                return "Issue: " + m_eProblem.ToString();
            }
        }

        public override string GetIssueTooltip()
        {
            return "Stop " + m_iStopNumber + " has a problem (" + m_eProblem.ToString() + ")";
        }

        public override void Update()
        {
            NetNode netNode = NetManager.instance.m_nodes.m_buffer[m_usStop];
            m_eProblem = netNode.m_problems;
        }

        public override void ShowIssue()
        {
            // Close the vehicle panel if open so we can move elsewhere.
            WorldInfoPanel.Hide<PublicTransportVehicleWorldInfoPanel>();
            PublicTransportVehicleButton.cameraController.ClearTarget();

            InstanceID oInstanceId = new InstanceID { TransportLine = (ushort)m_iLineId };
            Vector3 oStopPosition = Singleton<NetManager>.instance.m_nodes.m_buffer[m_usStop].m_position;
            PublicTransportVehicleButton.cameraController.SetTarget(oInstanceId, oStopPosition, true);

            // Open transport line panel
            WorldInfoPanel.Show<PublicTransportWorldInfoPanel>(oStopPosition, oInstanceId);
        }

        public virtual bool Equals(LineIssueBrokenStop oSecond)
        {
            return base.Equals(oSecond) && m_usStop == oSecond.m_usStop;
        }

        public override bool IsDespawned()
        {
            return false;
        }

        public override bool CanDelete()
        {
            return GetLevel() == IssueLevel.ISSUE_NONE;
        }
    }
}
