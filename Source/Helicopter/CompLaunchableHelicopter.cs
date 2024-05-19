using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Helicopter;

[StaticConstructorOnStartup]
public class CompLaunchableHelicopter : ThingComp
{
    private const float FuelPerTile = 2.25f;

    public static readonly Texture2D TargeterMouseAttachment =
        ContentFinder<Texture2D>.Get("UI/Overlays/LaunchableMouseAttachment");

    private static readonly Texture2D LaunchCommandTex = ContentFinder<Texture2D>.Get("UI/Commands/LaunchShip");

    private CompTransporter cachedCompTransporter;

    private Caravan carr;

    public Building FuelingPortSource => (Building)parent;

    public bool ConnectedToFuelingPort => FuelingPortSource != null;

    public bool FuelingPortSourceHasAnyFuel =>
        ConnectedToFuelingPort && FuelingPortSource.GetComp<CompRefuelable>().HasFuel;

    public bool LoadingInProgressOrReadyToLaunch => Transporter.LoadingInProgressOrReadyToLaunch;

    public bool AnythingLeftToLoad => Transporter.AnythingLeftToLoad;

    public Thing FirstThingLeftToLoad => Transporter.FirstThingLeftToLoad;

    public List<CompTransporter> TransportersInGroup => [parent.TryGetComp<CompTransporter>()];

    public bool AnyInGroupHasAnythingLeftToLoad => Transporter.AnyInGroupHasAnythingLeftToLoad;

    public Thing FirstThingLeftToLoadInGroup => Transporter.FirstThingLeftToLoadInGroup;

    public bool AnyInGroupIsUnderRoof
    {
        get
        {
            var transportersInGroup = TransportersInGroup;
            foreach (var compTransporter in transportersInGroup)
            {
                if (compTransporter.parent.Position.Roofed(parent.Map))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public CompTransporter Transporter
    {
        get
        {
            if (cachedCompTransporter == null)
            {
                cachedCompTransporter = parent.GetComp<CompTransporter>();
            }

            return cachedCompTransporter;
        }
    }

    public float FuelingPortSourceFuel => !ConnectedToFuelingPort ? 0f : parent.GetComp<CompRefuelable>().Fuel;

    public bool AllInGroupConnectedToFuelingPort => true;

    public bool AllFuelingPortSourcesInGroupHaveAnyFuel => true;

    private float FuelInLeastFueledFuelingPortSource => FuelingPortSourceFuel;

    private int MaxLaunchDistance
    {
        get
        {
            if (!parent.Spawned)
            {
                return MaxLaunchDistanceAtFuelLevel(FuelInLeastFueledFuelingPortSource);
            }

            return !LoadingInProgressOrReadyToLaunch
                ? 0
                : MaxLaunchDistanceAtFuelLevel(FuelInLeastFueledFuelingPortSource);
        }
    }

    private int MaxLaunchDistanceEverPossible
    {
        get
        {
            if (!LoadingInProgressOrReadyToLaunch)
            {
                return 0;
            }

            var num = 0f;
            var fuelingPortSource = FuelingPortSource;
            if (fuelingPortSource != null)
            {
                num = Mathf.Max(num, fuelingPortSource.GetComp<CompRefuelable>().Props.fuelCapacity);
            }

            return MaxLaunchDistanceAtFuelLevel(num);
        }
    }

    private bool PodsHaveAnyPotentialCaravanOwner
    {
        get
        {
            var transportersInGroup = TransportersInGroup;
            foreach (var compTransporter in transportersInGroup)
            {
                var innerContainer = compTransporter.innerContainer;
                foreach (var thing in innerContainer)
                {
                    if (thing is Pawn pawn && CaravanUtility.IsOwner(pawn, Faction.OfPlayer))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var g in base.CompGetGizmosExtra())
        {
            yield return g;
        }

        if (!LoadingInProgressOrReadyToLaunch)
        {
            yield break;
        }

        var launch = new Command_Action
        {
            defaultLabel = "CommandLaunchGroup".Translate(),
            defaultDesc = "CommandLaunchGroupDesc".Translate(),
            icon = LaunchCommandTex,
            alsoClickIfOtherInGroupClicked = false,
            action = delegate
            {
                if (AnyInGroupHasAnythingLeftToLoad)
                {
                    Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                        "ConfirmSendNotCompletelyLoadedPods".Translate(FirstThingLeftToLoadInGroup.LabelCapNoCount),
                        StartChoosingDestination));
                }
                else
                {
                    StartChoosingDestination();
                }
            }
        };
        if (!AllInGroupConnectedToFuelingPort)
        {
            launch.Disable("CommandLaunchGroupFailNotConnectedToFuelingPort".Translate());
        }
        else if (!AllFuelingPortSourcesInGroupHaveAnyFuel)
        {
            launch.Disable("CommandLaunchGroupFailNoFuel".Translate());
        }
        else if (AnyInGroupIsUnderRoof)
        {
            launch.Disable("CommandLaunchGroupFailUnderRoof".Translate());
        }

        yield return launch;
    }

    public override string CompInspectStringExtra()
    {
        if (!LoadingInProgressOrReadyToLaunch)
        {
            return null;
        }

        if (!AllInGroupConnectedToFuelingPort)
        {
            return "NotReadyForLaunch".Translate() + ": " + "NotAllInGroupConnectedToFuelingPort".Translate() + ".";
        }

        if (!AllFuelingPortSourcesInGroupHaveAnyFuel)
        {
            return "NotReadyForLaunch".Translate() + ": " + "NotAllFuelingPortSourcesInGroupHaveAnyFuel".Translate() +
                   ".";
        }

        if (AnyInGroupHasAnythingLeftToLoad)
        {
            return "NotReadyForLaunch".Translate() + ": " + "TransportPodInGroupHasSomethingLeftToLoad".Translate() +
                   ".";
        }

        return "ReadyForLaunch".Translate();
    }

    private void StartChoosingDestination()
    {
        CameraJumper.TryJump(CameraJumper.GetWorldTarget(parent));
        Find.WorldSelector.ClearSelection();
        var tile = parent.Map.Tile;
        carr = null;
        Find.WorldTargeter.BeginTargeting(ChoseWorldTarget, true, TargeterMouseAttachment, true,
            delegate { GenDraw.DrawWorldRadiusRing(tile, MaxLaunchDistance); }, delegate(GlobalTargetInfo target)
            {
                if (!target.IsValid)
                {
                    return null;
                }

                var num = Find.WorldGrid.TraversalDistanceBetween(tile, target.Tile);
                if (num > MaxLaunchDistance)
                {
                    GUI.color = Color.red;
                    return num > MaxLaunchDistanceEverPossible
                        ? "TransportPodDestinationBeyondMaximumRange".Translate()
                        : "TransportPodNotEnoughFuel".Translate();
                }

                var transportPodsFloatMenuOptionsAt = GetTransportPodsFloatMenuOptionsAt(target.Tile);
                if (!transportPodsFloatMenuOptionsAt.Any())
                {
                    if (Find.WorldGrid[target.Tile].biome.impassable || Find.World.Impassable(target.Tile))
                    {
                        return "MessageTransportPodsDestinationIsInvalid".Translate();
                    }

                    return string.Empty;
                }

                if (transportPodsFloatMenuOptionsAt.Count() == 1)
                {
                    if (transportPodsFloatMenuOptionsAt.First().Disabled)
                    {
                        GUI.color = Color.red;
                    }

                    return transportPodsFloatMenuOptionsAt.First().Label;
                }

                if (target.WorldObject is MapParent mapParent)
                {
                    return "ClickToSeeAvailableOrders_WorldObject".Translate(mapParent.LabelCap);
                }

                return "ClickToSeeAvailableOrders_Empty".Translate();
            });
    }


    public void WorldStartChoosingDestination(Caravan car)
    {
        CameraJumper.TryJump(CameraJumper.GetWorldTarget(car));
        Find.WorldSelector.ClearSelection();
        var tile = car.Tile;
        carr = car;
        Find.WorldTargeter.BeginTargeting(ChoseWorldTarget, true, TargeterMouseAttachment, false,
            delegate { GenDraw.DrawWorldRadiusRing(car.Tile, MaxLaunchDistance); }, delegate(GlobalTargetInfo target)
            {
                if (!target.IsValid)
                {
                    return null;
                }

                var num = Find.WorldGrid.TraversalDistanceBetween(tile, target.Tile);
                if (num > MaxLaunchDistance)
                {
                    GUI.color = Color.red;
                    return num > MaxLaunchDistanceEverPossible
                        ? "TransportPodDestinationBeyondMaximumRange".Translate()
                        : "TransportPodNotEnoughFuel".Translate();
                }

                var transportPodsFloatMenuOptionsAt = GetTransportPodsFloatMenuOptionsAt(target.Tile, car);
                if (!transportPodsFloatMenuOptionsAt.Any())
                {
                    if (Find.WorldGrid[target.Tile].biome.impassable || Find.World.Impassable(target.Tile))
                    {
                        return "MessageTransportPodsDestinationIsInvalid".Translate();
                    }

                    return string.Empty;
                }

                if (transportPodsFloatMenuOptionsAt.Count() == 1)
                {
                    if (transportPodsFloatMenuOptionsAt.First().Disabled)
                    {
                        GUI.color = Color.red;
                    }

                    return transportPodsFloatMenuOptionsAt.First().Label;
                }

                if (target.WorldObject is MapParent mapParent)
                {
                    return "ClickToSeeAvailableOrders_WorldObject".Translate(mapParent.LabelCap);
                }

                return "ClickToSeeAvailableOrders_Empty".Translate();
            });
    }

    private bool ChoseWorldTarget(GlobalTargetInfo target)
    {
        if (carr == null)
        {
            if (!LoadingInProgressOrReadyToLaunch)
            {
                return true;
            }
        }

        if (!target.IsValid)
        {
            Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput,
                false);
            return false;
        }

        var myTile = carr?.Tile ?? parent.Map.Tile;

        var num = Find.WorldGrid.TraversalDistanceBetween(myTile, target.Tile);
        if (num > MaxLaunchDistance)
        {
            Messages.Message(
                "MessageTransportPodsDestinationIsTooFar".Translate(FuelNeededToLaunchAtDist(num).ToString("0.#")),
                MessageTypeDefOf.RejectInput, false);
            return false;
        }

        if (Find.WorldGrid[target.Tile].biome.impassable || Find.World.Impassable(target.Tile))
        {
            Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput,
                false);
            return false;
        }

        Find.WorldObjects.MapParentAt(target.Tile);


        var transportPodsFloatMenuOptionsAt = GetTransportPodsFloatMenuOptionsAt(target.Tile, carr);


        if (!transportPodsFloatMenuOptionsAt.Any())
        {
            if (Find.WorldGrid[target.Tile].biome.impassable || Find.World.Impassable(target.Tile))
            {
                Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput,
                    false);
                return false;
            }

            TryLaunch(target.Tile, null);
            return true;
        }

        if (transportPodsFloatMenuOptionsAt.Count() == 1)
        {
            if (!transportPodsFloatMenuOptionsAt.First().Disabled)
            {
                transportPodsFloatMenuOptionsAt.First().action();
            }

            return false;
        }

        Find.WindowStack.Add(new FloatMenu(transportPodsFloatMenuOptionsAt.ToList()));
        return false;
    }


    public void TryLaunch(int destinationTile, TransportPodsArrivalAction arrivalAction, Caravan cafr = null)
    {
        if (cafr == null)
        {
            if (!parent.Spawned)
            {
                Log.Error($"Tried to launch {parent}, but it's unspawned.");
                return;
            }
        }

        if (parent.Spawned)
        {
            if (!LoadingInProgressOrReadyToLaunch)
            {
                return;
            }
        }

        if (!AllInGroupConnectedToFuelingPort || !AllFuelingPortSourcesInGroupHaveAnyFuel)
        {
            return;
        }

        if (cafr == null)
        {
            var map = parent.Map;
            var num = Find.WorldGrid.TraversalDistanceBetween(map.Tile, destinationTile);
            if (num > MaxLaunchDistance)
            {
                return;
            }

            Transporter.TryRemoveLord(map);
            var groupID = Transporter.groupID;
            var amount = Mathf.Max(FuelNeededToLaunchAtDist(num), 1f);

            var compTransporter = FuelingPortSource.TryGetComp<CompTransporter>();
            var fuelingPortSource = FuelingPortSource;
            fuelingPortSource?.TryGetComp<CompRefuelable>().ConsumeFuel(amount);

            var directlyHeldThings = compTransporter.GetDirectlyHeldThings();

            var helicopter = ThingMaker.MakeThing(ThingDef.Named("Building_Helicopter"));
            helicopter.SetFactionDirect(Faction.OfPlayer);

            var compr = helicopter.TryGetComp<CompRefuelable>();
            var tcr = compr.GetType();
            var finfos = tcr.GetField("fuel", BindingFlags.NonPublic | BindingFlags.Instance);
            finfos?.SetValue(compr, fuelingPortSource.TryGetComp<CompRefuelable>().Fuel);

            helicopter.stackCount = 1;
            directlyHeldThings.TryAddOrTransfer(helicopter);

            var activeDropPod = (ActiveDropPod)ThingMaker.MakeThing(ThingDef.Named("ActiveHelicopter"));
            activeDropPod.Contents = new ActiveDropPodInfo();
            activeDropPod.Contents.innerContainer.TryAddRangeOrTransfer(directlyHeldThings, true, true);
            var dropPodLeaving =
                (HelicopterLeaving)SkyfallerMaker.MakeSkyfaller(ThingDef.Named("HelicopterLeaving"), activeDropPod);
            dropPodLeaving.groupID = groupID;
            dropPodLeaving.destinationTile = destinationTile;
            dropPodLeaving.arrivalAction = arrivalAction;
            compTransporter.CleanUpLoadingVars(map);
            if (fuelingPortSource != null)
            {
                var poc = fuelingPortSource.Position;
                HelicopterStatic.HelicopterDestroy(fuelingPortSource);
                GenSpawn.Spawn(dropPodLeaving, poc, map);
            }

            CameraJumper.TryHideWorld();
        }
        else
        {
            var num = Find.WorldGrid.TraversalDistanceBetween(carr.Tile, destinationTile);
            if (num > MaxLaunchDistance)
            {
                return;
            }

            var amount = Mathf.Max(FuelNeededToLaunchAtDist(num), 1f);
            FuelingPortSource?.TryGetComp<CompRefuelable>().ConsumeFuel(amount);


            var directlyHeldThings = (ThingOwner<Pawn>)cafr.GetDirectlyHeldThings();
            Thing helicopter = null;
            foreach (var pawn in directlyHeldThings.InnerListForReading)
            {
                var pinv = pawn.inventory;
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < pinv.innerContainer.Count; i++)
                {
                    if (pinv.innerContainer[i].def.defName != "Building_Helicopter")
                    {
                        continue;
                    }

                    helicopter = pinv.innerContainer[i];
                    pinv.innerContainer[i].holdingOwner.Remove(pinv.innerContainer[i]);

                    break;
                }
            }

            var finalto = new ThingOwner<Thing>();
            var lpto = directlyHeldThings.AsEnumerable<Pawn>().ToList();
            foreach (var p in lpto)
            {
                finalto.TryAddOrTransfer(p);
            }


            if (helicopter is { holdingOwner: null })
            {
                finalto.TryAddOrTransfer(helicopter, false);
            }


            var activeDropPod = (ActiveDropPod)ThingMaker.MakeThing(ThingDef.Named("ActiveHelicopter"));
            activeDropPod.Contents = new ActiveDropPodInfo();
            activeDropPod.Contents.innerContainer.TryAddRangeOrTransfer(finalto, true, true);

            cafr.RemoveAllPawns();
            if (cafr.Spawned)
            {
                Find.WorldObjects.Remove(cafr);
            }

            var travelingTransportPods =
                (TravelingTransportPods)WorldObjectMaker.MakeWorldObject(
                    DefDatabase<WorldObjectDef>.GetNamed("TravelingHelicopters"));
            travelingTransportPods.Tile = cafr.Tile;
            travelingTransportPods.SetFaction(Faction.OfPlayer);
            travelingTransportPods.destinationTile = destinationTile;
            travelingTransportPods.arrivalAction = arrivalAction;
            Find.WorldObjects.Add(travelingTransportPods);
            travelingTransportPods.AddPod(activeDropPod.Contents, true);
            activeDropPod.Contents = null;
            activeDropPod.Destroy();
            Find.WorldTargeter.StopTargeting();
        }
    }

    public void Notify_FuelingPortSourceDeSpawned()
    {
        if (Transporter.CancelLoad())
        {
            Messages.Message("MessageTransportersLoadCanceled_FuelingPortGiverDeSpawned".Translate(), parent,
                MessageTypeDefOf.NegativeEvent);
        }
    }

    public static int MaxLaunchDistanceAtFuelLevel(float fuelLevel)
    {
        return Mathf.FloorToInt(fuelLevel / FuelPerTile);
    }

    public static float FuelNeededToLaunchAtDist(float dist)
    {
        return FuelPerTile * dist;
    }

    public IEnumerable<FloatMenuOption> GetTransportPodsFloatMenuOptionsAt(int tile, Caravan car = null)
    {
        var anything = false;
        IEnumerable<IThingHolder> pods = TransportersInGroup;
        if (car != null)
        {
            var rliss = new List<Caravan> { car };
            pods = rliss;
        }

        if (car == null)
        {
            if (TransportPodsArrivalAction_FormCaravan.CanFormCaravanAt(pods, tile) &&
                !Find.WorldObjects.AnySettlementBaseAt(tile) && !Find.WorldObjects.AnySiteAt(tile))
            {
                anything = true;
                yield return new FloatMenuOption("FormCaravanHere".Translate(),
                    delegate { TryLaunch(tile, new TransportPodsArrivalAction_FormCaravan()); });
            }
        }
        else
        {
            if (!Find.WorldObjects.AnySettlementBaseAt(tile) && !Find.WorldObjects.AnySiteAt(tile) &&
                !Find.World.Impassable(tile))
            {
                anything = true;
                yield return new FloatMenuOption("FormCaravanHere".Translate(),
                    delegate { TryLaunch(tile, new TransportPodsArrivalAction_FormCaravan(), car); });
            }
        }

        var worldObjects = Find.WorldObjects.AllWorldObjects;
        foreach (var worldObject in worldObjects)
        {
            if (worldObject.Tile != tile)
            {
                continue;
            }

            var nowre = HelicopterStatic.getFM(worldObject, pods, this, car);
            if (nowre.ToList().Count < 1)
            {
                yield return new FloatMenuOption("FormCaravanHere".Translate(),
                    delegate { TryLaunch(tile, new TransportPodsArrivalAction_FormCaravan(), car); });
            }
            else
            {
                foreach (var o in nowre)
                {
                    anything = true;
                    yield return o;
                }
            }
        }


        if (!anything && !Find.World.Impassable(tile))
        {
            yield return new FloatMenuOption("TransportPodsContentsWillBeLost".Translate(),
                delegate { TryLaunch(tile, null); });
        }
    }
}