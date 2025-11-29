using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIAnimationMenu : MonoBehaviour
{
    public static Vector2 startSize = new(3000, 4000);
    [SerializeField] private RectTransform rectTransformMenu; // Основное окно
    [SerializeField] private Vector2 finalSize = new(3500, 4000); // Конечный размер
    [SerializeField] private float scaleDuration = 1.0f; // Время растягивания

    private void Start()
    {
        rectTransformMenu.sizeDelta = Vector2.zero; // Начальный размер 0
        ShowMenu();
    }

    public void ShowMenu()
    {
        rectTransformMenu.DOSizeDelta(finalSize, scaleDuration).SetEase(Ease.OutBounce); // Плавное расширение с отскоком
    }

    public void HideMenu()
    {
        rectTransformMenu.DOSizeDelta(Vector2.zero, scaleDuration).SetEase(Ease.OutBounce); // Плавное уменьшение с отскоком
    }
}