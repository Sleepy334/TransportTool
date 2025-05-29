using ColossalFramework;
using SleepyCommon;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public class LineIssueBrokenStop : LineIssueStop
    {
        Notification.Problem1 m_eProblem;

        public LineIssueBrokenStop(ushort iLineId, TransportInfo.TransportType eType, int iStopNumber, ushort usStop, Notification.Problem1 eProblem) : 
            base(iLineId, eType, iStopNumber, usStop)
        {
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

        public override bool Equals(LineIssueBrokenStop oSecond)
        {
            return base.Equals(oSecond) && m_usStop == oSecond.m_usStop;
        }
    }
}
