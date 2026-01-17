# BetterVanilla.Ui

Système de génération d'interfaces Unity UI inspiré de WPF/XAML, avec génération de code au build time, data binding bidirectionnel et compatibilité IL2CPP.

## Table des matières

- [Vue d'ensemble](#vue-densemble)
- [Architecture](#architecture)
- [Démarrage rapide](#démarrage-rapide)
- [Fichiers BVUI](#fichiers-bvui)
- [Contrôles disponibles](#contrôles-disponibles)
- [Data Binding](#data-binding)
- [Event Handlers](#event-handlers)
- [ViewModels](#viewmodels)
- [Alias et Prefabs](#alias-et-prefabs)
- [Extensions IL2CPP](#extensions-il2cpp)
- [Référence MSBuild](#référence-msbuild)

## Vue d'ensemble

BetterVanilla.Ui permet de définir des interfaces utilisateur dans des fichiers XML (`.bvui.xml`) qui sont automatiquement compilés en code C# au moment du build. Ce système offre :

- **Déclaration XML** : Définissez vos vues de manière déclarative
- **Code-behind** : Séparez la logique de la présentation avec des classes partielles
- **Data Binding** : Liaison de données bidirectionnelle avec les ViewModels
- **Compatibilité IL2CPP** : Conçu pour fonctionner avec Unity IL2CPP

## Architecture

```
MyView.bvui.xml  ──┐
                   ├──[MSBuild]──> MyView.g.cs (généré)
ui-aliases.json  ──┘
MyView.cs (code utilisateur, classe partielle)
```

### Structure des fichiers

```
BetterVanilla.Ui/
├── Build/                          # Système de build MSBuild
│   ├── BetterVanilla.Ui.Xaml.props
│   └── BetterVanilla.Ui.Xaml.targets
├── Core/                           # Classes de base
│   ├── BaseView.cs                 # Classe de base pour les vues
│   ├── BaseControl.cs              # Classe de base pour les contrôles
│   ├── IViewControl.cs             # Interface des contrôles
│   ├── ViewElementAttribute.cs     # Attribut pour les éléments de vue
│   └── UnityEventExtensions.cs     # Extensions IL2CPP
├── Binding/                        # Système de binding
│   ├── BindingEngine.cs            # Moteur de binding
│   ├── BindingDefinition.cs        # Définition d'un binding
│   ├── BindableProperty.cs         # Propriété bindable
│   └── Converters/                 # Convertisseurs de valeurs
├── Controls/                       # Contrôles UI
│   ├── ButtonControl.cs
│   ├── ToggleControl.cs
│   ├── SliderControl.cs
│   ├── TextBlockControl.cs
│   ├── InputFieldControl.cs
│   └── PanelControl.cs
├── Helpers/                        # Utilitaires
│   ├── ViewModelBase.cs            # Classe de base ViewModel
│   ├── AssetBundleManager.cs       # Chargement des AssetBundles
│   └── Disposable.cs               # Utilitaires IDisposable
├── Views/                          # Vues de l'application
├── Tools/                          # Outils de génération
│   └── BetterVanilla.Ui.XamlGenerator/
├── ui-aliases.json                 # Configuration des alias
└── ui-aliases.schema.json          # Schéma JSON
```

## Démarrage rapide

### 1. Créer un fichier BVUI

Créez un fichier `MyView.bvui.xml` :

```xml
<?xml version="1.0" encoding="utf-8"?>
<View xmlns="http://schemas.bettervanilla.ui/2025"
      x:Class="BetterVanilla.Ui.Views.MyView">

  <Panel x:Name="root" Alias="Windows/Panel">
    <TextBlock x:Name="title" Alias="Controls/TextBlock" Text="Hello World" />
    <Button x:Name="okButton" Alias="Controls/Button" Text="OK" Click="OnOkClicked" />
  </Panel>
</View>
```

### 2. Créer le code-behind

Créez `MyView.cs` :

```csharp
using BetterVanilla.Ui.Core;
using UnityEngine;

namespace BetterVanilla.Ui.Views;

public partial class MyView : BaseView
{
    partial void OnOkClicked()
    {
        Debug.Log("OK clicked!");
    }
}
```

### 3. Build

Le build génère automatiquement `MyView.g.cs` avec :
- Les propriétés pour chaque élément nommé
- La méthode `InitializeComponent()`
- Le câblage des event handlers
- La configuration des bindings

## Fichiers BVUI

### Structure XML

```xml
<?xml version="1.0" encoding="utf-8"?>
<View xmlns="http://schemas.bettervanilla.ui/2025"
      x:Class="Namespace.ClassName">

  <!-- Éléments de la vue -->

</View>
```

### Attributs des éléments

| Attribut | Description                                    |
|----------|------------------------------------------------|
| `x:Name` | Nom unique de l'élément (génère une propriété) |
| `Alias`  | Référence à un prefab dans `ui-aliases.json`   |
| `Text`   | Texte affiché (pour les contrôles textuels)    |
| `IsOn`   | État du toggle (pour ToggleControl)            |
| `Value`  | Valeur (pour SliderControl, InputFieldControl) |

### Bindings

Utilisez la syntaxe `{Binding}` pour lier une propriété à un ViewModel :

```xml
<Toggle x:Name="myToggle"
        IsOn="{Binding IsEnabled, Mode=TwoWay}" />

<Slider x:Name="volumeSlider"
        Value="{Binding Volume, Mode=TwoWay}" />

<TextBlock x:Name="status"
           Text="{Binding StatusMessage}" />
```

### Modes de binding

| Mode             | Description                       |
|------------------|-----------------------------------|
| `OneWay`         | ViewModel → Vue (par défaut)      |
| `TwoWay`         | ViewModel ↔ Vue                   |
| `OneWayToSource` | Vue → ViewModel                   |
| `OneTime`        | Une seule fois à l'initialisation |

### Event Handlers

```xml
<Button x:Name="saveBtn" Click="OnSaveClicked" />
<Toggle x:Name="toggle" ValueChanged="OnToggleChanged" />
<Slider x:Name="slider" ValueChanged="OnSliderChanged" />
```

## Contrôles disponibles

### ButtonControl

Bouton cliquable.

```xml
<Button x:Name="myButton" Alias="Controls/Button" Text="Click me" Click="OnClick" />
```

| Propriété   | Type     | Description           |
|-------------|----------|-----------------------|
| `Text`      | `string` | Texte du bouton       |
| `IsEnabled` | `bool`   | État activé/désactivé |

| Événement | Signature | Description       |
|-----------|-----------|-------------------|
| `Clicked` | `Action`  | Déclenché au clic |

### ToggleControl

Case à cocher / Toggle.

```xml
<Toggle x:Name="myToggle" Alias="Controls/Toggle"
        Text="Enable feature"
        IsOn="{Binding IsFeatureEnabled, Mode=TwoWay}"
        ValueChanged="OnToggleChanged" />
```

| Propriété        | Type     | Description           |
|------------------|----------|-----------------------|
| `Text`           | `string` | Label du toggle       |
| `IsOn` / `Value` | `bool`   | État coché/décoché    |
| `IsEnabled`      | `bool`   | État activé/désactivé |

| Événement      | Signature      | Description             |
|----------------|----------------|-------------------------|
| `ValueChanged` | `Action<bool>` | Déclenché au changement |

### SliderControl

Curseur pour valeurs numériques.

```xml
<Slider x:Name="volumeSlider" Alias="Controls/Slider"
        Text="Volume"
        Value="{Binding Volume, Mode=TwoWay}"
        ValueChanged="OnVolumeChanged" />
```

| Propriété      | Type     | Description                         |
|----------------|----------|-------------------------------------|
| `Text`         | `string` | Label du slider                     |
| `Value`        | `float`  | Valeur actuelle                     |
| `MinValue`     | `float`  | Valeur minimale                     |
| `MaxValue`     | `float`  | Valeur maximale                     |
| `WholeNumbers` | `bool`   | Restreindre aux entiers             |
| `ValueFormat`  | `string` | Format d'affichage (ex: `"{0:F1}"`) |

| Événement      | Signature       | Description             |
|----------------|-----------------|-------------------------|
| `ValueChanged` | `Action<float>` | Déclenché au changement |

### TextBlockControl

Affichage de texte.

```xml
<TextBlock x:Name="statusText" Alias="Controls/TextBlock"
           Text="{Binding StatusMessage}" />
```

| Propriété       | Type                   | Description      |
|-----------------|------------------------|------------------|
| `Text`          | `string`               | Texte affiché    |
| `TextColor`     | `Color`                | Couleur du texte |
| `FontSize`      | `float`                | Taille de police |
| `TextAlignment` | `TextAlignmentOptions` | Alignement       |

### InputFieldControl

Champ de saisi texte.

```xml
<InputField x:Name="usernameInput" Alias="Controls/InputField"
            Text="{Binding Username, Mode=TwoWay}"
            ValueChanged="OnUsernameChanged" />
```

| Propriété        | Type          | Description          |
|------------------|---------------|----------------------|
| `Text` / `Value` | `string`      | Texte saisi          |
| `Placeholder`    | `string`      | Texte placeholder    |
| `ContentType`    | `ContentType` | Type de contenu      |
| `CharacterLimit` | `int`         | Limite de caractères |

| Événement      | Signature        | Description                     |
|----------------|------------------|---------------------------------|
| `ValueChanged` | `Action<string>` | Déclenché à chaque modification |

### PanelControl

Conteneur pour d'autres contrôles.

```xml
<Panel x:Name="root" Alias="Windows/Panel">
  <!-- Enfants -->
</Panel>
```

| Propriété   | Type   | Description           |
|-------------|--------|-----------------------|
| `IsEnabled` | `bool` | État activé/désactivé |

## Data Binding

### Configuration dans le ViewModel

Héritez de `ViewModelBase` pour bénéficier de `INotifyPropertyChanged` :

```csharp
public class MyViewModel : ViewModelBase
{
    private bool _isEnabled;
    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    private float _volume = 0.5f;
    public float Volume
    {
        get => _volume;
        set => SetProperty(ref _volume, value);
    }
}
```

### Assignation du DataContext

```csharp
var view = GetComponent<MyView>();
view.DataContext = new MyViewModel();
```

### Accès au ViewModel typé

Dans votre vue, utilisez `GetRequiredViewModel<T>()` :

```csharp
public partial class MyView : BaseView
{
    private MyViewModel ViewModel => GetRequiredViewModel<MyViewModel>();

    partial void OnSaveClicked()
    {
        ViewModel.Save();
    }
}
```

## Event Handlers

Les event handlers sont déclarés comme des méthodes partielles dans le code généré :

```csharp
// Généré dans MyView.g.cs
partial void OnButtonClicked();
partial void OnToggleChanged(bool value);
partial void OnSliderChanged(float value);
partial void OnInputChanged(string value);
```

Implémentez-les dans votre code-behind :

```csharp
// MyView.cs
public partial class MyView : BaseView
{
    partial void OnButtonClicked()
    {
        Debug.Log("Button clicked!");
    }

    partial void OnToggleChanged(bool value)
    {
        Debug.Log($"Toggle changed to: {value}");
    }
}
```

## ViewModels

### Classe de base

```csharp
public class MyViewModel : ViewModelBase
{
    private string _title = "Default Title";
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    // Commande
    public void ExecuteCommand()
    {
        // Logique
    }
}
```

### Pattern recommandé

```csharp
public class OptionsViewModel : ViewModelBase
{
    private bool _isFeatureEnabled;
    private float _volume = 1.0f;
    private string _username = string.Empty;

    public bool IsFeatureEnabled
    {
        get => _isFeatureEnabled;
        set => SetProperty(ref _isFeatureEnabled, value);
    }

    public float Volume
    {
        get => _volume;
        set => SetProperty(ref _volume, Mathf.Clamp01(value));
    }

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value ?? string.Empty);
    }

    public void SaveSettings()
    {
        // Sauvegarder les paramètres
    }

    public void ResetToDefaults()
    {
        IsFeatureEnabled = false;
        Volume = 1.0f;
        Username = string.Empty;
    }
}
```

## Alias et Prefabs

### Configuration (ui-aliases.json)

```json
{
  "$schema": "./ui-aliases.schema.json",
  "version": "1.0",
  "defaultBundle": "BetterVanilla.Ui.Assets.ui.bundle",
  "aliases": {
    "Windows/OptionsPanel": {
      "prefab": "Assets/Prefabs/Windows/OptionsPanel.prefab",
      "component": "BetterVanilla.Ui.Controls.PanelControl"
    },
    "Controls/Button": {
      "prefab": "Assets/Prefabs/Controls/Button.prefab",
      "component": "BetterVanilla.Ui.Controls.ButtonControl"
    },
    "Controls/Toggle": {
      "prefab": "Assets/Prefabs/Controls/Toggle.prefab",
      "component": "BetterVanilla.Ui.Controls.ToggleControl"
    },
    "Controls/Slider": {
      "prefab": "Assets/Prefabs/Controls/Slider.prefab",
      "component": "BetterVanilla.Ui.Controls.SliderControl"
    },
    "Controls/TextBlock": {
      "prefab": "Assets/Prefabs/Controls/TextBlock.prefab",
      "component": "BetterVanilla.Ui.Controls.TextBlockControl"
    },
    "Controls/InputField": {
      "prefab": "Assets/Prefabs/Controls/InputField.prefab",
      "component": "BetterVanilla.Ui.Controls.InputFieldControl"
    }
  }
}
```

### Propriétés d'alias

| Propriété   | Description                          |
|-------------|--------------------------------------|
| `prefab`    | Chemin du prefab dans l'AssetBundle  |
| `component` | Type complet du composant à attacher |
| `bundle`    | (Optionnel) AssetBundle spécifique   |

## Extensions IL2CPP

Pour la compatibilité IL2CPP, utilisez les extensions pour les événements Unity :

```csharp
using BetterVanilla.Ui.Core;

// Au lieu de :
button.onClick.AddListener(OnClick);  // Erreur IL2CPP

// Utilisez :
button.onClick.AddListener(OnClick);  // Fonctionne grâce aux extensions
```

### Extensions disponibles

```csharp
// UnityEvent (sans paramètre)
unityEvent.AddListener(Action handler);
unityEvent.RemoveListener(Action handler);

// UnityEvent<bool>
unityEvent.AddListener(Action<bool> handler);
unityEvent.RemoveListener(Action<bool> handler);

// UnityEvent<float>
unityEvent.AddListener(Action<float> handler);
unityEvent.RemoveListener(Action<float> handler);

// UnityEvent<string>
unityEvent.AddListener(Action<string> handler);
unityEvent.RemoveListener(Action<string> handler);

// UnityEvent<int>
unityEvent.AddListener(Action<int> handler);
unityEvent.RemoveListener(Action<int> handler);
```

## Référence MSBuild

### Propriétés

| Propriété                 | Description                   | Défaut                                       |
|---------------------------|-------------------------------|----------------------------------------------|
| `EnableBvuiGeneration`    | Active la génération BVUI     | `true`                                       |
| `BvuiAliasesFile`         | Chemin vers `ui-aliases.json` | `$(MSBuildProjectDirectory)\ui-aliases.json` |
| `BvuiGeneratedOutputPath` | Dossier de sortie             | `$(MSBuildProjectDirectory)\BvuiGenerated`   |

### Configuration dans .csproj

```xml
<PropertyGroup>
    <EnableBvuiGeneration>true</EnableBvuiGeneration>
    <BvuiAliasesFile>$(MSBuildProjectDirectory)\ui-aliases.json</BvuiAliasesFile>
</PropertyGroup>

<!-- Import du système BVUI -->
<Import Project="Build\BetterVanilla.Ui.Xaml.props" />
<Import Project="Build\BetterVanilla.Ui.Xaml.targets" />
```

### Types d'éléments

| Type            | Description             |
|-----------------|-------------------------|
| `BvuiFile`      | Fichier BVUI à compiler |
| `BvuiGenerated` | Fichier C# généré       |

## Exemple complet

### OptionsView.bvui.xml

```xml
<?xml version="1.0" encoding="utf-8"?>
<View xmlns="http://schemas.bettervanilla.ui/2025"
      x:Class="BetterVanilla.Ui.Views.OptionsView">

  <Panel x:Name="root" Alias="Windows/OptionsPanel">
    <TextBlock x:Name="titleText" Alias="Controls/TextBlock" Text="Options" />

    <Toggle x:Name="enableFeatureToggle"
            Alias="Controls/Toggle"
            Text="Enable Feature"
            IsOn="{Binding IsFeatureEnabled, Mode=TwoWay}"
            ValueChanged="OnFeatureToggleChanged" />

    <Slider x:Name="volumeSlider"
            Alias="Controls/Slider"
            Text="Volume"
            Value="{Binding Volume, Mode=TwoWay}"
            ValueChanged="OnVolumeChanged" />

    <InputField x:Name="usernameInput"
                Alias="Controls/InputField"
                Text="{Binding Username, Mode=TwoWay}"
                ValueChanged="OnUsernameChanged" />

    <Button x:Name="saveButton"
            Alias="Controls/Button"
            Text="Save Settings"
            Click="OnSaveClicked" />

    <Button x:Name="cancelButton"
            Alias="Controls/Button"
            Text="Cancel"
            Click="OnCancelClicked" />
  </Panel>
</View>
```

### OptionsView.cs

```csharp
using System;
using BetterVanilla.Ui.Core;
using UnityEngine;

namespace BetterVanilla.Ui.Views;

public partial class OptionsView : BaseView
{
    public event Action? SettingsSaved;
    public event Action? Cancelled;

    private OptionsViewModel ViewModel => GetRequiredViewModel<OptionsViewModel>();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        titleText.Text = "Game Options";
    }

    partial void OnFeatureToggleChanged(bool value)
    {
        Debug.Log($"Feature toggle: {value}");
    }

    partial void OnVolumeChanged(float value)
    {
        Debug.Log($"Volume: {value:F2}");
    }

    partial void OnUsernameChanged(string value)
    {
        Debug.Log($"Username: {value}");
    }

    partial void OnSaveClicked()
    {
        ViewModel.SaveSettings();
        SettingsSaved?.Invoke();
    }

    partial void OnCancelClicked()
    {
        ViewModel.ResetToDefaults();
        Cancelled?.Invoke();
    }
}
```

### OptionsViewModel.cs

```csharp
using BetterVanilla.Ui.Helpers;
using UnityEngine;

namespace BetterVanilla.Ui.Views;

public class OptionsViewModel : ViewModelBase
{
    private bool _isFeatureEnabled;
    private float _volume = 1.0f;
    private string _username = string.Empty;

    public bool IsFeatureEnabled
    {
        get => _isFeatureEnabled;
        set => SetProperty(ref _isFeatureEnabled, value);
    }

    public float Volume
    {
        get => _volume;
        set => SetProperty(ref _volume, Mathf.Clamp01(value));
    }

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value ?? string.Empty);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("FeatureEnabled", IsFeatureEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("Volume", Volume);
        PlayerPrefs.SetString("Username", Username);
        PlayerPrefs.Save();
    }

    public void ResetToDefaults()
    {
        IsFeatureEnabled = false;
        Volume = 1.0f;
        Username = string.Empty;
    }
}
```

### Utilisation

```csharp
// Créer et configurer la vue
var viewGo = new GameObject("OptionsView");
var view = viewGo.AddComponent<OptionsView>();
view.DataContext = new OptionsViewModel
{
    IsFeatureEnabled = true,
    Volume = 0.75f,
    Username = "Player1"
};

// Écouter les événements
view.SettingsSaved += () => Debug.Log("Settings saved!");
view.Cancelled += () => Destroy(viewGo);
```

## Licence

MIT License - Voir LICENSE pour plus de détails.
