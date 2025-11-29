using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;

public class CardItem : MonoBehaviour
{
    [Header("UI Front")]
    [SerializeField] private Text nameText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Text sympathyText;
    [SerializeField] private Button revealButton;

    [Header("UI Back")]
    [SerializeField] private Text storyText;
    [SerializeField] private Image mediaImage;
    [SerializeField] private RawImage rawImage; // Для VideoPlayer
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [Header("Настройки")]
    [SerializeField] private GameObject frontPanel;
    [SerializeField] private GameObject backPanel;
    [SerializeField] private float flipDuration = 0.6f;

    private CardData _data;
    private int _currentMediaIndex = 0;
    private Action<CardItem> _onRevealed;
    private Canvas _canvas; // Чтобы отключать при видео

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
    }

    public void SetData(CardData data, Action<CardItem> onRevealed)
    {
        _data = data;
        _onRevealed = onRevealed;
        _currentMediaIndex = 0;

        nameText.text = data.characterName;
        sympathyText.text = $"Симпатия: {data.sympathy}";
        storyText.text = data.story;

        // Обновляем портрет — первое открытое изображение
        UpdatePortrait();

        // Обновляем прогресс (если используется)
        progressSlider.value = data.GetUnlockProgress();

        // Кнопка "раскрыть" — только если карточка целиком закрыта
        revealButton.gameObject.SetActive(!IsAnyMediaUnlocked());

        UpdateMedia();
        ShowFront();
    }

    private void UpdatePortrait()
    {
        foreach (var img in _data.images)
        {
            if (img.isUnlocked && img.sprite != null)
            {
                portraitImage.sprite = img.sprite;
                return;
            }
        }
        // Если ничего не открыто — ставим placeholder
        portraitImage.sprite = null; // или заглушка
    }

    private bool IsAnyMediaUnlocked()
    {
        foreach (var img in _data.images) if (img.isUnlocked) return true;
        foreach (var vid in _data.videos) if (vid.isUnlocked) return true;
        return false;
    }


    public void OnRevealButton()
    {
        _onRevealed?.Invoke(this);
        PlayFlipAnimation();
    }

    private void PlayFlipAnimation()
    {
        transform.DORotate(new Vector3(0, 90, 0), flipDuration * 0.5f)
            .OnComplete(() =>
            {
                frontPanel.SetActive(false);
                backPanel.SetActive(true);
                transform.DORotate(Vector3.zero, flipDuration * 0.5f);
            });
    }

    public void ShowFront()
    {
        frontPanel.SetActive(true);
        backPanel.SetActive(false);
        transform.rotation = Quaternion.identity;
        StopCurrentMedia();
    }

    private void UpdateMedia()
    {
        StopCurrentMedia();

        // Собираем все открытые медиа
        var unlockedMedia = new System.Collections.Generic.List<(bool isImage, int index)>();

        for (int i = 0; i < _data.images.Length; i++)
            if (_data.images[i].isUnlocked) unlockedMedia.Add((true, i));

        for (int i = 0; i < _data.videos.Length; i++)
            if (_data.videos[i].isUnlocked) unlockedMedia.Add((false, i));

        if (unlockedMedia.Count == 0)
        {
            mediaImage.gameObject.SetActive(true);
            rawImage.gameObject.SetActive(false);
            mediaImage.sprite = null;
            leftButton.interactable = false;
            rightButton.interactable = false;
            return;
        }

        // Ограничиваем индекс
        _currentMediaIndex = Mathf.Clamp(_currentMediaIndex, 0, unlockedMedia.Count - 1);
        var (isImage, index) = unlockedMedia[_currentMediaIndex];

        if (isImage)
        {
            // Показываем изображение
            mediaImage.gameObject.SetActive(true);
            rawImage.gameObject.SetActive(false);
            videoPlayer.Stop();
            mediaImage.sprite = _data.images[index].sprite;
        }
        else
        {
            // Показываем видео
            mediaImage.gameObject.SetActive(false);
            rawImage.gameObject.SetActive(true);
            videoPlayer.clip = _data.videos[index].clip;
            videoPlayer.Play();
        }

        // Активность кнопок
        leftButton.interactable = _currentMediaIndex > 0;
        rightButton.interactable = _currentMediaIndex < unlockedMedia.Count - 1;
    }


    private void StopCurrentMedia()
    {
        if (videoPlayer.isPlaying) videoPlayer.Stop();
        rawImage.gameObject.SetActive(false);
        mediaImage.gameObject.SetActive(true);
    }

    public void OnLeft()
    {
        // Пересобираем открытые медиа
        var unlockedMedia = GetUnlockedMedia();
        if (_currentMediaIndex <= 0 || unlockedMedia.Count == 0) return;
        _currentMediaIndex--;
        UpdateMedia();
    }

    public void OnRight()
    {
        var unlockedMedia = GetUnlockedMedia();
        if (_currentMediaIndex >= unlockedMedia.Count - 1 || unlockedMedia.Count == 0) return;
        _currentMediaIndex++;
        UpdateMedia();
    }

    private System.Collections.Generic.List<(bool isImage, int index)> GetUnlockedMedia()
    {
        var list = new System.Collections.Generic.List<(bool, int)>();
        for (int i = 0; i < _data.images.Length; i++)
            if (_data.images[i].isUnlocked) list.Add((true, i));
        for (int i = 0; i < _data.videos.Length; i++)
            if (_data.videos[i].isUnlocked) list.Add((false, i));
        return list;
    }
}