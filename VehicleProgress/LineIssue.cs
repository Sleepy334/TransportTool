using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PublicTransportInfo
{
    public abstract class LineIssue : IEquatable<LineIssue>
    {
        public enum IssueLevel {
            ISSUE_NONE,
            ISSUE_INFORMATION,
            ISSUE_WARNING,
        }

        public ushort m_iLineId;
        private bool m_bHidden;

        public LineIssue()
        {
            m_iLineId = 0;
            m_bHidden = false;
        }

        public abstract IssueLevel GetLevel();
        public abstract string GetIssueLocation();
        public abstract string GetIssueDescription();
        public abstract string GetIssueTooltip();
        public abstract void Update();
        public abstract void ShowIssue();
        public abstract bool IsDespawned();
        public abstract bool CanDelete();

        public bool IsHidden()
        {
            return m_bHidden;
        }
        public void SetHidden(bool bHidden)
        {
            m_bHidden = bHidden;
        }

        public virtual bool Equals(LineIssue oSecond)
        {
            return GetType() == oSecond.GetType() && m_iLineId == oSecond.m_iLineId;
        }
    }
}
