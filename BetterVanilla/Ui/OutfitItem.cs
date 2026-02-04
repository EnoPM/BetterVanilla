using System;
using BetterVanilla.Cosmetics;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui;

public sealed class OutfitItem : MonoBehaviour
{
    private const float HatScale = 0.5f;
    private const float HatYOffset = 13f;
    private const float HatXOffset = 5f;
    
    public Image player = null!;
    //public Image hat = null!;
    public Image visor = null!;
    
    
    public Image hatFrontLayer = null!;
    public Canvas hatFrontLayerCanvas = null!;
        
    public Image hatBackLayer = null!;
    public Canvas hatBackLayerCanvas = null!;

    public void SetHat(string hatId)
    {
        if (CosmeticsManager.Hats.IsCustomCosmetic(hatId) && CosmeticsManager.Hats.TryGetViewData(hatId, out var viewData) && CosmeticsManager.Hats.TryGetCosmetic(hatId, out var cosmetic))
        {
            SetHat(viewData, !cosmetic.Behind);
        }
        else
        {
            var hat = HatManager.Instance.GetHatById(hatId);
            SetHat(hat);
        }
    }

    public void SetHat(HatData hat)
    {
        var addressableAsset = hat.CreateAddressableAsset();
        var inFront = hat.InFront;
        
        addressableAsset.LoadAsync(new Action(() =>
        {
            var asset = addressableAsset.GetAsset();
            SetHat(asset, inFront);
        }));
    }

    public void SetHat(HatViewData asset, bool inFront)
    {
        if (asset.MainImage != null)
        {
            hatFrontLayer.enabled = true;
            SetHatSprite(ref hatFrontLayer, ref hatFrontLayerCanvas, asset.MainImage, inFront || asset.BackImage != null);
        }
        else
        {
            hatFrontLayer.enabled = false;
        }

        if (asset.BackImage != null)
        {
            hatBackLayer.enabled = true;
            SetHatSprite(ref hatBackLayer, ref hatBackLayerCanvas, asset.BackImage, false);
        }
        else
        {
            hatBackLayer.enabled = false;
        }
    }

    private static void SetHatSprite(ref Image image, ref Canvas canvas, Sprite sprite, bool inFront)
    {
        // Setup hat sprite and size
        image.sprite = sprite;
        var spriteSize = new Vector2(sprite.rect.width, sprite.rect.height);
        image.rectTransform.sizeDelta = spriteSize * HatScale;
        
        // Sync RectTransform pivot with sprite pivot
        var normalizedPivot = new Vector2(
            sprite.pivot.x / sprite.rect.width,
            sprite.pivot.y / sprite.rect.height
        );
        image.rectTransform.pivot = normalizedPivot;
        
        // Force anchors to center
        image.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        image.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        
        // Use manual offsets
        image.rectTransform.anchoredPosition = new Vector2(HatXOffset, HatYOffset);

        canvas.sortingOrder = inFront ? 11 : 9;
    }
}