using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UIAudioSettings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;   // Слайдер громкости (0–10)
    [SerializeField] private AudioMixer audioMixer; // Ссылка на Audio Mixer
    [Space]
    [SerializeField] private GameObject[] waves;    // Визуальные "волны" (UI Images)

    private readonly string mixerParameter = "MasterVolume"; // Параметр в Audio Mixer
    private readonly string volumeKey = "AudioVolume"; // Ключ в PlayerPrefs

    private void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat(volumeKey, 10f);   // Загружаем сохранённое значение

        UpdateVolume(volumeSlider.value);  // Применяем громкость (микшер + волны)

        volumeSlider.onValueChanged.AddListener(UpdateVolume);  // Подписываемся на изменение слайдера
    }

    public void Volume() => UpdateVolume(volumeSlider.value);   // Обновление громкости

    private void UpdateVolume(float value)  // Обновление громкости и волны
    {
        int intValue = Mathf.RoundToInt(value); // Округляем до 0–10

        PlayerPrefs.SetFloat(volumeKey, intValue);
        PlayerPrefs.Save(); // Сохраняем в PlayerPrefs

        SetMixerVolume(intValue);   // Обновляем громкость в AudioMixer
        UpdateEffectObjects(intValue);  // Обновляем волны звука
    }

    private void SetMixerVolume(int volumeLevel)    // Установка громкости в Audio Mixer
    {
        float db = volumeLevel switch
        {
            0 => -80f,
            1 => -40f,
            2 => -35f,
            3 => -30f,
            4 => -25f,
            5 => -20f,
            6 => -15f,
            7 => -10f,
            8 => -6f,
            9 => -3f,
            10 => 0f,
            _ => -80f
        };

        audioMixer.SetFloat(mixerParameter, db);
    }

    private void UpdateEffectObjects(int volumeLevel)   // Обновление визуальных эффектов
    {
        foreach (var wave in waves) wave.SetActive(false);

        if (volumeLevel <= 0) return;
        if (volumeLevel >= 1) waves[0].SetActive(true);
        if (volumeLevel >= 5) waves[1].SetActive(true);
        if (volumeLevel >= 10) waves[2].SetActive(true);
    }
}