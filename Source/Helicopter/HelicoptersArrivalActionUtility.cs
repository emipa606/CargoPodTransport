﻿using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Helicopter;

public static class HelicoptersArrivalActionUtility
{
    public static IEnumerable<FloatMenuOption> GetFloatMenuOptions<T>(
        Func<FloatMenuAcceptanceReport> acceptanceReportGetter, Func<T> arrivalActionGetter, string label,
        CompLaunchableHelicopter representative, int destinationTile, Caravan car) where T : TransportPodsArrivalAction
    {
        var rep = acceptanceReportGetter();
        if (!rep.Accepted && rep.FailReason.NullOrEmpty() && rep.FailMessage.NullOrEmpty())
        {
            yield break;
        }

        if (!rep.FailReason.NullOrEmpty())
        {
            yield return new FloatMenuOption($"{label} ({rep.FailReason})", null);
        }
        else
        {
            yield return new FloatMenuOption(label, delegate
            {
                var floatMenuAcceptanceReport = acceptanceReportGetter();
                if (floatMenuAcceptanceReport.Accepted)
                {
                    representative.TryLaunch(destinationTile, arrivalActionGetter(), car);
                }
                else if (!floatMenuAcceptanceReport.FailMessage.NullOrEmpty())
                {
                    Messages.Message(floatMenuAcceptanceReport.FailMessage, new GlobalTargetInfo(destinationTile),
                        MessageTypeDefOf.RejectInput, false);
                }
            });
        }
    }


    public static IEnumerable<FloatMenuOption> GetATKFloatMenuOptions(CompLaunchableHelicopter representative,
        IEnumerable<IThingHolder> pods, Settlement settlement, Caravan car)
    {
        foreach (var f in GetFloatMenuOptions(
                     () => TransportPodsArrivalAction_AttackSettlement.CanAttack(pods, settlement),
                     () => new TransportPodsArrivalAction_AttackSettlement(settlement, PawnsArrivalModeDefOf.EdgeDrop),
                     "AttackAndDropAtEdge".Translate(settlement.Label), representative, settlement.Tile, car))
        {
            yield return f;
        }

        foreach (var f2 in GetFloatMenuOptions(
                     () => TransportPodsArrivalAction_AttackSettlement.CanAttack(pods, settlement),
                     () => new TransportPodsArrivalAction_AttackSettlement(settlement,
                         PawnsArrivalModeDefOf.CenterDrop), "AttackAndDropInCenter".Translate(settlement.Label),
                     representative, settlement.Tile, car))
        {
            yield return f2;
        }
    }

    public static IEnumerable<FloatMenuOption> GetGIFTFloatMenuOptions(CompLaunchableHelicopter representative,
        IEnumerable<IThingHolder> pods, Settlement settlement, Caravan car)
    {
        if (settlement.Faction == Faction.OfPlayer)
        {
            return [];
        }

        return GetFloatMenuOptions(() => TransportPodsArrivalAction_GiveGift.CanGiveGiftTo(pods, settlement),
            () => new TransportPodsArrivalAction_GiveGift(settlement), "GiveGiftViaTransportPods".Translate(
                settlement.Faction.Name,
                FactionGiftUtility.GetGoodwillChange(pods, settlement).ToStringWithSign()), representative,
            settlement.Tile, car);
    }

    public static IEnumerable<FloatMenuOption> GetVisitFloatMenuOptions(CompLaunchableHelicopter representative,
        IEnumerable<IThingHolder> pods, Settlement settlement, Caravan car)
    {
        return GetFloatMenuOptions(() => TransportPodsArrivalAction_VisitSettlement.CanVisit(pods, settlement),
            () => new TransportPodsArrivalAction_VisitSettlement(settlement),
            "VisitSettlement".Translate(settlement.Label), representative, settlement.Tile, car);
    }
}