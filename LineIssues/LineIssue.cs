using ColossalFramework.UI;
using PublicTransportInfo.Util;
using SleepyCommon;
using System;
using System.Collections.Generic;

namespace PublicTransportInfo
{
    public abstract class LineIssue : IEquatable<LineIssue>, IComparable
    {
        private static int s_iIssueId = 1;
        public DateTime m_CreationTime;
        private DateTime m_ResolvedTime;

        public enum IssueType
        {
            ISSUE_TYPE_NONE,
            ISSUE_TYPE_BROKEN_STOP,
            ISSUE_TYPE_DESPAWNED,
            ISSUE_TYPE_STUCK_VEHICLE,
            ISSUE_TYPE_BLOCKED,
            ISSUE_TYPE_BORED_COUNT,
        }

        public enum IssueLevel {
            ISSUE_NONE,
            ISSUE_INFORMATION,
            ISSUE_WARNING,
        }

        public int m_iIssueId;
        public ushort m_iLineId;
        public TransportInfo.TransportType m_transportType;

        public LineIssue(ushort iLineId, TransportInfo.TransportType eType)
        {
            m_iIssueId = s_iIssueId++;
            m_iLineId = iLineId;
            m_transportType = eType;
            m_CreationTime = DateTime.Now;
            m_ResolvedTime = DateTime.MaxValue;
        }

        public int Compare(LineIssue x, LineIssue y)
        {
            if (y.m_CreationTime == x.m_CreationTime)
            {
                return y.m_iIssueId.CompareTo(x.m_iIssueId);
            }
            else
            {
                return y.m_CreationTime.CompareTo(x.m_CreationTime); // Descending
            }
        }

        public int CompareTo(object second)
        {
            if (second == null)
            {
                return 1;
            }
            LineIssue oSecond = (LineIssue)second;
            return Compare(this, oSecond);
        }

        public virtual string GetCreationTime()
        {
            return m_CreationTime.ToString("h:mm:ss");
        }

        public virtual string GetTransportType()
        {
            return m_transportType.ToString();
        }

        public abstract IssueType GetIssueType();
        public abstract IssueLevel GetLevel();

        public virtual string GetLineDescription()
        {
            return TransportManagerUtils.GetSafeLineName(m_iLineId);
        }
        public abstract ushort GetVehicleId();
        public abstract string GetIssueLocation();
        
        public abstract string GetIssueTooltip();
        public abstract void ShowIssue();
        
        public virtual void Update() {}

        public virtual string GetIssueDescription()
        {
            if (IsResolved())
            {
                return Localization.Get("txtResolved") + " (" + GetResolved() + ")";
            }
            else
            {
                return "";
            }
        }

        public void SetResolved()
        {
            if (!IsResolved())
            {
                m_ResolvedTime = DateTime.Now;
            }
        }

        public bool IsResolved()
        {
            return m_ResolvedTime != DateTime.MaxValue;
        }

        public string GetResolved()
        {
            return m_ResolvedTime.ToString("h:mm:ss");
        }

        public void ClearResolved()
        {
            m_ResolvedTime = DateTime.MaxValue;
        }

        public virtual bool CanDelete()
        {
            return (IsResolved() && (DateTime.Now - m_ResolvedTime).TotalSeconds >= ModSettings.GetSettings().DeleteResolvedDelay);
        }

        public virtual bool Equals(LineIssue oSecond)
        {
            return GetIssueType() == oSecond.GetIssueType() && m_iLineId == oSecond.m_iLineId;
        }

        protected void UpdateTimeStamp()
        {
            m_CreationTime = DateTime.Now;
        }
    }
}
