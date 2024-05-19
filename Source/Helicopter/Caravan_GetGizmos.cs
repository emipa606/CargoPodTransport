using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Helicopter;

[HarmonyPatch(typeof(Caravan), nameof(Caravan.GetGizmos))]
public static class Caravan_GetGizmos
{
    public static void Postfix(Caravan __instance, ref IEnumerable<Gizmo> __result)
    {
        float masss = 0;
        foreach (var pawn in __instance.pawns.InnerListForReading)
        {
            foreach (var thing in pawn.inventory.innerContainer)
            {
                if (thing.def.defName != "Building_Helicopter")
                {
                    masss += thing.def.BaseMass *
                             thing.stackCount;
                }
            }
        }

        foreach (var pawn in __instance.pawns.InnerListForReading)
        {
            var pinv = pawn.inventory;
            for (var i = 0; i < pinv.innerContainer.Count; i++)
            {
                if (pinv.innerContainer[i].def.defName != "Building_Helicopter")
                {
                    continue;
                }

                var launch = new Command_Action
                {
                    defaultLabel = "CommandLaunchGroup".Translate(),
                    defaultDesc = "CommandLaunchGroupDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/LaunchShip"),
                    alsoClickIfOtherInGroupClicked = false,
                    action = delegate
                    {
                        var maxmass = pinv.innerContainer[i].TryGetComp<CompTransporter>().Props.massCapacity;
                        if (masss <= maxmass)
                        {
                            pinv.innerContainer[i].TryGetComp<CompLaunchableHelicopter>()
                                .WorldStartChoosingDestination(__instance);
                        }
                        else
                        {
                            Messages.Message(
                                $"{"TooBigTransportersMassUsage".Translate() + "("}{maxmass - masss}KG)",
                                MessageTypeDefOf.RejectInput, false);
                        }
                    }
                };

                var newr = __result.ToList();
                newr.Add(launch);

                var addFuel = new Command_Action
                {
                    defaultLabel = "CommandAddFuel".Translate(),
                    defaultDesc = "CommandAddFuelDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("Things/Item/Resource/Chemfuel"),
                    alsoClickIfOtherInGroupClicked = false,
                    action = delegate
                    {
                        var hasAddFuel = false;
                        var fcont = 0;
                        var comprf = pinv.innerContainer[i].TryGetComp<CompRefuelable>();
                        var list = CaravanInventoryUtility.AllInventoryItems(__instance);
                        //pinv.innerContainer.Count
                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var j = 0; j < list.Count; j++)
                        {
                            if (list[j].def != ThingDefOf.Chemfuel)
                            {
                                continue;
                            }

                            fcont = list[j].stackCount;
                            var ownerOf = CaravanInventoryUtility.GetOwnerOf(__instance, list[j]);
                            var need = comprf.Props.fuelCapacity - comprf.Fuel;

                            if (need is < 1f and > 0)
                            {
                                fcont = 1;
                            }

                            if (fcont * 1f >= need)
                            {
                                fcont = (int)need;
                            }


                            // Log.Warning("f&n is "+fcont+"/"+need);
                            if (list[j].stackCount * 1f <= fcont)
                            {
                                list[j].stackCount -= fcont;
                                var thing = list[j];
                                ownerOf.inventory.innerContainer.Remove(thing);
                                thing.Destroy();
                            }
                            else
                            {
                                if (fcont != 0)
                                {
                                    list[j].SplitOff(fcont).Destroy();
                                }
                            }


                            var crtype = comprf.GetType();
                            var finfo = crtype.GetField("fuel", BindingFlags.NonPublic | BindingFlags.Instance);
                            finfo?.SetValue(comprf, comprf.Fuel + fcont);
                            hasAddFuel = true;
                            break;
                        }

                        if (hasAddFuel)
                        {
                            Messages.Message("AddFuelDoneMsg".Translate(fcont, comprf.Fuel),
                                MessageTypeDefOf.PositiveEvent, false);
                        }
                        else
                        {
                            Messages.Message("NonOilMsg".Translate(), MessageTypeDefOf.RejectInput, false);
                        }
                    }
                };

                newr.Add(addFuel);

                var fuelStat = new Gizmo_MapRefuelableFuelStatus
                {
                    nowFuel = pinv.innerContainer[i].TryGetComp<CompRefuelable>().Fuel,
                    maxFuel = pinv.innerContainer[i].TryGetComp<CompRefuelable>().Props.fuelCapacity,
                    compLabel = pinv.innerContainer[i].TryGetComp<CompRefuelable>().Props.FuelGizmoLabel
                };


                newr.Add(fuelStat);

                __result = newr;
                return;
            }
        }
    }
}