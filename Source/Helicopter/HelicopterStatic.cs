using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Helicopter;

public static class HelicopterStatic
{
    public static IEnumerable<FloatMenuOption> getFM(WorldObject wobj, IEnumerable<IThingHolder> ih,
        CompLaunchableHelicopter comp, Caravan car)
    {
        if (wobj is Caravan)
        {
            return [];
        }

        if (wobj is Site site)
        {
            return GetSite(site, ih, comp, car);
        }

        if (wobj is Settlement settlement)
        {
            return GetSettle(settlement, ih, comp, car);
        }

        if (wobj is MapParent parent)
        {
            return GetMapParent(parent, ih, comp, car);
        }

        return [];
    }


    public static IEnumerable<FloatMenuOption> GetMapParent(MapParent mapparent, IEnumerable<IThingHolder> pods,
        CompLaunchableHelicopter representative, Caravan car)
    {
        /*
        foreach (FloatMenuOption o in mapparent.GetFloatMenuOptions())
        {
            yield return o;
        }
        */

        if (TransportersArrivalAction_LandInSpecificCell.CanLandInSpecificCell(pods, mapparent))
        {
            yield return new FloatMenuOption("LandInExistingMap".Translate(mapparent.Label), delegate
            {
                var myMap = car == null ? representative.parent.Map : null;

                var map = mapparent.Map;
                Current.Game.CurrentMap = map;
                CameraJumper.TryHideWorld();
                Find.Targeter.BeginTargeting(TargetingParameters.ForDropPodsDestination(),
                    delegate(LocalTargetInfo x)
                    {
                        representative.TryLaunch(mapparent.Tile,
                            new TransportersArrivalAction_LandInSpecificCell(mapparent, x.Cell), car);
                    }, null, delegate
                    {
                        if (myMap != null && Find.Maps.Contains(myMap))
                        {
                            Current.Game.CurrentMap = myMap;
                        }
                    }, CompLaunchable.TargeterMouseAttachment);
            });
        }
    }

    public static IEnumerable<FloatMenuOption> GetSite(Site site, IEnumerable<IThingHolder> pods,
        CompLaunchableHelicopter representative, Caravan car)
    {
        foreach (var o in GetMapParent(site, pods, representative, car))
        {
            yield return o;
        }

        foreach (var o2 in GetVisitSite(representative, pods, site, car))
        {
            yield return o2;
        }
    }

    public static IEnumerable<FloatMenuOption> GetVisitSite(CompLaunchableHelicopter representative,
        IEnumerable<IThingHolder> pods, Site site, Caravan car)
    {
        foreach (var f in HelicoptersArrivalActionUtility.GetFloatMenuOptions(
                     () => TransportersArrivalAction_VisitSite.CanVisit(pods, site),
                     () => new TransportersArrivalAction_VisitSite(site, PawnsArrivalModeDefOf.EdgeDrop),
                     "DropAtEdge".Translate(), representative, site.Tile, car))
        {
            yield return f;
        }

        foreach (var f2 in HelicoptersArrivalActionUtility.GetFloatMenuOptions(
                     () => TransportersArrivalAction_VisitSite.CanVisit(pods, site),
                     () => new TransportersArrivalAction_VisitSite(site, PawnsArrivalModeDefOf.CenterDrop),
                     "DropInCenter".Translate(), representative, site.Tile, car))
        {
            yield return f2;
        }
    }

    public static IEnumerable<FloatMenuOption> GetSettle(Settlement bs, IEnumerable<IThingHolder> pods,
        CompLaunchableHelicopter representative, Caravan car)
    {
        foreach (var o in GetMapParent(bs, pods, representative, car))
        {
            yield return o;
        }

        foreach (var f in HelicoptersArrivalActionUtility.GetVisitFloatMenuOptions(representative, pods, bs, car))
        {
            yield return f;
        }

        foreach (var f2 in HelicoptersArrivalActionUtility.GetGIFTFloatMenuOptions(representative, pods, bs, car))
        {
            yield return f2;
        }

        foreach (var f3 in HelicoptersArrivalActionUtility.GetATKFloatMenuOptions(representative, pods, bs, car))
        {
            yield return f3;
        }
    }

    public static void HelicopterDestroy(Thing thing, DestroyMode mode = DestroyMode.Vanish)
    {
        if (!Thing.allowDestroyNonDestroyable && !thing.def.destroyable)
        {
            Log.Error($"Tried to destroy non-destroyable thing {thing}");
            return;
        }

        if (thing.Destroyed)
        {
            Log.Error($"Tried to destroy already-destroyed thing {thing}");
            return;
        }

        if (thing.Spawned)
        {
            thing.DeSpawn(mode);
        }

        var typ = typeof(Thing);
        var finfo = typ.GetField("mapIndexOrState", BindingFlags.NonPublic | BindingFlags.Instance);
        sbyte sbt = -2;
        finfo?.SetValue(thing, sbt);


        if (thing.def.DiscardOnDestroyed)
        {
            thing.Discard();
        }

        thing.holdingOwner?.Notify_ContainedItemDestroyed(thing);

        var minfo = typ.GetMethod("RemoveAllReservationsAndDesignationsOnThis",
            BindingFlags.NonPublic | BindingFlags.Instance);
        minfo?.Invoke(thing, null);

        if (thing is not Pawn)
        {
            thing.stackCount = 0;
        }
    }
}