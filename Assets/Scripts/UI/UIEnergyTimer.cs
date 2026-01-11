using System;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIEnergyTimer : MonoBehaviour
{
    public static UIEnergyTimer Instance;

    public float refillTimePerEnergyUnit = 300f; // 5 минут

    [SerializeField] private RectTransform _EnergyBar;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private TextMeshProUGUI energyWhite;
    [SerializeField] private TextMeshProUGUI energyBlack;
    [SerializeField] private Image energyLine;
    [SerializeField] private GameObject popup;

    private float _currentEnergy = 10f;
    private float _lastSaveTime;
    private bool _isTimerActive = false;
    private readonly float _hidePos = 70f;
    private readonly float _showPos = -70f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadEnergy();
        UpdateUI();
        StartTimerIfNeeded();

        if (SceneManager.GetActiveScene().name == "MenuScene") AnimateShow();
    }

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "GameplayScene") AnimateShow();
    }

    private void Update()
    {
        if (_isTimerActive)
        {
            float timePassed = Time.time - _lastSaveTime;
            float energyRecovered = timePassed / refillTimePerEnergyUnit;

            if (energyRecovered >= 1f)
            {
                _currentEnergy++;
                _lastSaveTime = Time.time;
                SaveEnergy();

                if (_currentEnergy >= 10f)
                {
                    _currentEnergy = 10f;
                    StopTimer();
                }

                UpdateUI();
            }
            else UpdateTimerText((int)(refillTimePerEnergyUnit - timePassed));
        }
    }

    private void LoadEnergy()
    {
        _currentEnergy = PlayerPrefs.GetFloat("Energy", 10f);
        _lastSaveTime = PlayerPrefs.GetFloat("LastSaveTime", Time.time);

        _currentEnergy = Mathf.Clamp(_currentEnergy, 0f, 10f);

        float offlineTime = Time.time - _lastSaveTime;
        int energyToAdd = Mathf.FloorToInt(offlineTime / refillTimePerEnergyUnit);

        _currentEnergy = Mathf.Min(10f, _currentEnergy + energyToAdd);
        _lastSaveTime = Time.time;

        SaveEnergy();
    }


    private void SaveEnergy()   // Сохранение энергии и времени последнего сохранения
    {
        PlayerPrefs.SetFloat("Energy", _currentEnergy);
        PlayerPrefs.SetFloat("LastSaveTime", _lastSaveTime);
        PlayerPrefs.Save();
    }

    private void UpdateUI() // Обновление UI и таймера
    {
        int energyInt = Mathf.FloorToInt(_currentEnergy);
        energyWhite.text = energyInt.ToString();
        energyBlack.text = energyInt.ToString();
        energyLine.fillAmount = _currentEnergy / 10f;

        StartTimerIfNeeded();
    }

    private void UpdateTimerText(int secondsLeft)   // Обновление таймера
    {
        TimeSpan time = TimeSpan.FromSeconds(secondsLeft);
        timer.text = time.ToString("mm\\:ss");
    }

    private void StartTimerIfNeeded()
    {
        if (_currentEnergy < 10f)
        {
            timer.gameObject.SetActive(true);
            _isTimerActive = true;

            float timePassedSinceLastRefill = Time.time - _lastSaveTime;
            float nextRefillIn = refillTimePerEnergyUnit - timePassedSinceLastRefill;

            if (nextRefillIn > 0) UpdateTimerText(Mathf.CeilToInt(nextRefillIn));
        }
        else StopTimer();
    }


    private void StopTimer()    // Сброс таймера
    {
        _isTimerActive = false;
        timer.text = "00:00";
        timer.gameObject.SetActive(false);
    }

    public bool CheckEnergy(int amount) // Проверка наличия энергии
    {
        if (_currentEnergy >= amount)
        {
            _currentEnergy -= amount;
            SaveEnergy();
            UpdateUI();
            return true;
        }
        else
        {
            ShowNotEnoughEnergyPopup();
            return false;
        }
    }

    public void SpendEnergy() => CheckEnergy(5);

    public void AddEnergy(int amount)   // Пополняет энергию на указанное количество.
    {
        _currentEnergy = Mathf.Min(10f, _currentEnergy + amount);
        SaveEnergy();
        UpdateUI();
        AnimScaleBar();
    }

    private void ShowNotEnoughEnergyPopup() // Показывает поп-ап при нехватке энергии.
    {
        popup.SetActive(true);
        UpdateUI();
        _EnergyBar.DOKill();
        _EnergyBar.localScale = Vector3.one;
        AnimScaleBar();
    }

    private void AnimScaleBar()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_EnergyBar.DOScale(1.4f, 0.3f).SetEase(Ease.OutQuad))
                .Append(_EnergyBar.DOScale(1f, 0.2f).SetEase(Ease.OutBack))
                .SetLoops(3);
    }

    public void AnimateShow()   // Анимация появления
    {
        _EnergyBar.DOKill();
        _EnergyBar.gameObject.SetActive(true);
        _EnergyBar.DOAnchorPosY(_showPos, 0.5f).SetEase(Ease.OutQuad);
    }

    public void AnimateHide()   // Анимация скрытия
    {
        _EnergyBar.DOKill();
        _EnergyBar.DOAnchorPosY(_hidePos, 0.5f).SetEase(Ease.InQuad)
                  .OnComplete(() => _EnergyBar.gameObject.SetActive(false));
    }

    private void OnApplicationQuit() => SaveEnergy();    // Сохранение при выход

    private void OnApplicationPause(bool pauseStatus)   // Игра уходит в фон
    { if (pauseStatus) SaveEnergy(); }
}