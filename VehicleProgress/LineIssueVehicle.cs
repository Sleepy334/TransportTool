using ColossalFramework;

namespace PublicTransportInfo
{
    public abstract class LineIssueVehicle : LineIssue
    {
        public ushort m_vehicleId;
        public string m_sVehicleName;

        public LineIssueVehicle(ushort usVehicleId) : base()
        {
            m_vehicleId = usVehicleId;
            m_sVehicleName = Singleton<VehicleManager>.instance.GetVehicleName(m_vehicleId);
        }

        public override string GetIssueLocation()
        {
            return "Vehicle: " + m_sVehicleName + " (" + m_vehicleId + ") ";
        }

        public virtual bool Equals(LineIssueVehicle oSecond)
        {
            return base.Equals(oSecond) && m_vehicleId == oSecond.m_vehicleId;
        }
    }
}
