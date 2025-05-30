using ColossalFramework;
using SleepyCommon;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public abstract class LineIssueStop : LineIssue
    {
        public ushort m_usStop;
        public int m_iStopNumber;

        // ----------------------------------------------------------------------------------------
        public LineIssueStop(ushort iLineId, TransportInfo.TransportType eType, int iStopNumber, ushort usStop) : 
            base(iLineId, eType)
        {
            m_usStop = usStop;
            m_iStopNumber = iStopNumber;
        }

        public override ushort GetVehicleId()
        {
            return 0;
        }

        public override string GetIssueLocation()
        {
            return "Stop: " + m_iStopNumber;
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

        public override void ShowIssue()
        {
            // Close the vehicle panel if open so we can move elsewhere.
            WorldInfoPanel.Hide<PublicTransportVehicleWorldInfoPanel>();

            // Move camera to correct position
            InstanceHelper.ShowInstance(new InstanceID { NetNode = m_usStop }, ModSettings.GetSettings().ZoomInOnTarget);

            // Open transport line panel
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                Vector3 oStopPosition = Singleton<NetManager>.instance.m_nodes.m_buffer[m_usStop].m_position;
                WorldInfoPanel.Show<PublicTransportWorldInfoPanel>(oStopPosition, new InstanceID { TransportLine = (ushort)m_iLineId });
            }
        }

        public virtual bool Equals(LineIssueBrokenStop oSecond)
        {
            return base.Equals(oSecond) && m_usStop == oSecond.m_usStop;
        }
    }
}
