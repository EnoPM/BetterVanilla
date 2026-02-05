using System;
using BetterVanilla.Core;
using BetterVanilla.Core.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui;

public sealed class OutfitItem : MonoBehaviour
{
    private const float HatScale = 0.55f;
    private const float HatYOffset = 13f;
    private const float HatXOffset = 5f;
    
    private const float VisorScale = 0.55f;
    private const float VisorYOffset = 13f;
    private const float VisorXOffset = 5f;

    public Image player = null!;

    public Image hatFrontLayer = null!;
    public Canvas hatFrontLayerCanvas = null!;

    public Image hatBackLayer = null!;
    public Canvas hatBackLayerCanvas = null!;
    
    public Image visorLayer = null!;
    public Canvas visorLayerCanvas = null!;

    public void SetVisor(string visorId)
    {
        var visor = HatManager.Instance.GetVisorById(visorId);
        SetVisor(visor);
    }

    public void SetVisor(VisorData visor)
    {
        if (visor.TryGetCustomVisor(out var viewData, out var cosmetic))
        {
            SetVisor(viewData, cosmetic.BehindHats);
        }
        var addressableAsset = visor.CreateAddressableAsset();
        var behindHat = visor.BehindHats;

        addressableAsset.LoadAsync(new Action(() =>
        {
            var asset = addressableAsset.GetAsset();
            SetVisor(asset, behindHat);
        }));
    }

    public void SetVisor(VisorViewData asset, bool behindHat)
    {
        if (asset.IdleFrame == null)
        {
            visorLayer.enabled = false;
            return;
        }
        Ls.LogMessage($"Visor behind hats: {behindHat}");
        visorLayer.enabled = true;
        SetVisorSprite(ref visorLayer, ref visorLayerCanvas, asset.IdleFrame, behindHat);
    }

    public void SetHat(string hatId)
    {
        var hat = HatManager.Instance.GetHatById(hatId);
        SetHat(hat);
    }

    public void SetHat(HatData hat)
    {
        if (hat.TryGetCustomHat(out var viewData, out var cosmetic))
        {
            SetHat(viewData, !cosmetic.Behind);
            return;
        }
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
        if (asset.MainImage == null)
        {
            hatFrontLayer.enabled = false;
        }
        else
        {
            hatFrontLayer.enabled = true;
            SetHatSprite(ref hatFrontLayer, ref hatFrontLayerCanvas, asset.MainImage, inFront || asset.BackImage != null);
        }

        if (asset.BackImage == null)
        {
            hatBackLayer.enabled = false;
        }
        else
        {
            hatBackLayer.enabled = true;
            SetHatSprite(ref hatBackLayer, ref hatBackLayerCanvas, asset.BackImage, false);
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

        canvas.sortingOrder = inFront ? 12 : 8;
    }
    
    private static void SetVisorSprite(ref Image image, ref Canvas canvas, Sprite sprite, bool behindHats)
    {
        // Setup hat sprite and size
        image.sprite = sprite;
        var spriteSize = new Vector2(sprite.rect.width, sprite.rect.height);
        image.rectTransform.sizeDelta = spriteSize * VisorScale;

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
        image.rectTransform.anchoredPosition = new Vector2(VisorXOffset, VisorYOffset);

        canvas.sortingOrder = behindHats ? 11 : 13;
    }
}