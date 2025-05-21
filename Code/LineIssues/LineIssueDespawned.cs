using UnityEngine;

namespace PublicTransportInfo
{
    public class LineIssueDespawned : LineIssueVehicle
    {
        public Vector3 m_despawnedPosition;

        public LineIssueDespawned(ushort iLineId, TransportInfo.TransportType eType, ushort usVehicleId, Vector3 oDespawnPosition) : base(iLineId, eType, usVehicleId)
        {
            m_iLineId = iLineId;
            m_despawnedPosition = oDespawnPosition;
            SetResolved();
        }

        public override IssueType GetIssueType()
        {
            return IssueType.ISSUE_TYPE_DESPAWNED;
        }

        public override IssueLevel GetLevel()
        {
            return IssueLevel.ISSUE_WARNING;
        }

        public override string GetIssueDescription()
        {
            return "Vehicle has despawned";
        }
        public override string GetIssueTooltip()
        {
            return "Vehicle " + m_sVehicleName + " (" + m_vehicleId + ") has despawned";
        }

        public override void Update()
        {
            // Nothing to do.
        }

        public override Vector3 GetPosition()
        {
            return m_despawnedPosition;
        }

        public override void ShowIssue()
        {
            // Close the vehicle panel if open so we can move elsewhere.
            WorldInfoPanel.Hide<PublicTransportVehicleWorldInfoPanel>();
            PublicTransportVehicleButton.cameraController.ClearTarget();

            // Zoom in on location where vehicle despawned.
            ToolsModifierControl.cameraController.m_targetPosition = m_despawnedPosition;
            ModSettings oSettings = ModSettings.GetSettings();
            if (oSettings.ZoomInOnTarget)
            {
                ToolsModifierControl.cameraController.m_targetAngle.y = 45f;
                ToolsModifierControl.cameraController.m_targetSize = 100f;
            }
        }
    }
}
