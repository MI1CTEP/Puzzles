using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class CardItem : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject frontPanel;   // Лицо карточки
    public GameObject backPanel;    // Обратная сторона

    [Header("UI Front")]
    [SerializeField] private TextMeshProUGUI nameText;      // Имя персонажа
    [SerializeField] private Image portraitImage;           // Портрет
    [SerializeField] private GameObject blur;               // Блюр
    [SerializeField] private Transform block;               // Блок

    [Header("UI Back")]
    [SerializeField] private TextMeshProUGUI storyText;     // Сюжет
    [SerializeField] private Image mediaImage;              // Изображение
    [SerializeField] private RawImage rawImage;             // Для VideoPlayer
    [SerializeField] private VideoPlayer videoPlayer;       // Видео

    private CardData _data;                 // Данные карточки
    private int _currentMediaIndex = 0;     // Текущий индекс медиа
    private bool _isRevealing = false;      // Флаг анимации переворота
    private Tweener _lockTweener;           // Для анимации покачивания замка

    public void SetData(CardData data) // Обновляем данные карточки
    {
        _data = data;
        _currentMediaIndex = 0;
        nameText.text = data.characterName;
        storyText.text = data.story;

        UpdateMedia();
        UpdateLockState();
        ShowFront();
    }

    public CardData GetData() => _data;  // Получить данные карточки

    public int GetCurrentMediaIndex() => _currentMediaIndex;    // Получить индекс текущего медиа

    public void OnLeft() => ChangeMedia(-1);    // Влево

    public void OnRight() => ChangeMedia(1);    // Вправо

    private void ChangeMedia(int delta) // Изменение медиа (влево/вправо)
    {
        List<(bool isImage, int index)> allMedia = GetAllMedia();
        int newIndex = _currentMediaIndex + delta;

        if (newIndex < 0 || newIndex >= allMedia.Count) return;

        _currentMediaIndex = newIndex;
        UpdateMedia();
    }

    public void ShowFront() // Показываем лицо карточки
    {
        frontPanel.SetActive(true);
        backPanel.SetActive(false);
        transform.rotation = Quaternion.identity;
        StopCurrentMedia();
        UpdateMedia();
        UpdateLockState();
    }

    private void UpdateMedia()  // Обновляем медиа (изображение или видео)
    {
        StopCurrentMedia();

        List<(bool isImage, int index)> allMedia = GetAllMedia();
        if (allMedia.Count == 0) return;

        _currentMediaIndex = Mathf.Clamp(_currentMediaIndex, 0, allMedia.Count - 1);
        var (isImage, index) = allMedia[_currentMediaIndex];

        bool isUnlocked = isImage ? _data.images[index].isUnlocked : _data.videos[index].isUnlocked;

        if (isImage)
        {
            mediaImage.gameObject.SetActive(true);
            rawImage.gameObject.SetActive(false);
            videoPlayer.Stop();
            mediaImage.sprite = _data.images[index].sprite;
        }
        else
        {
            mediaImage.gameObject.SetActive(false);
            rawImage.gameObject.SetActive(true);
            videoPlayer.clip = _data.videos[index].clip;
            if (isUnlocked) videoPlayer.Play();
        }

        blur.SetActive(!isUnlocked);

        if (CardUIManager.Instance != null) CardUIManager.Instance.OnCardMediaChanged();

        UpdateLockState();
    }

    private void StopCurrentMedia() // Останавливаем текущее медиа
    {
        if (videoPlayer.isPlaying) videoPlayer.Stop();
        rawImage.gameObject.SetActive(false);
        mediaImage.gameObject.SetActive(true);
    }

    private List<(bool isImage, int index)> GetAllMedia()   // Получить список открытых медиа
    {
        var list = new List<(bool, int)>();
        for (int i = 0; i < _data.images.Length; i++) list.Add((true, i));
        for (int i = 0; i < _data.videos.Length; i++) list.Add((false, i));
        return list;
    }

    private void UpdateLockState()
    {
        if (!blur.activeSelf) StopLockWiggle();
        else StartLockWiggle();
    }

    private void StartLockWiggle()
    {
        if (_lockTweener != null && _lockTweener.IsPlaying()) return;

        _lockTweener = block.DORotate(new Vector3(0, 0, 5), 0.8f)
                              .SetLoops(-1, LoopType.Yoyo)
                              .SetEase(Ease.InOutSine)
                              .SetUpdate(true);
    }

    private void StopLockWiggle()
    {
        _lockTweener?.Kill();
        _lockTweener = null;
        block.rotation = Quaternion.identity;
    }

    public void FlipCard()      // Перевернуть карточку
    {
        if (_isRevealing) return;
        StartCoroutine(AnimateFlip());
    }

    private IEnumerator AnimateFlip()        // Анимация переворота
    {
        _isRevealing = true;
        float duration = 0.3f;

        transform.DORotate(new Vector3(0, 90, 0), duration).SetUpdate(true);
        yield return new WaitForSeconds(duration);

        if (frontPanel.activeSelf)
        {
            frontPanel.SetActive(false);
            backPanel.SetActive(true);
            transform.DORotate(new Vector3(0, 180, 0), duration).SetUpdate(true);
        }
        else
        {
            frontPanel.SetActive(true);
            backPanel.SetActive(false);
            transform.DORotate(new Vector3(0, 0, 0), duration).SetUpdate(true);
        }

        yield return new WaitForSeconds(duration);
        _isRevealing = false;
        UpdateMedia();
        UpdateLockState();
    }

    private void OnDestroy() => StopLockWiggle();
}