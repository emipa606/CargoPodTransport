using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Helicopter;

[HarmonyPatch(typeof(TransportersArrivalAction_LandInSpecificCell),
    nameof(TransportersArrivalAction_LandInSpecificCell.Arrived))]
public static class TransportPodsArrivalAction_LandInSpecificCell_Arrived
{
    public static bool Prefix(TransportersArrivalAction_LandInSpecificCell __instance,
        List<ActiveTransporterInfo> transporters)
    {
        foreach (var info in transporters)
        {
            if (!info.innerContainer.Contains(ThingDef.Named("Building_Helicopter")))
            {
                continue;
            }

            var lookTarget = TransportersArrivalActionUtility.GetLookTarget(transporters);
            var tv = Traverse.Create(__instance);
            var c = tv.Field("cell").GetValue<IntVec3>();
            var map = tv.Field("mapParent").GetValue<MapParent>().Map;
            TransportersArrivalActionUtility.RemovePawnsFromWorldPawns(transporters);
            foreach (var activeDropPodInfo in transporters)
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