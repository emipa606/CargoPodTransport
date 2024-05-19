using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Helicopter;

[HarmonyPatch(typeof(Dialog_Trade), "SetupPlayerCaravanVariables")]
public static class Dialog_Trade_SetupPlayerCaravanVariables
{
    public static void Postfix(Dialog_Trade __instance)
    {
        var tv = Traverse.Create(__instance);
        var contents = tv.Field("playerCaravanAllPawnsAndItems").GetValue<List<Thing>>();
        var newResult = new List<Thing>();
        if (contents is not { Count: > 0 })
        {
            return;
        }

        foreach (var thing in contents)
        {
            if (thing.def.defName != "Building_Helicopter")
            {
                newResult.Add(thing);
            }
        }

        tv.Field("playerCaravanAllPawnsAndItems").SetValue(newResult);
    }
}