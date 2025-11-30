using UnityEngine;
using DG.Tweening;

public class UIAnimationGallery : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransformMenu; // Основное окно
    [SerializeField] private GameObject gallary; // Панель галереи
    [SerializeField] private Vector2 finalSize = new(4000, 6000); // Конечный размер панели
    [SerializeField] private float expandDuration = 0.6f;   // Длительность анимации расширения

    public void GalleryOpen()    // Открытие галереи
    {
        rectTransformMenu.DOSizeDelta(finalSize, expandDuration).SetEase(Ease.OutBounce)
        .OnComplete(() => { gallary.SetActive(true); }); ;  // Расширяем панель
    }

    public void GalleryClose()  // Закрытие галереи
    {
        gallary.SetActive(false);
        rectTransformMenu.DOSizeDelta(UIAnimationMenu.startSize, expandDuration * 0.8f).SetEase(Ease.OutBounce);  // Сжимаем панель
    }
}