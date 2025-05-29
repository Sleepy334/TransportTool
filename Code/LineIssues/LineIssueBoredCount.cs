using SleepyCommon;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public class LineIssueBoredCount : LineIssueStop
    {
        public int m_iBoredCount;

        // ----------------------------------------------------------------------------------------
        public LineIssueBoredCount(ushort iLineId, TransportInfo.TransportType eType, int iStopNumber, ushort usStop, int iBored) 
            : base(iLineId, eType, iStopNumber, usStop)
        {
            m_iBoredCount = iBored;
        }

        public override IssueType GetIssueType()
        {
            return IssueType.ISSUE_TYPE_BORED_COUNT;
        }

        public override IssueLevel GetLevel()
        {
            if (m_iBoredCount >= ModSettings.GetSettings().WarnBoredCountThreshold)
            {
                return IssueLevel.ISSUE_WARNING;
            }
            else
            {
                if (!IsResolved())
                {
                    SetResolved();
                }
                return IssueLevel.ISSUE_NONE;
            }
        }

        public override string GetIssueDescription()
        {
            if (GetLevel() != IssueLevel.ISSUE_NONE)
            {
                return Localization.Get("OverviewBored") + ": " + m_iBoredCount;
            }
            else
            {
                return base.GetIssueDescription();
            }
        }

        public override string GetIssueTooltip()
        {
            return $"{GetTransportType()}:{GetLineDescription()} - {Localization.Get("txtStop")}:{m_iStopNumber} - {Localization.Get("OverviewBored")} ({m_iBoredCount})";
        }

        public override void Update()
        {
            IssueLevel eLevel = GetLevel();

            // Check node is still part of line
            List<ushort> oList = GetStopList();
            if (oList.Contains(m_usStop))
            {
                TransportManagerUtils.CalculatePassengerCount(m_usStop, m_transportType, out m_iBoredCount);
            }
            else
            {
                m_iBoredCount = 0;
            }

            // Issue has changed restart deletion time stamp
            if (eLevel != GetLevel())
            {
                UpdateTimeStamp();
            }
        }
    }
}
