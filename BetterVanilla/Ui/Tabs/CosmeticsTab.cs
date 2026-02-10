using System.Collections;
using AmongUs.Data;
using BetterVanilla.Core;
using BetterVanilla.Ui.Base;
using EnoUnityLoader.Il2Cpp.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Tabs;

public sealed class CosmeticsTab : TabBase
{
    public Button saveCurrentOutfitButton = null!;
    public TextMeshProUGUI saveCurrentOutfitButtonText = null!;
    public SavedOutfitItem savedOutfitItemPrefab = null!;

    private void Start()
    {
        this.StartCoroutine(CoStart());
    }

    public void OnSaveCurrentOutfitButtonClicked()
    {
        var outfit = OutfitsManager.SaveEquipped();
        if (outfit == null) return;
        var savedOutfitItem = Instantiate(savedOutfitItemPrefab, container);
        savedOutfitItem.Outfit = outfit;
    }

    private IEnumerator CoStart()
    {
        while (!HatManager.InstanceExists)
        {
            yield return new WaitForEndOfFrame();
        }

        while (DataManager.Player == null || DataManager.Player.Customization == null)
        {
            yield return new WaitForEndOfFrame();
        }

        while (HatManager.Instance.PlayerMaterial == null)
        {
            yield return new WaitForEndOfFrame();
        }

        foreach (var outfit in OutfitsManager.AllOutfits)
        {
            var savedOutfitItem = Instantiate(savedOutfitItemPrefab, container);
            savedOutfitItem.Outfit = outfit;
        }
    }

    private void Update()
    {
        saveCurrentOutfitButton.interactable = !OutfitsManager.CurrentOutfitIsSaved;
    }

    protected override void SetupTranslation()
    {
        base.SetupTranslation();
        saveCurrentOutfitButtonText.SetText(UiLocalization.SaveCurrentOutfitButton);
    }
}