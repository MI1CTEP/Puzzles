using UnityEngine;
using System;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "CardData", menuName = "Cards/Card Data")]

[Serializable] public class CardData : ScriptableObject
{
    public string characterName;    // Имя персонажа
    [TextArea(5, 10)] public string story;  // Сюжет
    [Range(0, 100)] public int sympathy; // Симпатия: 0 до 100
    [Range(0, 100)] public int progress;  // Общий прогресс
    [Header("Изображения (5)")]
    public UnlockableSprite[] images = new UnlockableSprite[5]; // Изображения
    [Header("Видео (4)")]
    public UnlockableVideo[] videos = new UnlockableVideo[4];   // Видео

    public float GetUnlockProgress()    // Получить общий прогресс открытия
    {
        int total = images.Length + videos.Length;
        int unlocked = 0;
        foreach (var img in images) if (img.isUnlocked) unlocked++;
        foreach (var vid in videos) if (vid.isUnlocked) unlocked++;
        return total > 0 ? (float)unlocked / total : 0;
    }
}

[Serializable] public class UnlockableSprite    // Спрайт с флагом открытия
{
    public Sprite sprite;
    public bool isUnlocked = false;
}

[Serializable] public class UnlockableVideo     // Видео с флагом открытия
{
    public VideoClip clip;
    public bool isUnlocked = false;
}