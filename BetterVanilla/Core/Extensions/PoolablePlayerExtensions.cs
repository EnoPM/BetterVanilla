using BetterVanilla.Cosmetics;
using BetterVanilla.Options;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class PoolablePlayerExtensions
{
    extension(PoolablePlayer player)
    {
        public void SetVisorColor(Color color)
        {
            if (player == null || player.cosmetics == null || !player.cosmetics.initialized) return;
            player.cosmetics.currentBodySprite.BodySprite.SetVisorColor(color);
        
            var hat = player.cosmetics.hat;
            if (CosmeticsManager.Instance.Hats.TryGetViewData(hat.Hat.ProductId, out var viewData) && viewData.MatchPlayerColor)
            {
                hat.FrontLayer.SetVisorColor(color);
            }
        }

        public void SetLocalVisorColor()
        {
            if (!LocalConditions.AmSponsor()) return;
            player.SetVisorColor(SponsorOptions.Default.VisorColor.Value);
        }
    }
}