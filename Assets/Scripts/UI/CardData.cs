using UnityEngine;
using System;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "CardData", menuName = "Cards/Card Data")]
[Serializable]
public class CardData : ScriptableObject
{
    public string characterName;
    [TextArea(5, 10)] public string story;

    [Range(-10, 10)] public int sympathy; // Симпатия: -10 до +10
    [Range(0, 100)] public int progress;  // Общий прогресс

    [Header("Изображения (5)")]
    public UnlockableSprite[] images = new UnlockableSprite[5];

    [Header("Видео (4)")]
    public UnlockableVideo[] videos = new UnlockableVideo[4];

    // Получить общий прогресс открытия
    public float GetUnlockProgress()
    {
        int total = images.Length + videos.Length;
        int unlocked = 0;
        foreach (var img in images) if (img.isUnlocked) unlocked++;
        foreach (var vid in videos) if (vid.isUnlocked) unlocked++;
        return total > 0 ? (float)unlocked / total : 0;
    }
}

[Serializable]
public class UnlockableSprite
{
    public Sprite sprite;
    public bool isUnlocked = false;
}

[Serializable]
public class UnlockableVideo
{
    public VideoClip clip;
    public bool isUnlocked = false;
}
