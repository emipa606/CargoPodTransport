using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Helicopter;

public class HelicopterLeaving : Skyfaller, IActiveDropPod
{
    private static readonly List<Thing> tmpActiveDropPods = [];

    private bool alreadyLeft;

    public TransportPodsArrivalAction arrivalAction;

    public int destinationTile = -1;

    public int groupID = -1;

    public ActiveDropPodInfo Contents
    {
        get => ((ActiveDropPod)innerContainer[0]).Contents;
        set => ((ActiveDropPod)innerContainer[0]).Contents = value;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref groupID, "groupID");
        Scribe_Values.Look(ref destinationTile, "destinationTile");
        Scribe_Deep.Look(ref arrivalAction, "arrivalAction");
        Scribe_Values.Look(ref alreadyLeft, "alreadyLeft");
    }

    protected override void LeaveMap()
    {
        if (alreadyLeft)
        {
            base.LeaveMap();
            return;
        }

        if (groupID < 0)
        {
            Log.Error($"Drop pod left the map, but its group ID is {groupID}");
            Destroy();
            return;
        }

        if (destinationTile < 0)
        {
            Log.Error($"Drop pod left the map, but its destination tile is {destinationTile}");
            Destroy();
            return;
        }

        var lord = TransporterUtility.FindLord(groupID, Map);
        if (lord != null)
        {
            Map.lordManager.RemoveLord(lord);
        }

        var travelingTransportPods =
            (TravelingTransportPods)WorldObjectMaker.MakeWorldObject(
                DefDatabase<WorldObjectDef>.GetNamed("TravelingHelicopters"));
        travelingTransportPods.Tile = Map.Tile;
        travelingTransportPods.SetFaction(Faction.OfPlayer);
        travelingTransportPods.destinationTile = destinationTile;
        travelingTransportPods.arrivalAction = arrivalAction;
        Find.WorldObjects.Add(travelingTransportPods);
        tmpActiveDropPods.Clear();
        tmpActiveDropPods.AddRange(Map.listerThings.ThingsInGroup(ThingRequestGroup.ActiveDropPod));
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < tmpActiveDropPods.Count; i++)
        {
            if (tmpActiveDropPods[i] is not HelicopterLeaving HelicopterLeaving || HelicopterLeaving.groupID != groupID)
            {
                continue;
            }

            HelicopterLeaving.alreadyLeft = true;
            travelingTransportPods.AddPod(HelicopterLeaving.Contents, true);
            HelicopterLeaving.Contents = null;
            HelicopterLeaving.Destroy();
        }
    }
}