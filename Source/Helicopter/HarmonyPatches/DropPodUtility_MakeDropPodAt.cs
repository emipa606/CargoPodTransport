using HarmonyLib;
using RimWorld;
using Verse;

namespace Helicopter;

[HarmonyPatch(typeof(DropPodUtility), nameof(DropPodUtility.MakeDropPodAt))]
public static class DropPodUtility_MakeDropPodAt
{
    public static bool Prefix(IntVec3 c, Map map, ActiveTransporterInfo info)
    {
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var index = 0; index < info.innerContainer.Count; index++)
        {
            if (info.innerContainer[index].TryGetComp<CompLaunchableHelicopter>() == null)
            {
                continue;
            }

            var activeDropPod = (ActiveTransporter)ThingMaker.MakeThing(ThingDef.Named("ActiveHelicopter"));
            activeDropPod.Contents = info;

            EnsureInBounds(ref c, map);
            SkyfallerMaker.SpawnSkyfaller(ThingDef.Named("HelicopterIncoming"), activeDropPod, c, map);
            return false;
        }

        return true;
    }

    private static void EnsureInBounds(ref IntVec3 c, Map map)
    {
        var y = 9;

        if (c.x < y)
        {
            c.x = y;
        }
        else if (c.x >= map.Size.x - y)
        {
            c.x = map.Size.x - y;
        }

        if (c.z < y)
        {
            c.z = y;
        }
        else if (c.z > map.Size.z - y)
        {
            c.z = map.Size.z - y;
        }
    }
}