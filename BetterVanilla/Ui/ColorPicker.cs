using System;
using BetterVanilla.Extensions;
using BetterVanilla.Options.Core.OptionTypes;
using BetterVanilla.Ui.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BetterVanilla.Ui;

public sealed class ColorPicker : OptionBase
{
    private static readonly int Hue = Shader.PropertyToID("_Hue");
    
    public RawImage hueBar = null!;
    public RawImage svSquare = null!;
    public RectTransform hueCursor = null!;
    public RectTransform svCursor = null!;
    public Image previewImage = null!;

    private Material _svMaterial = null!;
    private float _hue;
    private float _saturation = 1f;
    private float _value = 1f;

    private bool _isDraggingHue;
    private bool _isDraggingSv;

    public Color CurrentColor => Color.HSVToRGB(_hue, _saturation, _value);
    public event Action<Color>? ValueChanged;

    public ColorOption? Option
    {
        get;
        set
        {
            if (value == field) return;
            BaseOption = field = value;
            UpdateFromOption();
        }
    }

    private void Awake()
    {
        if (svSquare != null)
        {
            _svMaterial = Instantiate(svSquare.material);
            svSquare.material = _svMaterial;
        }

        SetupDragHandlers();
    }

    private void OnColorChanged(Color value)
    {
        Option?.Value = value;
        ValueChanged?.Invoke(value);
        TriggerValueUpdated();
    }

    private void SetupDragHandlers()
    {
        if (hueBar != null)
        {
            var hueTrigger = hueBar.gameObject.AddComponent<EventTrigger>();
            AddDragEvents(hueTrigger, new Action<BaseEventData>(OnHuePointerDown), new Action<BaseEventData>(OnHueDrag), new Action<BaseEventData>(OnHuePointerUp));
        }

        if (svSquare != null)
        {

            var svTrigger = svSquare.gameObject.AddComponent<EventTrigger>();
            AddDragEvents(svTrigger, new Action<BaseEventData>(OnSVPointerDown), new Action<BaseEventData>(OnSVDrag), new Action<BaseEventData>(OnSVPointerUp));
        }
    }

    private static void AddDragEvents(EventTrigger trigger,
        UnityAction<BaseEventData> pointerDown,
        UnityAction<BaseEventData> drag,
        UnityAction<BaseEventData> pointerUp)
    {
        var entryDown = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        entryDown.callback.AddListener(pointerDown);
        trigger.triggers.Add(entryDown);

        var entryDrag = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Drag
        };
        entryDrag.callback.AddListener(drag);
        trigger.triggers.Add(entryDrag);

        var entryUp = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        entryUp.callback.AddListener(pointerUp);
        trigger.triggers.Add(entryUp);
    }

    private void OnHuePointerDown(BaseEventData data)
    {
        _isDraggingHue = true;
        OnHueDrag(data);
    }

    private void OnHuePointerUp(BaseEventData data)
    {
        _isDraggingHue = false;
    }

    private void OnHueDrag(BaseEventData data)
    {
        if (!_isDraggingHue) return;

        var pointerData = data.As<PointerEventData>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            hueBar.rectTransform, pointerData.position, pointerData.pressEventCamera, out var localPos);

        var rect = hueBar.rectTransform.rect;
        _hue = Mathf.Clamp01((localPos.x - rect.x) / rect.width);

        UpdateCursors();
        UpdateColor();
    }

    private void OnSVPointerDown(BaseEventData data)
    {
        _isDraggingSv = true;
        OnSVDrag(data);
    }

    private void OnSVPointerUp(BaseEventData data)
    {
        _isDraggingSv = false;
    }

    private void OnSVDrag(BaseEventData data)
    {
        if (!_isDraggingSv) return;

        var pointerData = data.As<PointerEventData>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            svSquare.rectTransform, pointerData.position, pointerData.pressEventCamera, out var localPos);

        var rect = svSquare.rectTransform.rect;
        _saturation = Mathf.Clamp01((localPos.x - rect.x) / rect.width);
        _value = Mathf.Clamp01((localPos.y - rect.y) / rect.height);

        UpdateCursors();
        UpdateColor();
    }

    private void UpdateCursors()
    {
        if (hueCursor != null && hueBar != null)
        {
            var hueRect = hueBar.rectTransform.rect;
            hueCursor.anchoredPosition = new Vector2(_hue * hueRect.width, 0);
        }

        if (svCursor != null && svSquare != null)
        {
            var svRect = svSquare.rectTransform.rect;
            svCursor.anchoredPosition = new Vector2(_saturation * svRect.width, _value * svRect.height);
        }
    }

    private void UpdateColor()
    {
        if (_svMaterial != null)
        {
            _svMaterial.SetFloat(Hue, _hue);
        }

        var color = CurrentColor;

        if (previewImage != null)
        {
            previewImage.color = color;
        }
        
        OnColorChanged(color);
    }

    public void SetColor(Color color)
    {
        Color.RGBToHSV(color, out _hue, out _saturation, out _value);
        UpdateCursors();
        UpdateColor();
    }

    public void SetColorWithoutNotify(Color color)
    {
        Color.RGBToHSV(color, out _hue, out _saturation, out _value);

        if (_svMaterial != null)
        {
            _svMaterial.SetFloat(Hue, _hue);
        }

        if (previewImage != null)
        {
            previewImage.color = color;
        }

        UpdateCursors();
    }

    protected override void SetInteractable(bool isInteractable)
    {
        base.SetInteractable(isInteractable);
        hueBar.raycastTarget = isInteractable;
        svSquare.raycastTarget = isInteractable;
    }

    public override void UpdateFromOption()
    {
        SetColorWithoutNotify(Option?.Value ?? Color.white);
    }
}