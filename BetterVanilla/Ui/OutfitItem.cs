using System;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Cosmetics;
using Innersloth.Assets;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui;

public sealed class OutfitItem : MonoBehaviour
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

    public Image player = null!;

    public Image hatFrontLayer = null!;
    public Canvas hatFrontLayerCanvas = null!;

    public Image hatBackLayer = null!;
    public Canvas hatBackLayerCanvas = null!;

    public Image visorLayer = null!;
    public Canvas visorLayerCanvas = null!;

    public Image skinLayer = null!;

    public Image petLayer = null!;

    public Image nameplateLayer = null!;

    private HatViewData? CurrentHat { get; set; }
    private VisorViewData? CurrentVisor { get; set; }
    private SkinViewData? CurrentSkin { get; set; }
    private PetData? CurrentPet { get; set; }
    private int CurrentColorId { get; set; }
    private Color? CurrentVisorColor { get; set; }

    private Material CurrentMaterial => HatManager.Instance.PlayerMaterial;

    private void UpdateMaterialColors()
    {
        SetMaterialColors(player.material, CurrentColorId, CurrentVisorColor ?? Palette.VisorColor);
        if (CurrentHat != null && CurrentHat.MatchPlayerColor)
        {
            if (hatFrontLayer.enabled && hatFrontLayer.material != null)
            {
                SetMaterialColors(hatFrontLayer.material, CurrentColorId, CurrentVisorColor ?? Palette.VisorColor);
            }
            if (hatBackLayer.enabled && hatBackLayer.material != null)
            {
                SetMaterialColors(hatBackLayer.material, CurrentColorId, CurrentVisorColor ?? Palette.VisorColor);
            }
        }
        if (CurrentVisor != null && CurrentVisor.MatchPlayerColor)
        {
            if (visorLayer.enabled && visorLayer.material != null)
            {
                SetMaterialColors(visorLayer.material, CurrentColorId, CurrentVisorColor ?? Palette.VisorColor);
            }
        }
        if (CurrentSkin != null && CurrentSkin.MatchPlayerColor)
        {
            if (skinLayer.enabled && skinLayer.material != null)
            {
                SetMaterialColors(skinLayer.material, CurrentColorId, CurrentVisorColor ?? Palette.VisorColor);
            }
        }
        if (CurrentPet != null && CurrentPet.PreviewCrewmateColor)
        {
            if (petLayer.enabled && petLayer.material != null)
            {
                SetMaterialColors(petLayer.material, CurrentColorId, CurrentVisorColor ?? Palette.VisorColor);
            }
        }
    }

    public void SetColorId(int colorId)
    {
        player.material = new Material(CurrentMaterial);
        CurrentColorId = colorId;
        UpdateMaterialColors();
    }

    public void SetVisorColor(Color color)
    {
        CurrentVisorColor = color;
        UpdateMaterialColors();
    }

    public void SetNameplate(string nameplateId)
    {
        var nameplate = HatManager.Instance.GetNamePlateById(nameplateId);
        SetNameplate(nameplate);
    }

    public void SetNameplate(NamePlateData nameplate)
    {
        var adressableAsset = nameplate.CreateAddressableAsset();
        adressableAsset.LoadAsync(new Action(() =>
        {
            var asset = adressableAsset.GetAsset();
            SetNameplate(asset);
        }));
    }

    private void SetNameplate(NamePlateViewData asset)
    {
        if (asset.Image == null)
        {
            nameplateLayer.enabled = false;
            return;
        }
        nameplateLayer.enabled = true;
        SetNameplateSprite(ref nameplateLayer, asset.Image);
    }

    public void SetPet(string petId)
    {
        var pet = HatManager.Instance.GetPetById(petId);
        SetPet(pet);
    }

    public void SetPet(PetData pet)
    {
        pet.CoLoadPreview(new Action<Sprite, AddressableAsset>((sprite, _) =>
        {
            if (sprite == null)
            {
                petLayer.enabled = false;
                return;
            }
            petLayer.enabled = true;
            CurrentPet = pet;
            SetPetSprite(ref petLayer, sprite);
            if (pet.PreviewCrewmateColor)
            {
                petLayer.material = new Material(CurrentMaterial);
                UpdateMaterialColors();
            }
            else
            {
                petLayer.material = null;
            }
        }));
    }

    public void SetSkin(string skinId)
    {
        var skin = HatManager.Instance.GetSkinById(skinId);
        SetSkin(skin);
    }

    public void SetSkin(SkinData skin)
    {
        var adressableAsset = skin.CreateAddressableAsset();
        adressableAsset.LoadAsync(new Action(() =>
        {
            var asset = adressableAsset.GetAsset();
            SetSkin(asset);
        }));
    }

    public void SetSkin(SkinViewData skin)
    {
        if (skin.IdleFrame == null)
        {
            skinLayer.enabled = false;
            return;
        }
        skinLayer.enabled = true;
        CurrentSkin = skin;
        SetSkinSprite(ref skinLayer, skin.IdleFrame);
        if (skin.MatchPlayerColor)
        {
            skinLayer.material = new Material(CurrentMaterial);
            UpdateMaterialColors();
        }
        else
        {
            skinLayer.material = null;
        }
    }

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
        visorLayer.enabled = true;
        CurrentVisor = asset;
        SetVisorSprite(ref visorLayer, ref visorLayerCanvas, asset.IdleFrame, behindHat);
        if (CurrentVisor.MatchPlayerColor)
        {
            visorLayer.material = new Material(CurrentMaterial);
            UpdateMaterialColors();
        }
        else
        {
            visorLayer.material = null;
        }
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
            CurrentHat = asset;
            if (CurrentHat.MatchPlayerColor)
            {
                hatFrontLayer.material = new Material(CurrentMaterial);
                UpdateMaterialColors();
            }
            else
            {
                hatFrontLayer.material = null;
            }
        }

        if (asset.BackImage == null)
        {
            hatBackLayer.enabled = false;
        }
        else
        {
            hatBackLayer.enabled = true;
            SetHatSprite(ref hatBackLayer, ref hatBackLayerCanvas, asset.BackImage, false);
            CurrentHat = asset;
            if (CurrentHat.MatchPlayerColor)
            {
                hatBackLayer.material = new Material(CurrentMaterial);
                UpdateMaterialColors();
            }
            else
            {
                hatBackLayer.material = null;
            }
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

    private static void SetSkinSprite(ref Image image, Sprite sprite)
    {
        // Setup hat sprite and size
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

    private static void SetPetSprite(ref Image image, Sprite sprite)
    {
        // Setup hat sprite and size
        image.sprite = sprite;
    }

    private static void SetNameplateSprite(ref Image image, Sprite sprite)
    {
        // Setup hat sprite and size
        image.sprite = sprite;
    }

    private static void SetMaterialColors(Material material, int colorId, Color visorColor)
    {
        material.SetColor(PlayerMaterial.BackColor, Palette.ShadowColors[colorId]);
        material.SetColor(PlayerMaterial.BodyColor, Palette.PlayerColors[colorId]);
        material.SetColor(PlayerMaterial.VisorColor, visorColor);
    }
}