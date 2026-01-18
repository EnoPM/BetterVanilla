using BetterVanilla.Ui.Core;
using BetterVanilla.Ui.Helpers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BetterVanilla.Ui;

public sealed class UiManager
{
    public static readonly UiManager Instance = new();

    private GameObject? _uiContainer;

    private GameObject UiContainer
    {
        get
        {
            // Recreate if destroyed (can happen on scene change)
            if (_uiContainer == null)
            {
                CreateUiContainer();
            }
            return _uiContainer!;
        }
    }

    public UiManager()
    {
        CreateUiContainer();
    }

    private void CreateUiContainer()
    {
        _uiContainer = AssetBundleManager.Instance.InstantiatePrefab(
            "BetterVanilla.Ui.Assets.ui.bundle",
            "Assets/Prefabs/UiContainer.prefab"
        );
        _uiContainer.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        Object.DontDestroyOnLoad(_uiContainer);

        Plugin.Instance.Log.LogMessage("[UiManager] UiContainer created");
    }

    public TView CreateView<TView, TViewModel>(TViewModel viewModel)
        where TView : BaseView
        where TViewModel : ViewModelBase
    {
        var gameObject = new GameObject(typeof(TView).Name);
        gameObject.transform.SetParent(UiContainer.transform);

        var rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        var view = gameObject.AddComponent<TView>();
        view.DataContext = viewModel;

        return view;
    }
}
