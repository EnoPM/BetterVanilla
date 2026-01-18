# BetterVanilla.Ui

Système de génération d'interfaces Unity UI inspiré de WPF/XAML, avec génération de code au build time, data binding bidirectionnel et compatibilité IL2CPP.

## Table des matières

- [Vue d'ensemble](#vue-densemble)
- [Architecture](#architecture)
- [Démarrage rapide](#démarrage-rapide)
- [Fichiers BVUI](#fichiers-bvui)
- [Layout et Positionnement](#layout-et-positionnement)
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
│   ├── LayoutExtensions.cs         # Types et extensions de layout
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
│   ├── DropdownControl.cs
│   ├── PanelControl.cs
│   ├── ScrollViewControl.cs
│   ├── ImageControl.cs
│   └── IconButtonControl.cs
├── Helpers/                        # Utilitaires
│   ├── ViewModelBase.cs            # Classe de base ViewModel
│   ├── AssetBundleManager.cs       # Chargement des AssetBundles
│   └── Disposable.cs               # Utilitaires IDisposable
├── Schemas/                        # Schémas XSD pour autocomplétion
│   ├── bvui.xsd                    # Schéma principal
│   └── bvui-xaml.xsd               # Schéma pour x:Name, x:Class
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

## Layout et Positionnement

BVUI supporte les attributs de layout pour contrôler la taille et le positionnement des éléments.

### Attributs de taille

| Attribut     | Type    | Description                    |
|--------------|---------|--------------------------------|
| `Width`      | `float` | Largeur en pixels (-1 = auto)  |
| `Height`     | `float` | Hauteur en pixels (-1 = auto)  |
| `MinWidth`   | `float` | Largeur minimale               |
| `MinHeight`  | `float` | Hauteur minimale               |
| `MaxWidth`   | `float` | Largeur maximale               |
| `MaxHeight`  | `float` | Hauteur maximale               |

### Attributs d'espacement

| Attribut  | Type        | Description                              |
|-----------|-------------|------------------------------------------|
| `Margin`  | `Thickness` | Marge externe (espace autour du control) |
| `Padding` | `Thickness` | Marge interne (pour les conteneurs)      |

**Formats de Thickness :**
- `"10"` - Valeur uniforme (10 sur tous les côtés)
- `"10,5"` - Horizontal, Vertical (gauche/droite=10, haut/bas=5)
- `"10,5,15,20"` - Left, Top, Right, Bottom

### Attributs d'alignement

| Attribut              | Valeurs                         | Description                   |
|-----------------------|---------------------------------|-------------------------------|
| `HorizontalAlignment` | `Left`, `Center`, `Right`, `Stretch` | Alignement horizontal    |
| `VerticalAlignment`   | `Top`, `Center`, `Bottom`, `Stretch` | Alignement vertical      |

### Exemple complet

```xml
<Panel x:Name="root" Alias="Windows/Panel"
       Width="400" Height="500" Padding="20">

  <TextBlock x:Name="title"
             Text="Options"
             Height="40"
             HorizontalAlignment="Center" />

  <Toggle x:Name="toggle"
          Text="Enable Feature"
          Margin="0,10,0,0" />

  <Button x:Name="saveBtn"
          Text="Save"
          Width="150" Height="40"
          Margin="0,20,0,0"
          HorizontalAlignment="Center" />
</Panel>
```

### Code généré

Le générateur produit automatiquement le code de layout :

```csharp
root.Width = 400f;
root.Height = 500f;
root.ApplyLayout();

title.Height = 40f;
title.HorizontalAlignment = HorizontalAlignment.Center;
title.ApplyLayout();

toggle.Margin = Thickness.Parse("0,10,0,0");
toggle.ApplyLayout();

saveBtn.Width = 150f;
saveBtn.Height = 40f;
saveBtn.Margin = Thickness.Parse("0,20,0,0");
saveBtn.HorizontalAlignment = HorizontalAlignment.Center;
saveBtn.ApplyLayout();
```

### Modification en code

Vous pouvez également modifier le layout en code :

```csharp
// Modifier la taille
myButton.Width = 200f;
myButton.Height = 50f;

// Modifier la marge
myButton.Margin = new Thickness(10, 20, 10, 20);

// Modifier l'alignement
myButton.HorizontalAlignment = HorizontalAlignment.Right;
myButton.VerticalAlignment = VerticalAlignment.Bottom;

// Appliquer les changements
myButton.ApplyLayout();
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

Conteneur pour d'autres contrôles avec support de layout (LayoutGroup).

```xml
<Panel x:Name="root" Alias="Controls/Panel"
       Background="#1A1A2E"
       Orientation="Vertical"
       Padding="10"
       Spacing="8"
       ChildAlignment="UpperCenter"
       ChildControlWidth="true"
       ChildControlHeight="false"
       ChildForceExpandWidth="false"
       ChildForceExpandHeight="false">
  <!-- Enfants -->
</Panel>
```

| Propriété              | Type             | Description                                          |
|------------------------|------------------|------------------------------------------------------|
| `Background`           | `Color` (hex)    | Couleur de fond (#RGB, #RRGGBB, #RRGGBBAA)           |
| `Orientation`          | `Orientation`    | Direction du layout: `None`, `Vertical`, `Horizontal`|
| `Padding`              | `Thickness`      | Marge interne du conteneur                           |
| `Spacing`              | `float`          | Espacement entre les enfants                         |
| `ChildAlignment`       | `ChildAlignment` | Alignement des enfants dans le conteneur             |
| `ChildControlWidth`    | `bool`           | Le layout contrôle la largeur des enfants            |
| `ChildControlHeight`   | `bool`           | Le layout contrôle la hauteur des enfants            |
| `ChildForceExpandWidth`| `bool`           | Force les enfants à remplir la largeur disponible    |
| `ChildForceExpandHeight`| `bool`          | Force les enfants à remplir la hauteur disponible    |
| `ReverseArrangement`   | `bool`           | Inverse l'ordre des enfants                          |

**Valeurs de ChildAlignment :** `UpperLeft`, `UpperCenter`, `UpperRight`, `MiddleLeft`, `MiddleCenter`, `MiddleRight`, `LowerLeft`, `LowerCenter`, `LowerRight`

### DropdownControl

Liste déroulante pour sélection d'options.

```xml
<Dropdown x:Name="difficultyDropdown"
          Text="Difficulty:"
          Options="{Binding DifficultyOptions}"
          SelectedIndex="{Binding SelectedDifficulty, Mode=TwoWay}"
          SelectedIndexChanged="OnDifficultyChanged" />
```

| Propriété       | Type                   | Description                              |
|-----------------|------------------------|------------------------------------------|
| `Text`          | `string`               | Label affiché à côté du dropdown         |
| `Options`       | `IEnumerable<string>`  | Liste des options (bindable)             |
| `SelectedIndex` | `int`                  | Index de l'option sélectionnée (0-based) |
| `Value`         | `int`                  | Alias pour `SelectedIndex`               |
| `SelectedText`  | `string` (readonly)    | Texte de l'option sélectionnée           |

| Événement              | Signature      | Description                          |
|------------------------|----------------|--------------------------------------|
| `ValueChanged`         | `Action<int>`  | Déclenché au changement de sélection |
| `SelectedIndexChanged` | `Action<int>`  | Alias pour `ValueChanged`            |

**Configuration des options en code :**

```csharp
// Depuis une liste
difficultyDropdown.SetOptions(new[] { "Easy", "Normal", "Hard" });

// Depuis un enum
difficultyDropdown.SetOptionsFromEnum<DifficultyLevel>();

// Ajouter une option
difficultyDropdown.AddOption("Expert");

// Vider les options
difficultyDropdown.ClearOptions();
```

### ScrollViewControl

Conteneur scrollable pour listes et contenus longs.

```xml
<ScrollView x:Name="optionsScrollView"
            HorizontalAlignment="Stretch"
            FlexibleHeight="1"
            Orientation="Vertical"
            Spacing="8"
            Padding="10"
            Vertical="true"
            Horizontal="false"
            ChildControlWidth="true"
            ChildControlHeight="false"
            ChildForceExpandWidth="false"
            ChildForceExpandHeight="false">

    <Button x:Name="item1" Text="Item 1" />
    <Button x:Name="item2" Text="Item 2" />
    <Button x:Name="item3" Text="Item 3" />
    <!-- ... plus d'éléments ... -->

</ScrollView>
```

| Propriété           | Type             | Description                                          |
|---------------------|------------------|------------------------------------------------------|
| `Background`        | `Color` (hex)    | Couleur de fond                                      |
| `Horizontal`        | `bool`           | Active le scroll horizontal (défaut: false)          |
| `Vertical`          | `bool`           | Active le scroll vertical (défaut: true)             |
| `Inertia`           | `bool`           | Active l'inertie (défaut: true)                      |
| `DecelerationRate`  | `float`          | Taux de décélération (0-1, défaut: 0.135)            |
| `Elasticity`        | `float`          | Élasticité aux bords (défaut: 0.1)                   |
| `ScrollSensitivity` | `float`          | Sensibilité de la molette (défaut: 1)                |
| `Orientation`       | `Orientation`    | Layout du contenu: `Vertical`, `Horizontal`, `None`  |
| `Spacing`           | `float`          | Espacement entre les enfants                         |
| `Padding`           | `Thickness`      | Marge interne du contenu                             |
| `ChildAlignment`    | `ChildAlignment` | Alignement des enfants                               |
| `ChildControlWidth` | `bool`           | Le layout contrôle la largeur des enfants            |
| `ChildControlHeight`| `bool`           | Le layout contrôle la hauteur des enfants            |

**Méthodes utilitaires :**

```csharp
// Aller en haut
optionsScrollView.ScrollToTop();

// Aller en bas
optionsScrollView.ScrollToBottom();

// Position normalisée (0-1)
optionsScrollView.NormalizedPosition = new Vector2(0, 0.5f);
```

### ImageControl

Affichage d'images.

```xml
<Image x:Name="icon"
       Source="BetterVanilla.Ui.Assets.Images.MyIcon.png"
       PreserveAspect="true"
       Width="64" Height="64" />
```

| Propriété       | Type        | Description                                    |
|-----------------|-------------|------------------------------------------------|
| `Source`        | `string`    | Chemin de la ressource embarquée               |
| `Color`         | `Color`     | Teinte de l'image                              |
| `PreserveAspect`| `bool`      | Conserver le ratio d'aspect                    |
| `ImageType`     | `ImageType` | Type: `Simple`, `Sliced`, `Tiled`, `Filled`    |
| `FillCenter`    | `bool`      | Remplir le centre (pour Sliced)                |

### IconButtonControl

Bouton avec icône.

```xml
<IconButton x:Name="closeButton"
            Source="BetterVanilla.Ui.Assets.Images.CloseIcon.png"
            PreserveAspect="true"
            Width="40" Height="40"
            Background="#FF000080"
            Click="OnCloseClicked" />
```

| Propriété       | Type        | Description                       |
|-----------------|-------------|-----------------------------------|
| `Source`        | `string`    | Chemin de l'icône                 |
| `PreserveAspect`| `bool`      | Conserver le ratio d'aspect       |
| `Background`    | `Color`     | Couleur de fond du bouton         |
| `ImageType`     | `ImageType` | Type d'image                      |

| Événement | Signature | Description       |
|-----------|-----------|-------------------|
| `Clicked` | `Action`  | Déclenché au clic |

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
    "Panel": {
      "prefab": "Assets/Prefabs/Controls/Panel.prefab",
      "component": "BetterVanilla.Ui.Controls.PanelControl"
    },
    "ScrollView": {
      "prefab": "Assets/Prefabs/Controls/ScrollView.prefab",
      "component": "BetterVanilla.Ui.Controls.ScrollViewControl"
    },
    "Button": {
      "prefab": "Assets/Prefabs/Controls/Button.prefab",
      "component": "BetterVanilla.Ui.Controls.ButtonControl"
    },
    "Toggle": {
      "prefab": "Assets/Prefabs/Controls/Toggle.prefab",
      "component": "BetterVanilla.Ui.Controls.ToggleControl"
    },
    "Slider": {
      "prefab": "Assets/Prefabs/Controls/Slider.prefab",
      "component": "BetterVanilla.Ui.Controls.SliderControl"
    },
    "TextBlock": {
      "prefab": "Assets/Prefabs/Controls/TextBlock.prefab",
      "component": "BetterVanilla.Ui.Controls.TextBlockControl"
    },
    "InputField": {
      "prefab": "Assets/Prefabs/Controls/InputField.prefab",
      "component": "BetterVanilla.Ui.Controls.InputFieldControl"
    },
    "Dropdown": {
      "prefab": "Assets/Prefabs/Controls/Dropdown.prefab",
      "component": "BetterVanilla.Ui.Controls.DropdownControl"
    },
    "Image": {
      "prefab": "Assets/Prefabs/Controls/Image.prefab",
      "component": "BetterVanilla.Ui.Controls.ImageControl"
    },
    "IconButton": {
      "prefab": "Assets/Prefabs/Controls/IconButton.prefab",
      "component": "BetterVanilla.Ui.Controls.IconButtonControl"
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
      xmlns:x="http://schemas.bettervanilla.ui/2025/xaml"
      x:Class="BetterVanilla.Ui.Views.OptionsView">

  <Panel x:Name="root"
         Width="600"
         HorizontalAlignment="Left"
         VerticalAlignment="Stretch"
         Margin="20"
         Background="#1A1A2E"
         Orientation="Vertical"
         Padding="10"
         Spacing="8"
         ChildForceExpandWidth="false"
         ChildForceExpandHeight="false"
         ChildControlHeight="false"
         ChildAlignment="UpperCenter">

    <!-- Header avec titre et bouton fermer -->
    <Panel x:Name="header" HorizontalAlignment="Stretch" Height="50">
      <TextBlock x:Name="titleText"
                 Text="{Binding Username}"
                 FontSize="40"
                 TextAlignment="MidlineLeft"
                 Margin="20,0,0,0"
                 Height="50"
                 Width="480"
                 HorizontalAlignment="Left" />

      <IconButton x:Name="closeButton"
                  HorizontalAlignment="Right"
                  Margin="5"
                  Height="50" Width="50"
                  Source="BetterVanilla.Ui.Assets.Images.CloseButtonIcon.png"
                  PreserveAspect="true"
                  Click="OnCloseClicked" />
    </Panel>

    <!-- Zone scrollable pour les options -->
    <ScrollView x:Name="optionsScrollView"
                HorizontalAlignment="Stretch"
                FlexibleHeight="1"
                Orientation="Vertical"
                Spacing="8"
                Padding="5"
                Vertical="true"
                Horizontal="false"
                ChildControlWidth="true"
                ChildForceExpandWidth="false"
                ChildForceExpandHeight="false"
                ChildControlHeight="false">

      <Toggle x:Name="enableFeatureToggle"
              Text="Enable Feature"
              HorizontalAlignment="Stretch"
              IsOn="{Binding IsFeatureEnabled, Mode=TwoWay}"
              ValueChanged="OnFeatureToggleChanged">
        <Toggle.TextStyle MinFontSize="5" MaxFontSize="50" FontStyle="Bold" />
      </Toggle>

      <Slider x:Name="volumeSlider"
              Text="Volume"
              HorizontalAlignment="Stretch"
              Value="{Binding Volume, Mode=TwoWay}"
              ValueChanged="OnVolumeChanged" />

      <InputField x:Name="usernameInput"
                  Height="40"
                  HorizontalAlignment="Stretch"
                  Text="{Binding Username, Mode=TwoWay}"
                  ValueChanged="OnUsernameChanged">
        <InputField.TextStyle TextAlignment="MidlineLeft" />
      </InputField>

      <Dropdown x:Name="difficultyDropdown"
                Options="{Binding DifficultyOptions}"
                SelectedIndex="{Binding SelectedDifficulty, Mode=TwoWay}"
                HorizontalAlignment="Stretch"
                SelectedIndexChanged="OnDifficultyChanged" />

    </ScrollView>

    <!-- Boutons en bas -->
    <Button x:Name="saveButton"
            Text="Save Settings"
            Margin="10"
            Width="150" Height="40"
            HorizontalAlignment="Center"
            Click="OnSaveClicked" />

    <Button x:Name="cancelButton"
            Text="Cancel"
            Width="150" Height="40"
            HorizontalAlignment="Right"
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
    public event Action? Closed;

    private OptionsViewModel ViewModel => GetRequiredViewModel<OptionsViewModel>();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        titleText.Text = "Game Options";
    }

    partial void OnCloseClicked()
    {
        Closed?.Invoke();
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

    partial void OnDifficultyChanged(int index)
    {
        Debug.Log($"Difficulty changed to index: {index}");
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
using System.Collections.Generic;
using BetterVanilla.Ui.Helpers;
using UnityEngine;

namespace BetterVanilla.Ui.Views;

public class OptionsViewModel : ViewModelBase
{
    private bool _isFeatureEnabled;
    private float _volume = 1.0f;
    private string _username = string.Empty;
    private int _selectedDifficulty;

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

    // Liste des options pour le Dropdown (bindable)
    public IEnumerable<string> DifficultyOptions { get; } = new[]
    {
        "Easy",
        "Normal",
        "Hard",
        "Expert"
    };

    public int SelectedDifficulty
    {
        get => _selectedDifficulty;
        set => SetProperty(ref _selectedDifficulty, value);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("FeatureEnabled", IsFeatureEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("Volume", Volume);
        PlayerPrefs.SetString("Username", Username);
        PlayerPrefs.SetInt("Difficulty", SelectedDifficulty);
        PlayerPrefs.Save();
    }

    public void ResetToDefaults()
    {
        IsFeatureEnabled = false;
        Volume = 1.0f;
        Username = string.Empty;
        SelectedDifficulty = 1; // Normal
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
