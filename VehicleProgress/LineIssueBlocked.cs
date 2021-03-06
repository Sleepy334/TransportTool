using ColossalFramework;
using ColossalFramework.UI;
using System;
using UnityEngine;

namespace PublicTransportInfo
{
    public class LineIssueBlocked : LineIssueVehicle
    {
        IssueLevel m_eLevel = IssueLevel.ISSUE_NONE;
        public LineIssueBlocked(ushort iLineId, TransportInfo.TransportType eType, ushort usVehicleId) : 
            base(iLineId, eType, usVehicleId)
        {
        }

        public override IssueType GetIssueType()
        {
            return IssueType.ISSUE_TYPE_BLOCKED;
        }

        public byte GetBlockCounter()
        {
            Vehicle oVehicle = VehicleManager.instance.m_vehicles.m_buffer[m_vehicleId];
            return oVehicle.m_blockCounter;
        }

        public override IssueLevel GetLevel()
        {
            ModSettings oSettings = PublicTransportInstance.GetSettings();
            byte blockCounter = GetBlockCounter();

            if (oSettings.WarnVehicleStopsMoving && blockCounter > oSettings.WarnVehicleBlockedThreshold)
            {
                if (m_eLevel != IssueLevel.ISSUE_WARNING)
                {
                    UpdateTimeStamp();
                }
                m_eLevel = IssueLevel.ISSUE_WARNING;
            }
            else if (oSettings.WarnVehicleMovesSlowly && blockCounter > oSettings.WarnVehicleMovingSlowlyThreshold) 
            {
                if (m_eLevel != IssueLevel.ISSUE_INFORMATION)
                {
                    UpdateTimeStamp();
                }
                m_eLevel = IssueLevel.ISSUE_INFORMATION;
            }
            else
            {
                if (m_eLevel != IssueLevel.ISSUE_NONE)
                {
                    UpdateTimeStamp();
                }
                m_eLevel = IssueLevel.ISSUE_NONE;
            }
            return m_eLevel;
        }

        public override string GetIssueDescription()
        {
            byte blockCounter = GetBlockCounter();
            IssueLevel eLevel = GetLevel();
            switch (eLevel)
            {
                case IssueLevel.ISSUE_WARNING:
                case IssueLevel.ISSUE_INFORMATION:
                    {
                        return "Issue: Vehicle is blocked (" + blockCounter + ")";
                    }
                case IssueLevel.ISSUE_NONE:
                default:
                    return "Issue: Resolved, vehicle has moved (" + blockCounter + ")";
            }
        }

        public override string GetIssueTooltip()
        {
            string sTooltip = "";
            byte blockCounter = GetBlockCounter();
            IssueLevel eLevel = GetLevel();
            switch (eLevel)
            {
                case IssueLevel.ISSUE_WARNING:
                case IssueLevel.ISSUE_INFORMATION:
                    {
                        sTooltip = m_sVehicleName + " (" + m_vehicleId + ")" + " is blocked (" + blockCounter + ")";
                        break;
                    }
                case IssueLevel.ISSUE_NONE:
                default:
                    {
                        break;
                    }
            }

            return sTooltip;
        }

        public override void Update()
        {
            // Nothing to do
        }

        public override void ShowIssue()
        {
            // Close the vehicle panel if open so we can move elsewhere.
            WorldInfoPanel.Hide<PublicTransportVehicleWorldInfoPanel>();
            PublicTransportVehicleButton.cameraController.ClearTarget();

            if (m_transportType == TransportInfo.TransportType.Metro) {
                // Turn on public transport mode so you can see the underground vehicles
                Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.Transport, InfoManager.SubInfoMode.Default);
                UIView.library.Hide("PublicTransportInfoViewPanel");
            } 
            else
            {
                Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
            }

            ModSettings oSettings = PublicTransportInstance.GetSettings();
            ushort usStuckVehicleId = m_vehicleId;
            Vehicle oVehicle = VehicleManager.instance.m_vehicles.m_buffer[usStuckVehicleId];
            Vector3 oVehiclePosition = VehiclePosition.GetVehiclePosition(oVehicle);
            InstanceID oInstanceId = new InstanceID { Vehicle = usStuckVehicleId };

            PublicTransportVehicleButton.cameraController.SetTarget(oInstanceId, oVehiclePosition, oSettings.ZoomInOnTarget);

            // Open vehicle details
            WorldInfoPanel.Show<PublicTransportVehicleWorldInfoPanel>(oVehiclePosition, oInstanceId);
        }

        public override bool CanDelete()
        {
            return GetLevel() == IssueLevel.ISSUE_NONE;
        }
    }
}
