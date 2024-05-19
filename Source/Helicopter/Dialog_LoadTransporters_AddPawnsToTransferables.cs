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
    public static bool Prefix(Dialog_LoadTransporters __instance)
    {
        var tv = Traverse.Create(__instance);
        var lp = tv.Field("transporters").GetValue<List<CompTransporter>>();
        foreach (var lpc in lp)
        {
            if (lpc.parent.TryGetComp<CompLaunchableHelicopter>() == null)
            {
                continue;
            }

            var map = tv.Field("map").GetValue<Map>();
            var list = CaravanFormingUtility.AllSendablePawns(map, true, true, true, true);
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