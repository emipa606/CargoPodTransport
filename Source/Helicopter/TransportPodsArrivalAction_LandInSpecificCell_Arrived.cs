using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Helicopter;

[HarmonyPatch(typeof(TransportPodsArrivalAction_LandInSpecificCell),
    nameof(TransportPodsArrivalAction_LandInSpecificCell.Arrived))]
public static class TransportPodsArrivalAction_LandInSpecificCell_Arrived
{
    public static bool Prefix(TransportPodsArrivalAction_LandInSpecificCell __instance, List<ActiveDropPodInfo> pods)
    {
        foreach (var info in pods)
        {
            if (!info.innerContainer.Contains(ThingDef.Named("Building_Helicopter")))
            {
                continue;
            }

            var lookTarget = TransportPodsArrivalActionUtility.GetLookTarget(pods);
            var tv = Traverse.Create(__instance);
            var c = tv.Field("cell").GetValue<IntVec3>();
            var map = tv.Field("mapParent").GetValue<MapParent>().Map;
            TransportPodsArrivalActionUtility.RemovePawnsFromWorldPawns(pods);
            foreach (var activeDropPodInfo in pods)
            {
                DropPodUtility.MakeDropPodAt(c, map, activeDropPodInfo);
            }

            Messages.Message("MessageTransportPodsArrived".Translate(), lookTarget,
                MessageTypeDefOf.TaskCompletion);
            return false;
        }

        return true;
    }
}