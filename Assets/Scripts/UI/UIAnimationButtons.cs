using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

public class UIAnimationButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [System.Serializable]
    public class ButtonGroup
    {
        public List<Button> buttons = new();
        private Dictionary<Button, Vector3> originalScales;

        public void CacheOriginalScales()
        {
            originalScales = new Dictionary<Button, Vector3>();
            foreach (var button in buttons)
            { if (button != null) originalScales[button] = button.transform.localScale; }
        }

        public Vector3 GetOriginalScale(Button button) => originalScales != null && originalScales.TryGetValue(button, out var scale) ? scale : Vector3.one;
    }

    [Header("Настройки анимации")]
    [SerializeField] private float hoverScaleFactor = 1.1f; // 1.1 = +10% при наведении
    [SerializeField] private float pressScale = 0.7f;
    [SerializeField] private float popUpScale = 1.2f;
    [SerializeField] private float hoverDuration = 0.2f;
    [SerializeField] private float pressDuration = 0.1f;
    [SerializeField] private float popUpDuration = 0.3f;
    [SerializeField] private float shrinkDuration = 0.3f;

    [Header("Группы кнопок")]
    [SerializeField] private ButtonGroup menuGroup;
    [SerializeField] private ButtonGroup galleryGroup;
    [SerializeField] private ButtonGroup settingsGroup;

    [Space]
    [SerializeField] private CanvasGroup _canvasGroup;

    private bool _isBlocked = false;
    private List<ButtonGroup> allGroups;

    private void Start()
    {
        allGroups = new List<ButtonGroup> { menuGroup, galleryGroup, settingsGroup };

        foreach (var group in allGroups)    // Сохраняем оригинальные масштабы
        {
            if (group != null)
            {
                group.CacheOriginalScales();
                foreach (var button in group.buttons)
                {
                    AddEventListeners(button);
                    button.gameObject.SetActive(false);
                }
            }
        }
        ShowButtonsMenu();
    }

    private void AddEventListeners(Button button)
    {
        if (!button.gameObject.TryGetComponent<EventTrigger>(out var trigger)) trigger = button.gameObject.AddComponent<EventTrigger>();

        trigger.triggers.Clear();

        AddTrigger(trigger, EventTriggerType.PointerEnter, _ => OnButtonEnter(button));
        AddTrigger(trigger, EventTriggerType.PointerExit, _ => OnButtonExit(button));
        AddTrigger(trigger, EventTriggerType.PointerDown, _ => OnButtonDown(button));
        AddTrigger(trigger, EventTriggerType.PointerUp, _ => OnButtonUp(button));
    }

    private void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new();
        entry.eventID = type;
        entry.callback.AddListener(data => callback.Invoke(data));
        trigger.triggers.Add(entry);
    }

    private void OnButtonEnter(Button button)
    {
        if (_isBlocked) return;

        button.transform.DOKill();
        Vector3 targetScale = GetOriginalScale(button) * hoverScaleFactor;
        button.transform.DOScale(targetScale, hoverDuration).SetEase(Ease.OutSine);
    }

    private void OnButtonExit(Button button)
    {
        if (_isBlocked) return;

        button.transform.DOKill();
        button.transform.DOScale(GetOriginalScale(button), hoverDuration).SetEase(Ease.OutSine);
    }

    private void OnButtonDown(Button button)
    {
        if (_isBlocked) return;

        button.transform.DOKill();
        Vector3 targetScale = GetOriginalScale(button) * pressScale;
        button.transform.DOScale(targetScale, pressDuration).SetEase(Ease.OutQuad);
    }

    private void OnButtonUp(Button button)
    {
        if (_isBlocked) return;
        _isBlocked = true;
        _canvasGroup.blocksRaycasts = false;

        button.transform.DOKill();
        Vector3 original = GetOriginalScale(button);
        button.transform.DOScale(original * popUpScale, popUpDuration / 2f);
    }

    private Vector3 GetOriginalScale(Button button)
    {
        if (menuGroup.buttons.Contains(button)) return menuGroup.GetOriginalScale(button);
        if (galleryGroup.buttons.Contains(button)) return galleryGroup.GetOriginalScale(button);
        if (settingsGroup.buttons.Contains(button)) return settingsGroup.GetOriginalScale(button);
        return Vector3.one;
    }

    // === Методы Show/Hide для каждой группы ===

    public void ShowButtonsMenu() => ShowGroup(menuGroup);
    public void HideButtonsMenu() => HideGroup(menuGroup);
    public void ShowButtonsGallery() => ShowGroup(galleryGroup);
    public void HideButtonsGallery() => HideGroup(galleryGroup);
    public void ShowButtonsSettings() => ShowGroup(settingsGroup);
    public void HideButtonsSettings() => HideGroup(settingsGroup);


    private async void ShowGroup(ButtonGroup group)
    {
        await Task.Delay(1000);
        if (group == null) return;
        foreach (var button in group.buttons)
        {
            if (button == null) continue;
            button.gameObject.SetActive(true);
            button.transform.localScale = Vector3.zero;
            button.transform.DOScale(GetOriginalScale(button), shrinkDuration * 1.5f).SetEase(Ease.OutBack);
        }
    }

    private void HideGroup(ButtonGroup group)
    {
        if (group == null) return;
        foreach (var button in group.buttons)
        {
            if (button == null) continue;
            button.transform.DOKill();
            button.transform.DOScale(0, shrinkDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    _isBlocked = false;
                    _canvasGroup.blocksRaycasts = true;
                    button.gameObject.SetActive(false);
                });
        }
    }

    // Для интерфейса
    public void OnPointerEnter(PointerEventData eventData) { }
    public void OnPointerExit(PointerEventData eventData) { }
}