using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

namespace MyGame.Menu
{
    public sealed class UIHearts : MonoBehaviour
    {
        [Space]
        [SerializeField] private RectTransform _canvas;           // Канвас для размеров
        [Header("Hearts Settings")]
        [SerializeField] private GameObject _heartPrefab;          // Префаб сердечка
        [SerializeField] private List<Sprite> _heartSprites = new();  // Спрайты сердечек
        [Header("Animation Settings")]
        [SerializeField] private float _spawnInterval = 1f;        // Интервал появления
        [SerializeField] private float _fadeInDuration = 0.5f;     // Время появления
        [SerializeField] private float _fadeOutDuration = 0.5f;    // Время исчезновения
        [SerializeField] private float _visibleTime = 2f;          // Сколько сердце "стоит" на месте
        [SerializeField] private float _minScale = 0.5f;           // Минимальный размер
        [SerializeField] private float _maxScale = 1.2f;           // Максимальный размер

        private bool _isSpawning = true;

        private void Start() => StartCoroutine(SpawnRoutine());

        private IEnumerator SpawnRoutine()  // Цикл появления сердечек
        {
            for (int i = 0; i < _heartSprites.Count; i++)
            {
                SpawnHeart();
                yield return new WaitForSeconds(_spawnInterval);
            }
        }

        private void SpawnHeart()   // Создание сердечка с анимацией
        {
            GameObject heartGO = Instantiate(_heartPrefab, transform);
            RectTransform rectTransform = heartGO.GetComponent<RectTransform>();
            Image image = heartGO.GetComponent<Image>();

            rectTransform.anchoredPosition = GetRandomPosition();   // Случайная позиция
            if (_heartSprites.Count > 0) image.sprite = _heartSprites[Random.Range(0, _heartSprites.Count)];    // Случайный спрайт

            StartCoroutine(HeartCycle(heartGO));    // Цикл анимации
        }

        private IEnumerator HeartCycle(GameObject heart)    // Цикл анимации сердечка
        {
            Image image = heart.GetComponent<Image>();
            RectTransform rectTransform = heart.GetComponent<RectTransform>();
            Transform transform = heart.transform;

            while (_isSpawning)
            {
                // Увеличение масштаба и прозрачности
                image.DOFade(1f, _fadeInDuration).SetUpdate(true);
                transform.DOScale(_maxScale, _fadeInDuration).SetEase(Ease.OutBack);
                yield return new WaitForSeconds(_visibleTime);

                // Уменьшение масштаба и прозрачности
                image.DOFade(0f, _fadeOutDuration).SetUpdate(true);
                transform.DOScale(_minScale, _fadeOutDuration).SetEase(Ease.InBack);
                yield return new WaitForSeconds(_fadeOutDuration);

                rectTransform.anchoredPosition = GetRandomPosition();   // Перемещение в новую позицию
            }
        }

        private Vector2 GetRandomPosition() // Случайная позиция сердечка
        {
            Vector2 size = _canvas.sizeDelta;
            float x = Random.Range(-size.x / 2 + 50f, size.x / 2 - 50f);
            float y = Random.Range(-size.y / 2 + 50f, size.y / 2 - 50f);
            return new Vector2(x, y);
        }

        public void StopSpawning() => _isSpawning = false;   // Остановка цикла появления
    }
}