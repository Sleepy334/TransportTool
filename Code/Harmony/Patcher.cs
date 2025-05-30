﻿using System;
using System.Collections.Generic;
using HarmonyLib;
using SleepyCommon;

namespace PublicTransportInfo
{
    public static class Patcher {
        public const string HarmonyId = "Sleepy.TransportTool";

        private static bool s_patched = false;
        private static int s_iHarmonyPatches = 0;

        public static int GetHarmonyPatchCount() { return s_iHarmonyPatches; }

        public static void PatchAll() {
            if (!s_patched)
            {
                UnityEngine.Debug.Log("TransportTool: Patching...");

                s_patched = true;
                var harmony = new Harmony(HarmonyId);

                List<Type> patchList = new List<Type>();

                // VehicleManager.ReleaseVehicle
                patchList.Add(typeof(Patch.VehicleManagerPatch));

                // General patches
                patchList.Add(typeof(Patch.EscapePatch));

                s_iHarmonyPatches = patchList.Count;

                string sMessage = "Patching the following functions:\r\n";
                foreach (var patchType in patchList)
                {
                    sMessage += patchType.ToString() + "\r\n";
                    harmony.CreateClassProcessor(patchType).Patch();
                }
                CDebug.Log(sMessage);
            }
        }

        public static void UnpatchAll() {
            if (s_patched)
            {
                var harmony = new Harmony(HarmonyId);
                harmony.UnpatchAll(HarmonyId);
                s_patched = false;

                CDebug.Log("TransportTool: Unpatching...");
            }
        }

        public static int GetPatchCount()
        {
            var harmony = new Harmony(HarmonyId);
            var methods = harmony.GetPatchedMethods();
            int i = 0;
            foreach (var method in methods)
            {
                var info = Harmony.GetPatchInfo(method);
                if (info.Owners?.Contains(harmony.Id) == true)
                {
                    CDebug.Log($"Harmony patch method = {method.FullDescription()}");
                    if (info.Prefixes.Count != 0)
                    {
                        CDebug.Log("Harmony patch method has PreFix");
                    }
                    if (info.Postfixes.Count != 0)
                    {
                        CDebug.Log("Harmony patch method has PostFix");
                    }
                    i++;
                }
            }

            return i;
        }
    }
}
