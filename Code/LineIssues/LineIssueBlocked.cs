using ColossalFramework;
using ColossalFramework.UI;
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
            if (oVehicle.m_flags != 0)
            {
                return oVehicle.m_blockCounter;
            }
            else
            {
                return 0;
            }
        }

        public override IssueLevel GetLevel()
        {
            ModSettings oSettings = ModSettings.GetSettings();
            byte blockCounter = GetBlockCounter();

            if (oSettings.WarnVehicleStopsMoving && blockCounter > oSettings.WarnVehicleBlockedThreshold)
            {
                m_eLevel = IssueLevel.ISSUE_WARNING;
                ClearResolved();
            }
            else if (oSettings.WarnVehicleMovesSlowly && blockCounter > oSettings.WarnVehicleMovingSlowlyThreshold) 
            {
                m_eLevel = IssueLevel.ISSUE_INFORMATION;
                ClearResolved();
            }
            else
            {
                SetResolved();
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
                        return "Vehicle is blocked (" + blockCounter + ")";
                    }
                case IssueLevel.ISSUE_NONE:
                default:
                    return base.GetIssueDescription();
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
                        sTooltip = m_sVehicleName + " (" + m_vehicleId + ")" + " is " + Localization.Get("txtBlocked") + "(" + blockCounter + ")";
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

        public override Vector3 GetPosition()
        {
            Vehicle vehicle = VehicleManager.instance.m_vehicles.m_buffer[m_vehicleId];
            return vehicle.GetLastFramePosition();
        }

        public override void Update()
        {
            // Set resolved if needed
            GetLevel();
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

            ModSettings oSettings = ModSettings.GetSettings();
            ushort usStuckVehicleId = m_vehicleId;
            Vehicle oVehicle = VehicleManager.instance.m_vehicles.m_buffer[usStuckVehicleId];
            Vector3 oVehiclePosition = VehiclePosition.GetVehiclePosition(oVehicle);
            InstanceID oInstanceId = new InstanceID { Vehicle = usStuckVehicleId };

            PublicTransportVehicleButton.cameraController.SetTarget(oInstanceId, oVehiclePosition, oSettings.ZoomInOnTarget);

            // Open vehicle details
            WorldInfoPanel.Show<PublicTransportVehicleWorldInfoPanel>(oVehiclePosition, oInstanceId);
        }
    }
}
