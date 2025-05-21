using ColossalFramework;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public class LineIssueBoredCount : LineIssue
    {
        public ushort m_usStop;
        public int m_iStopNumber;
        public int m_iBoredCount;

        public LineIssueBoredCount(ushort iLineId, TransportInfo.TransportType eType, int iStopNumber, ushort usStop, int iBored) 
            : base(iLineId, eType)
        {
            m_usStop = usStop;
            m_iStopNumber = iStopNumber;
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

        public override ushort GetVehicleId()
        {
            return 0;
        }

        public override string GetIssueLocation()
        {
            return "Stop: " + m_iStopNumber;
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
