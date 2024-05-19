using HarmonyLib;
using RimWorld;
using Verse;

namespace Helicopter;

//helicopter direct place
[HarmonyPatch(typeof(ActiveDropPod), "PodOpen")]
public static class ActiveDropPod_PodOpen
{
    public static void Prefix(ActiveDropPod __instance)
    {
        var tv = Traverse.Create(__instance);
        var contents = tv.Field("contents").GetValue<ActiveDropPodInfo>();
        for (var i = contents.innerContainer.Count - 1; i >= 0; i--)
        {
            var thing = contents.innerContainer[i];
            if (thing == null || thing.def.defName != "Building_Helicopter")
            {
                continue;
            }

            GenPlace.TryPlaceThing(thing, __instance.Position, __instance.Map, ThingPlaceMode.Direct, out _,
                delegate(Thing placedThing, int _)
                {
                    if (Find.TickManager.TicksGame < 1200 && TutorSystem.TutorialMode &&
                        placedThing.def.category == ThingCategory.Item)
                    {
                        Find.TutorialState.AddStartingItem(placedThing);
                    }
                });
            break;
        }
    }
}