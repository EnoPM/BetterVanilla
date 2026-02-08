using System.Diagnostics.CodeAnalysis;
using BetterVanilla.Cosmetics.Hats;
using BetterVanilla.Cosmetics.Visors;

namespace BetterVanilla.Cosmetics.Core.Extensions;

public static class CosmeticDataExtensions
{
    extension(HatData hat)
    {
        public bool IsCustomCosmetic => CosmeticsManager.Instance.Hats.IsCustomCosmetic(hat.ProductId);

        public bool TryGetCustomHat([MaybeNullWhen(false)] out HatViewData viewData, [MaybeNullWhen(false)] out HatCosmetic cosmetic)
        {
            if (hat.IsCustomCosmetic && CosmeticsManager.Instance.Hats.TryGetViewData(hat.ProductId, out viewData) && CosmeticsManager.Instance.Hats.TryGetCosmetic(hat.ProductId, out cosmetic))
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
        public bool IsCustomCosmetic => CosmeticsManager.Instance.Visors.IsCustomCosmetic(visor.ProductId);

        public bool TryGetCustomVisor([MaybeNullWhen(false)] out VisorViewData viewData, [MaybeNullWhen(false)] out VisorCosmetic cosmetic)
        {
            if (visor.IsCustomCosmetic && CosmeticsManager.Instance.Visors.TryGetViewData(visor.ProductId, out viewData) && CosmeticsManager.Instance.Visors.TryGetCosmetic(visor.ProductId, out cosmetic))
            {
                return true;
            }
            viewData = null;
            cosmetic = null;
            return false;
        }
    }
}