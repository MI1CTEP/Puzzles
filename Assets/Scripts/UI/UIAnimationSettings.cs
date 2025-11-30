using UnityEngine;
using DG.Tweening;

public class UIAnimationSettings : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransformMenu; // Основное окно
    [SerializeField] private GameObject settings; // Панель настроек
    [SerializeField] private Vector2 finalSize = new(3000, 3000); // Конечный размер панели
    [SerializeField] private float expandDuration = 0.6f;   // Длительность анимации расширения

    public void SettingsOpen()    // Открытие настроек
    {
        rectTransformMenu.DOSizeDelta(finalSize, expandDuration).SetEase(Ease.OutBounce).OnComplete(() => { settings.SetActive(true); });
    }

    public void SettingsClose()  // Закрытие настроек
    {
        settings.SetActive(false);
        rectTransformMenu.DOSizeDelta(UIAnimationMenu.startSize, expandDuration * 0.8f).SetEase(Ease.OutBounce);
    }
}