using UnityEngine;
using DG.Tweening;

public class UIAnimationMenu : MonoBehaviour
{
    public static Vector2 startSize = new(3000, 4000);  // Начальный размер
    [SerializeField] private RectTransform rectTransformMenu; // Основное окно
    [SerializeField] private Vector2 finalSize = new(3500, 4000); // Конечный размер
    [SerializeField] private float scaleDuration = 1.0f; // Время растягивания

    void Awake() => rectTransformMenu.sizeDelta = Vector2.zero;

    private void Start() => ShowMenu();

    public void ShowMenu() => rectTransformMenu.DOSizeDelta(finalSize, scaleDuration).SetEase(Ease.OutBounce);    // Показать меню

    public void HideMenu() => rectTransformMenu.DOSizeDelta(Vector2.zero, scaleDuration).SetEase(Ease.OutBounce); // Скрыть меню
}