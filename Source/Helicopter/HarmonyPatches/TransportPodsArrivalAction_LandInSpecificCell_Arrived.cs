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
    public static bool Prefix(List<ActiveTransporterInfo> transporters, IntVec3 ___cell, MapParent ___mapParent)
    {
        foreach (var info in transporters)
        {
            if (!info.innerContainer.Contains(ThingDef.Named("Building_Helicopter")))
            {
                continue;
            }

            var lookTarget = TransportersArrivalActionUtility.GetLookTarget(transporters);

            TransportersArrivalActionUtility.RemovePawnsFromWorldPawns(transporters);
            foreach (var activeDropPodInfo in transporters)
            {
                DropPodUtility.MakeDropPodAt(___cell, ___mapParent.Map, activeDropPodInfo);
            }

            Messages.Message("MessageTransportPodsArrived".Translate(), lookTarget,
                MessageTypeDefOf.TaskCompletion);
            return false;
        }

        return true;
    }
}