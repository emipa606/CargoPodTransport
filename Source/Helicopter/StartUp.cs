using System.Reflection;
using HarmonyLib;
using Verse;

namespace Helicopter;

[StaticConstructorOnStartup]
public static class StartUp
{
    static StartUp()
    {
        new Harmony("Jellypowered.TransportCargoPod").PatchAll(Assembly.GetExecutingAssembly());
    }
}