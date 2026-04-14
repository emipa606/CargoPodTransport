using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Helicopter;

[HarmonyPatch(typeof(Dialog_Trade), "SetupPlayerCaravanVariables")]
public static class Dialog_Trade_SetupPlayerCaravanVariables
{
    public static void Postfix(ref List<Thing> ___playerCaravanAllPawnsAndItems)
    {
        var newResult = new List<Thing>();
        if (___playerCaravanAllPawnsAndItems is not { Count: > 0 })
        {
            return;
        }

        foreach (var thing in ___playerCaravanAllPawnsAndItems)
        {
            if (thing.def.defName != "Building_Helicopter")
            {
                newResult.Add(thing);
            }
        }

        ___playerCaravanAllPawnsAndItems = newResult;
    }
}