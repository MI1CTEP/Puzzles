using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using DG.Tweening;

public class CardItem : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject frontPanel;   // Лицо карточки
    public GameObject backPanel;    // Обратная сторона

    [Header("UI Front")]
    [SerializeField] private TextMeshProUGUI nameText;      // Имя персонажа
    [SerializeField] private Image portraitImage;           // Портрет

    [Header("UI Back")]
    [SerializeField] private TextMeshProUGUI storyText;     // Сюжет
    [SerializeField] private Image mediaImage;              // Изображение
    [SerializeField] private RawImage rawImage;             // Для VideoPlayer
    [SerializeField] private VideoPlayer videoPlayer;       // Видео

    private CardData _data;      // Данные карточки
    private int _currentMediaIndex = 0;     // Текущий индекс медиа

    public void SetData(CardData data) // Обновляем данные карточки
    {
        _data = data;
        _currentMediaIndex = 0;
        nameText.text = data.characterName;
        storyText.text = data.story;

        UpdateMedia();
        ShowFront();
    }

    public CardData GetData() => _data;  // Получить данные карточки

    public int GetCurrentMediaIndex() => _currentMediaIndex;    // Получить индекс текущего медиа

    public void OnLeft() => ChangeMedia(-1);    // Влево

    public void OnRight() => ChangeMedia(1);    // Вправо

    private void ChangeMedia(int delta) // Изменение медиа (влево/вправо)
    {
        var unlockedMedia = GetUnlockedMedia();
        int newIndex = _currentMediaIndex + delta;

        if (newIndex < 0 || newIndex >= unlockedMedia.Count) return;

        _currentMediaIndex = newIndex;
        UpdateMedia();
    }

    public void ShowFront() // Показываем лицо карточки
    {
        frontPanel.SetActive(true);
        backPanel.SetActive(false);
        transform.rotation = Quaternion.identity;
        StopCurrentMedia();
    }

    private void UpdateMedia()  // Обновляем медиа (изображение или видео)
    {
        StopCurrentMedia();

        var unlockedMedia = new System.Collections.Generic.List<(bool isImage, int index)>();   // Собираем все открытые медиа

        for (int i = 0; i < _data.images.Length; i++) if (_data.images[i].isUnlocked) unlockedMedia.Add((true, i));
        for (int i = 0; i < _data.videos.Length; i++) if (_data.videos[i].isUnlocked) unlockedMedia.Add((false, i));

        if (unlockedMedia.Count == 0)
        {
            mediaImage.gameObject.SetActive(true);
            rawImage.gameObject.SetActive(false);
            mediaImage.sprite = null;
            return;
        }

        _currentMediaIndex = Mathf.Clamp(_currentMediaIndex, 0, unlockedMedia.Count - 1);   // Ограничиваем индекс
        var (isImage, index) = unlockedMedia[_currentMediaIndex];

        if (isImage)    // Показываем изображение
        {
            mediaImage.gameObject.SetActive(true);
            rawImage.gameObject.SetActive(false);
            videoPlayer.Stop();
            mediaImage.sprite = _data.images[index].sprite;
        }
        else           // Показываем видео
        {
            mediaImage.gameObject.SetActive(false);
            rawImage.gameObject.SetActive(true);
            videoPlayer.clip = _data.videos[index].clip;
            videoPlayer.Play();
        }

        if (CardUIManager.Instance != null) CardUIManager.Instance.OnCardMediaChanged();
    }

    private void StopCurrentMedia() // Останавливаем текущее медиа
    {
        if (videoPlayer.isPlaying) videoPlayer.Stop();
        rawImage.gameObject.SetActive(false);
        mediaImage.gameObject.SetActive(true);
    }

    private System.Collections.Generic.List<(bool isImage, int index)> GetUnlockedMedia()   // Получить список открытых медиа
    {
        var list = new System.Collections.Generic.List<(bool, int)>();
        for (int i = 0; i < _data.images.Length; i++) if (_data.images[i].isUnlocked) list.Add((true, i));
        for (int i = 0; i < _data.videos.Length; i++) if (_data.videos[i].isUnlocked) list.Add((false, i));
        return list;
    }

    private bool IsRevealing = false;           // Флаг анимации переворота

    public void FlipCard()      // Перевернуть карточку
    {
        if (IsRevealing) return;
        StartCoroutine(AnimateFlip());
    }

    private System.Collections.IEnumerator AnimateFlip()        // Анимация переворота
    {
        IsRevealing = true;
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
        IsRevealing = false;
    }
}