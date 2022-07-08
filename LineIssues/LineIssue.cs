using ColossalFramework.UI;
using SleepyCommon;
using System;
using System.Collections.Generic;

namespace PublicTransportInfo
{
    public abstract class LineIssue : ListData, IEquatable<LineIssue>
    {
        private static int s_iIssueId = 1;
        private DateTime m_CreationTime;
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
        protected TransportInfo.TransportType m_transportType;

        public LineIssue(ushort iLineId, TransportInfo.TransportType eType)
        {
            m_iIssueId = s_iIssueId++;
            m_iLineId = iLineId;
            m_transportType = eType;
            m_CreationTime = DateTime.Now;
            m_ResolvedTime = DateTime.MaxValue;
        }

        public override int CompareTo(object second)
        {
            if (second == null)
            {
                return 1;
            }
            LineIssue oSecond = (LineIssue)second;
            if (oSecond.m_CreationTime == m_CreationTime)
            {
                return oSecond.m_iIssueId.CompareTo(m_iIssueId);
            }
            else
            {
                return oSecond.m_CreationTime.CompareTo(m_CreationTime); // Descending
            }
        }

        public abstract IssueType GetIssueType();
        public abstract IssueLevel GetLevel();

        public virtual string GetLineDescription()
        {
            return TransportManagerUtils.GetSafeLineName(m_iLineId);
        }
        public abstract ushort GetVehicleId();
        public abstract string GetIssueLocation();
        public abstract string GetIssueDescription();
        public abstract string GetIssueTooltip();
        public abstract void ShowIssue();
        

        public void SetResolved()
        {
            m_ResolvedTime = DateTime.Now;
        }

        public bool IsResolved()
        {
            return m_ResolvedTime != DateTime.MaxValue;
        }

        public virtual bool CanDelete()
        {
            return (IsResolved() && (DateTime.Now - m_ResolvedTime).TotalSeconds > 20);
        }

        public virtual bool Equals(LineIssue oSecond)
        {
            return GetIssueType() == oSecond.GetIssueType() && m_iLineId == oSecond.m_iLineId;
        }

        protected void UpdateTimeStamp()
        {
            m_CreationTime = DateTime.Now;
        }

        public override string GetText(NewListViewRowComparer.Columns eColumn)
        {
            switch (eColumn)
            {
                case NewListViewRowComparer.Columns.COLUMN_TIME: return m_CreationTime.ToString("h:mm:ss");
                case NewListViewRowComparer.Columns.COLUMN_TYPE: return m_transportType.ToString();
                case NewListViewRowComparer.Columns.COLUMN_SOURCE: return GetLineDescription();
                case NewListViewRowComparer.Columns.COLUMN_LOCATION: return GetIssueLocation();
                case NewListViewRowComparer.Columns.COLUMN_DESCRIPTION: return GetIssueDescription();
            }
            return "";
        }

        public override void CreateColumns(NewListViewRow oRow, List<NewListViewRowColumn> m_columns)
        {
            oRow.AddColumn(NewListViewRowComparer.Columns.COLUMN_TIME, GetText(NewListViewRowComparer.Columns.COLUMN_TIME), "", LineIssuePanel.iCOLUMN_WIDTH_TIME, UIHorizontalAlignment.Left, UIAlignAnchor.TopLeft);
            oRow.AddColumn(NewListViewRowComparer.Columns.COLUMN_TYPE, GetText(NewListViewRowComparer.Columns.COLUMN_TYPE), "", LineIssuePanel.iCOLUMN_WIDTH_NORMAL, UIHorizontalAlignment.Left, UIAlignAnchor.TopLeft);
            oRow.AddColumn(NewListViewRowComparer.Columns.COLUMN_SOURCE, GetText(NewListViewRowComparer.Columns.COLUMN_SOURCE), "", LineIssuePanel.iCOLUMN_VEHICLE_WIDTH, UIHorizontalAlignment.Left, UIAlignAnchor.TopLeft);
            oRow.AddColumn(NewListViewRowComparer.Columns.COLUMN_LOCATION, GetText(NewListViewRowComparer.Columns.COLUMN_LOCATION), "", LineIssuePanel.iCOLUMN_VEHICLE_WIDTH, UIHorizontalAlignment.Left, UIAlignAnchor.TopRight);
            oRow.AddColumn(NewListViewRowComparer.Columns.COLUMN_DESCRIPTION, GetText(NewListViewRowComparer.Columns.COLUMN_DESCRIPTION), "", LineIssuePanel.iCOLUMN_DESCRIPTION_WIDTH, UIHorizontalAlignment.Left, UIAlignAnchor.TopRight);
        }

        public override void OnClick(NewListViewRowColumn column)
        {
            ShowIssue();
        }
    }
}
