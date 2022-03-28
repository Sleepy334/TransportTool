using PublicTransportInfo;

namespace PublicTransportInfo.HarmonyPatches
{
    public class VehicleManagerPatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(VehicleManager), nameof(VehicleManager.ReleaseVehicle)),
                new PatchUtil.MethodDefinition(typeof(VehicleManagerPatch), nameof(PrefixReleaseVehicle))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(VehicleManager), nameof(VehicleManager.ReleaseVehicle))
            );
        }

        //public void RemoveVehicle(ushort vehicleID, ref Vehicle data)

        public static bool PrefixReleaseVehicle(System.UInt16 vehicle)
        {
            LineIssueManager oManager = PublicTransportInstance.GetLineIssueManager();
            if (oManager != null)
            {
                oManager.DespawnVehicle(vehicle);
            }

            return true;
        }
    }
}