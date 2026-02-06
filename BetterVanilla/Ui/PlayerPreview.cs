using System;
using BetterVanilla.Core.Extensions;
using Innersloth.Assets;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui;

public sealed class PlayerPreview : MonoBehaviour
{
    private const float HatScale = 0.5f;
    private const float HatYOffset = 13f;
    private const float HatXOffset = 5f;

    private const float VisorScale = 0.55f;
    private const float VisorYOffset = 13f;
    private const float VisorXOffset = 5f;

    private const float SkinScale = 0.57f;
    private const float SkinYOffset = -18f;
    private const float SkinXOffset = 6.5f;
    
    public Image playerImage = null!;

    public Image hatFrontLayer = null!;
    public Canvas hatFrontLayerCanvas = null!;
    
    public Image hatBackLayer = null!;
    public Canvas hatBackLayerCanvas = null!;

    public Image visorLayer = null!;
    public Canvas visorLayerCanvas = null!;

    public Image skinLayer = null!;

    public Image petLayer = null!;

    public Image nameplateLayer = null!;
    
    private Material? SharedMaterial { get; set; }

    public void Initialize()
    {
        SharedMaterial = new Material(HatManager.Instance.PlayerMaterial);
        playerImage.material = SharedMaterial;
    }

    public void RefreshSharedMaterial(int colorId, Color visorColor)
    {
        if (SharedMaterial == null) return;
        SharedMaterial.SetColor(PlayerMaterial.BackColor, Palette.ShadowColors[colorId]);
        SharedMaterial.SetColor(PlayerMaterial.BodyColor, Palette.PlayerColors[colorId]);
        SharedMaterial.SetColor(PlayerMaterial.VisorColor, visorColor);
    }

    public void SetNameplate(string nameplateId)
    {
        var nameplate = HatManager.Instance.GetNamePlateById(nameplateId);
        var adressableAsset = nameplate.CreateAddressableAsset();
        adressableAsset.LoadAsync(new Action(() =>
        {
            var asset = adressableAsset.GetAsset();
            if (asset.Image == null)
            {
                nameplateLayer.enabled = false;
                return;
            }
            nameplateLayer.enabled = true;
            nameplateLayer.sprite = asset.Image;
        }));
    }

    public void SetPet(string petId)
    {
        var pet = HatManager.Instance.GetPetById(petId);
        pet.CoLoadPreview(new Action<Sprite, AddressableAsset>((sprite, _) =>
        {
            if (sprite == null)
            {
                petLayer.enabled = false;
                return;
            }
            petLayer.enabled = true;
            petLayer.sprite = sprite;
            petLayer.material = pet.PreviewCrewmateColor ? SharedMaterial : null;
        }));
    }

    public void SetSkin(string skinId)
    {
        var skin = HatManager.Instance.GetSkinById(skinId);
        var adressableAsset = skin.CreateAddressableAsset();
        adressableAsset.LoadAsync(new Action(() =>
        {
            var asset = adressableAsset.GetAsset();
            if (asset.IdleFrame == null)
            {
                skinLayer.enabled = false;
                return;
            }
            skinLayer.enabled = true;
            SetSkinSprite(ref skinLayer, asset.IdleFrame);
            skinLayer.material = asset.MatchPlayerColor ? SharedMaterial : null;
        }));
    }

    public void SetVisor(string visorId)
    {
        var visor = HatManager.Instance.GetVisorById(visorId);
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

    public void SetHat(string hatId)
    {
        var hat = HatManager.Instance.GetHatById(hatId);
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
    
    private void SetVisor(VisorViewData asset, bool behindHat)
    {
        if (asset.IdleFrame == null)
        {
            visorLayer.enabled = false;
            return;
        }
        visorLayer.enabled = true;
        SetVisorSprite(ref visorLayer, ref visorLayerCanvas, asset.IdleFrame, behindHat);
        visorLayer.material = asset.MatchPlayerColor ? SharedMaterial : null;
    }
    
    private void SetHat(HatViewData asset, bool inFront)
    {
        if (asset.MainImage == null)
        {
            hatFrontLayer.enabled = false;
        }
        else
        {
            hatFrontLayer.enabled = true;
            SetHatSprite(ref hatFrontLayer, ref hatFrontLayerCanvas, asset.MainImage, inFront || asset.BackImage != null);
            hatFrontLayer.material = asset.MatchPlayerColor ? SharedMaterial : null;
        }

        if (asset.BackImage == null)
        {
            hatBackLayer.enabled = false;
        }
        else
        {
            hatBackLayer.enabled = true;
            SetHatSprite(ref hatBackLayer, ref hatBackLayerCanvas, asset.BackImage, false);
            hatBackLayer.material = asset.MatchPlayerColor ? SharedMaterial : null;
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
        // Setup visor sprite and size
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
    
    private static void SetSkinSprite(ref Image image, Sprite sprite)
    {
        // Setup skin sprite and size
        image.sprite = sprite;
        var spriteSize = new Vector2(sprite.rect.width, sprite.rect.height);
        image.rectTransform.sizeDelta = spriteSize * SkinScale;

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
        image.rectTransform.anchoredPosition = new Vector2(SkinXOffset, SkinYOffset);
    }
}