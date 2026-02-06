using System;
using System.Collections;
using BetterVanilla.Core;
using BetterVanilla.Ui.Base;
using EnoUnityLoader.Il2Cpp.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui;

public sealed class SavedOutfitItem : LocalizationBehaviourBase
{
    public PlayerPreview preview = null!;
    public Button applyButton = null!;
    public TextMeshProUGUI applyButtonText = null!;

    public OutfitsManager.Outfit? Outfit { get; set; }

    private void Start()
    {
        this.StartCoroutine(CoStart());
    }

    private IEnumerator CoStart()
    {
        while (Outfit == null)
        {
            yield return new WaitForEndOfFrame();
        }

        preview.Initialize();

        preview.SetHat(Outfit.HatId);
        preview.SetVisor(Outfit.VisorId);
        preview.SetSkin(Outfit.SkinId);
        preview.SetPet(Outfit.PetId);
        preview.SetNameplate(Outfit.NameplateId);
        preview.RefreshSharedMaterial(Outfit.ColorId, Outfit.VisorColor);
    }

    private void Update()
    {
        applyButton.interactable = Outfit != null && !Outfit.IsEquipped;
    }

    public void OnApplyButtonClicked()
    {

    }

    public void OnDeleteButtonClicked()
    {
        if (Outfit == null) return;
        OutfitsManager.Delete(Outfit);
        Destroy(gameObject);
    }

    protected override void SetupTranslation()
    {
        applyButtonText.SetText(UiLocalization.ApplyOutfitButton);
    }
}