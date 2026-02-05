using System.Diagnostics.CodeAnalysis;
using BetterVanilla.Cosmetics;
using BetterVanilla.Cosmetics.Hats;
using BetterVanilla.Cosmetics.Visors;

namespace BetterVanilla.Core.Extensions;

public static class CosmeticDataExtensions
{
    extension(HatData hat)
    {
        public bool IsCustomCosmetic => CosmeticsManager.Hats.IsCustomCosmetic(hat.ProductId);

        public bool TryGetCustomHat([MaybeNullWhen(false)] out HatViewData viewData, [MaybeNullWhen(false)] out HatCosmetic cosmetic)
        {
            if (hat.IsCustomCosmetic && CosmeticsManager.Hats.TryGetViewData(hat.ProductId, out viewData) && CosmeticsManager.Hats.TryGetCosmetic(hat.ProductId, out cosmetic))
            {
                return true;
            }
            viewData = null;
            cosmetic = null;
            return false;
        }
    }

    extension(VisorData visor)
    {
        public bool IsCustomCosmetic => CosmeticsManager.Visors.IsCustomCosmetic(visor.ProductId);

        public bool TryGetCustomVisor([MaybeNullWhen(false)] out VisorViewData viewData, [MaybeNullWhen(false)] out VisorCosmetic cosmetic)
        {
            if (visor.IsCustomCosmetic && CosmeticsManager.Visors.TryGetViewData(visor.ProductId, out viewData) && CosmeticsManager.Visors.TryGetCosmetic(visor.ProductId, out cosmetic))
            {
                return true;
            }
            viewData = null;
            cosmetic = null;
            return false;
        }
    }
}