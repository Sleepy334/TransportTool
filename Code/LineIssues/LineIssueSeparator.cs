
using UnityEngine;

namespace PublicTransportInfo
{
    public class LineIssueSeparator : LineIssue
    {
        public LineIssueSeparator() : 
            base(0, TransportInfo.TransportType.Bus)
        {
        }

        public override string GetCreationTime()
        {
            return "";
        }

        public override string GetTransportType()
        {
            return "";
        }

        public override string GetLineDescription()
        {
            return "";
        }

        public override IssueType GetIssueType()
        {
            return IssueType.ISSUE_TYPE_NONE;
        }

        public override IssueLevel GetLevel()
        {
            return IssueLevel.ISSUE_NONE;
        }

        public override string GetIssueDescription()
        {
            return "";
        }

        public override string GetIssueLocation()
        {
            return "";
        }

        public override string GetIssueTooltip()
        {
            return "";
        }

        public override ushort GetVehicleId()
        {
            return 0;
        }

        public override void Update()
        {
        }

        public override void ShowIssue()
        {
        }
    }
}
