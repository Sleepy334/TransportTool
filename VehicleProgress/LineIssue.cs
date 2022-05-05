using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PublicTransportInfo
{
    public abstract class LineIssue : IEquatable<LineIssue>
    {
        private static int s_iIssueId = 1;
        private DateTime m_CreationTime;

        public enum IssueType
        {
            ISSUE_TYPE_NONE,
            ISSUE_TYPE_BROKEN_STOP,
            ISSUE_TYPE_DESPAWNED,
            ISSUE_TYPE_STUCK_VEHICLE,
            ISSUE_TYPE_BLOCKED,
        }

        public enum IssueLevel {
            ISSUE_NONE,
            ISSUE_INFORMATION,
            ISSUE_WARNING,
        }

        public int m_iIssueId;
        public ushort m_iLineId;
        protected TransportInfo.TransportType m_transportType;
        private bool m_bHidden;

        public LineIssue(ushort iLineId, TransportInfo.TransportType eType)
        {
            m_iIssueId = s_iIssueId++;
            m_iLineId = iLineId;
            m_transportType = eType;
            m_bHidden = false;
            m_CreationTime = DateTime.Now;
        }

        public abstract IssueType GetIssueType();
        public abstract IssueLevel GetLevel();
        public abstract ushort GetVehcileId();
        public abstract string GetIssueLocation();
        public abstract string GetIssueDescription();
        public abstract string GetIssueTooltip();
        public abstract void Update();
        public abstract void ShowIssue();
        public abstract bool CanDelete();

        public int GetIssueId()
        {
            return m_iIssueId;
        }

        public bool IsHidden()
        {
            return m_bHidden;
        }
        public virtual void SetHidden(bool bHidden)
        {
            m_bHidden = bHidden;
        }

        public virtual bool Equals(LineIssue oSecond)
        {
            return GetIssueType() == oSecond.GetIssueType() && m_iLineId == oSecond.m_iLineId;
        }

        public double GetLineIssueCreationTimeSeconds()
        {
            return (DateTime.Now - m_CreationTime).TotalSeconds;
        }

        protected void UpdateTimeStamp()
        {
            m_CreationTime = DateTime.Now;
        }
    }
}
