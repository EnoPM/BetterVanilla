using BetterVanilla.Core.Extensions;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(VisorsTab))]
public static class VisorsTabPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(VisorsTab.SelectVisor))]
    [HarmonyPatch(nameof(VisorsTab.ClickEquip))]
    private static void SelectVisorPostfix(VisorsTab __instance)
    {
        __instance.PlayerPreview.SetLocalVisorColor();
    }
}