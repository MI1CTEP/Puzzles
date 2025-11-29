using UnityEngine;
using DG.Tweening;

public class UIAnimationGallery : MonoBehaviour
{

    [SerializeField] private RectTransform rectTransformMenu; // Основное окно
    [SerializeField] private GameObject gallary; // Панель галереи
    [SerializeField] private Vector2 finalSize = new(4000, 6000); // Конечный размер панели
    [SerializeField] private float expandDuration = 0.6f;   // Длительность анимации расширения

    [SerializeField] private UIVerticalItemScroller uIVerticalItemScroller;

    public void GalleryOpen()
    {
        rectTransformMenu.DOSizeDelta(finalSize, expandDuration).SetEase(Ease.OutBounce)
        .OnComplete(() =>
        {
            gallary.SetActive(true);
            uIVerticalItemScroller.Open();
        }); ;  // Расширяем панель
    }

    public void GalleryClose()
    {
        gallary.SetActive(false);
        rectTransformMenu.DOSizeDelta(UIAnimationMenu.startSize, expandDuration * 0.8f).SetEase(Ease.OutBounce)
        .OnComplete(() => { uIVerticalItemScroller.Close(); });  // Сжимаем панель
    }
}