using HarmonyLib;

namespace BetterVanilla.Cosmetics.Core.Patches;

[HarmonyPatch(typeof(CosmeticData))]
internal static class CosmeticDataPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(CosmeticData.GetItemName))]
    private static void GetItemNamePostfix(CosmeticData __instance, ref string __result)
    {
        var productId = __instance.ProductId;
        
        if (productId.StartsWith("bv_hat_"))
        {
            if (!CosmeticsManager.Instance.Hats.TryGetCosmetic(productId, out var cosmetic)) return;
            __result = cosmetic.GetDisplayName();
            return;
        }
        
        if (productId.StartsWith("bv_visor_"))
        {
            if (!CosmeticsManager.Instance.Visors.TryGetCosmetic(productId, out var cosmetic)) return;
            __result = cosmetic.GetDisplayName();
        }
    }
}