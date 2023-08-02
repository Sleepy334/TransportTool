using HarmonyLib;

namespace PublicTransportInfo.Patch
{
    [HarmonyPatch(typeof(VehicleManager), "ReleaseVehicle")]
    public class VehicleManagerPatch
    {
        public static bool PrefixReleaseVehicle(ushort vehicle)
        {
            if (LineIssueManager.Instance != null)
            {
                LineIssueManager.Instance.DespawnVehicle(vehicle);
            }

            return true;
        }
    }
}