using ColossalFramework;

namespace PublicTransportInfo
{
    public abstract class LineIssueVehicle : LineIssue
    {   
        public ushort m_vehicleId;
        public string m_sVehicleName;

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
    }
}
