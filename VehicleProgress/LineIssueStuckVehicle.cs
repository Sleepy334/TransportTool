using UnityEngine;

namespace PublicTransportInfo
{
    public class LineIssueStuckVehicle : LineIssueVehicle
    {
        private float m_fTimeStamp;
        private Vector3 m_position;
        private LineIssueDespawned? m_despawnedIssue = null; // Wraps a despawn issue incase it despawns after warning.
        public LineIssueStuckVehicle(ushort usVehicleId, float fTimeStamp) : base(usVehicleId)
        {
            m_fTimeStamp = fTimeStamp;
            Vehicle oVehicle = VehicleManager.instance.m_vehicles.m_buffer[usVehicleId];
            m_position = VehicleProgress.GetVehiclePosition(oVehicle);
            m_despawnedIssue = null;
        }

        public override bool Equals(LineIssue oSecond)
        {
            if (m_despawnedIssue != null)
            {
                return m_despawnedIssue.Equals(oSecond);
            }
            else
            {
                return base.Equals(oSecond);
            }
        }

        public override IssueLevel GetLevel()
        {
            if (m_despawnedIssue != null)
            {
                return m_despawnedIssue.GetLevel();
            }
            else
            {
                ModSettings oSettings = PublicTransportInstance.GetSettings();
                int iDays = Utils.GetTimestampDays(m_fTimeStamp);
                if (oSettings.WarnVehicleStopsMoving && iDays >= oSettings.WarnVehicleStuckDays)
                {
                    return IssueLevel.ISSUE_WARNING;
                }
                else if (oSettings.WarnVehicleMovesSlowly && iDays >= oSettings.WarnVehicleMovingSlowlyDays)
                {
                    return IssueLevel.ISSUE_INFORMATION;
                }
                else
                {
                    return IssueLevel.ISSUE_NONE;
                }
            }
        }

        public override string GetIssueDescription()
        {
            if (m_despawnedIssue != null)
            {
                return m_despawnedIssue.GetIssueDescription();
            }
            else
            {
                IssueLevel eLevel = GetLevel();
                switch (eLevel)
                {
                    case IssueLevel.ISSUE_WARNING:
                    case IssueLevel.ISSUE_INFORMATION:
                        {
                            int iDays = Utils.GetTimestampDays(m_fTimeStamp);
                            return "Issue: Has not moved for " + iDays + " days";
                        }
                    case IssueLevel.ISSUE_NONE:
                    default:
                        return "Issue: Resolved, vehicle has moved.";
                }
            }
        }

        public override string GetIssueTooltip()
        {
            if (m_despawnedIssue != null)
            {
                return m_despawnedIssue.GetIssueTooltip();
            }
            else
            {
                string sTooltip = "Vehicle (" + m_vehicleId + ") " + m_sVehicleName;

                IssueLevel eLevel = GetLevel();
                switch (eLevel)
                {
                    case IssueLevel.ISSUE_WARNING:
                    case IssueLevel.ISSUE_INFORMATION:
                        {
                            int iDays = Utils.GetTimestampDays(m_fTimeStamp);
                            sTooltip += " has not moved for " + iDays + " days";
                            break;
                        }
                    case IssueLevel.ISSUE_NONE:
                    default:
                        {
                            sTooltip += " resolved, vehicle has moved.";
                            break;
                        }
                }

                return sTooltip;
            }
        }

        public override void Update()
        {
            // Update vehicle time stamp.
            if (m_despawnedIssue == null)
            {
                LineIssueManager oManager = PublicTransportInstance.GetLineIssueManager();
                if (oManager != null && oManager.ContainsLine(m_iLineId))
                {
                    VehicleProgressLine? oProgress = oManager.GetVehicleProgressForLine(m_iLineId);
                    if (oProgress != null)
                    {
                        VehicleProgress? oVehicleProgress = oProgress.GetProgress(m_vehicleId);
                        if (oVehicleProgress != null)
                        {
                            m_fTimeStamp = oVehicleProgress.m_fTimeStamp;
                            Vehicle oVehicle = VehicleManager.instance.m_vehicles.m_buffer[m_vehicleId];
                            m_position = VehicleProgress.GetVehiclePosition(oVehicle);
                        }
                        else
                        {
                            CreateDespawnIssue();
                        }
                    }
                    else
                    {
                        CreateDespawnIssue();
                    }
                }
                else
                {
                    CreateDespawnIssue();
                }
            }
        }

        public override void ShowIssue()
        {
            if (m_despawnedIssue != null)
            {
                m_despawnedIssue.ShowIssue();
            }
            else
            {
                // Close the vehicle panel if open so we can move elsewhere.
                WorldInfoPanel.Hide<PublicTransportVehicleWorldInfoPanel>();
                PublicTransportVehicleButton.cameraController.ClearTarget();

                ushort usStuckVehicleId = m_vehicleId;
                Vehicle oVehicle = VehicleManager.instance.m_vehicles.m_buffer[usStuckVehicleId];
                Vector3 oVehiclePosition = VehicleProgress.GetVehiclePosition(oVehicle);
                InstanceID oInstanceId = new InstanceID { Vehicle = usStuckVehicleId };
                PublicTransportVehicleButton.cameraController.SetTarget(oInstanceId, oVehiclePosition, true);

                // Open vehicle details
                WorldInfoPanel.Show<PublicTransportVehicleWorldInfoPanel>(oVehiclePosition, oInstanceId);
            }
        }

        private void CreateDespawnIssue()
        {
            if (m_despawnedIssue == null)
            {
                m_despawnedIssue = new LineIssueDespawned(m_iLineId, m_vehicleId, m_position);
            }
        }

        public override bool IsDespawned()
        {
            return (m_despawnedIssue != null);
        }

        public override bool CanDelete()
        {
            return m_despawnedIssue != null || GetLevel() == IssueLevel.ISSUE_NONE;
        }
    }
}
