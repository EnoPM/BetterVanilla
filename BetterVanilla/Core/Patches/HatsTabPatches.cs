using BetterVanilla.Core.Extensions;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(HatsTab))]
public static class HatsTabPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(HatsTab.SelectHat))]
    [HarmonyPatch(nameof(HatsTab.ClickEquip))]
    private static void SelectHatPostfix(VisorsTab __instance)
    {
        __instance.PlayerPreview.SetLocalVisorColor();
    }
}