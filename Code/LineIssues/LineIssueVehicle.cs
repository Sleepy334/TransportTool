using ColossalFramework;
using ColossalFramework.UI;
using SleepyCommon;
using UnityEngine;

namespace PublicTransportInfo
{
    public abstract class LineIssueVehicle : LineIssue
    {   
        public ushort m_vehicleId;
        public string m_sVehicleName;

        // ----------------------------------------------------------------------------------------
        public LineIssueVehicle(ushort iLineId, TransportInfo.TransportType eType, ushort usVehicleId) : base(iLineId, eType)
        {
            m_vehicleId = usVehicleId;
            m_sVehicleName = Singleton<VehicleManager>.instance.GetVehicleName(m_vehicleId); // Store it incase vehicle is despawning
        }

        public override ushort GetVehicleId()
        {
            return m_vehicleId;
        }

        public override string GetIssueLocation()
        {
#if DEBUG
            return m_sVehicleName + " (" + m_vehicleId + ") ";
#else
            return m_sVehicleName;
#endif
        }

        public override bool Equals(LineIssue oSecond)
        {
            bool bEquals = base.Equals(oSecond);
            if (bEquals)
            {
                return m_vehicleId == ((LineIssueVehicle)oSecond).m_vehicleId;
            }
            return false;
        }

        public override void ShowIssue()
        {
            InstanceID target = new InstanceID { Vehicle = m_vehicleId };

            // Close the vehicle panel if open so we can move elsewhere.
            WorldInfoPanel.Hide<PublicTransportVehicleWorldInfoPanel>();

            if (m_transportType == TransportInfo.TransportType.Metro)
            {
                // Turn on public transport mode so you can see the underground vehicles
                Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.Transport, InfoManager.SubInfoMode.Default);
                UIView.library.Hide("PublicTransportInfoViewPanel");
            }

            // Move camera to correct position
            InstanceHelper.ShowInstance(target, ModSettings.GetSettings().ZoomInOnTarget);

            // Open vehicle details
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                Vehicle oVehicle = VehicleManager.instance.m_vehicles.m_buffer[m_vehicleId];
                Vector3 oVehiclePosition = VehiclePosition.GetVehiclePosition(oVehicle);

                WorldInfoPanel.Show<PublicTransportVehicleWorldInfoPanel>(oVehiclePosition, target);
            }
        }
    }
}
