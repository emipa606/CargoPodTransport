using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Helicopter;

[HarmonyPatch(typeof(Dialog_LoadTransporters), "AddPawnsToTransferables")]
public static class Dialog_LoadTransporters_AddPawnsToTransferables
{
    public static bool Prefix(Dialog_LoadTransporters __instance, List<CompTransporter> ___transporters, Map ___map)
    {
        foreach (var lpc in ___transporters)
        {
            if (lpc.parent.TryGetComp<CompLaunchableHelicopter>() == null)
            {
                continue;
            }

            var list = CaravanFormingUtility.AllSendablePawns(___map, true, true, true, true);
            foreach (var pawn in list)
            {
                var typ = __instance.GetType();
                var minfo = typ.GetMethod("AddToTransferables", BindingFlags.NonPublic | BindingFlags.Instance);
                minfo?.Invoke(__instance, [pawn]);
            }

            return false;
        }

        return true;
    }
}