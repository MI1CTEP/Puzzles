using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class UILanguage : MonoBehaviour
{
    public static event System.Action OnLanguageChanged;

    [SerializeField] private Image _languageImage;
    [Space]
    [SerializeField] private Sprite _russian;
    [SerializeField] private Sprite _english;
    [SerializeField] private Sprite _german;
    [SerializeField] private Sprite _chinese;
    [SerializeField] private Sprite _japanese;
    [SerializeField] private Sprite _french;
    [SerializeField] private Sprite _portuguese;
    [SerializeField] private Sprite _italian;
    [SerializeField] private Sprite _hindi;

    private readonly System.Collections.Generic.Dictionary<string, Sprite> _languageSprites = new();

    private readonly string[] _languages = {
        "Russian",
        "English",
        "German",
        "Chinese",
        "Japanese",
        "French",
        "Portuguese",
        "Italian",
        "Hindi"
    };

    private void Awake()
    {
        _languageSprites["Russian"] = _russian;
        _languageSprites["English"] = _english;
        _languageSprites["German"] = _german;
        _languageSprites["Chinese"] = _chinese;
        _languageSprites["Japanese"] = _japanese;
        _languageSprites["French"] = _french;
        _languageSprites["Portuguese"] = _portuguese;
        _languageSprites["Italian"] = _italian;
        _languageSprites["Hindi"] = _hindi;
    }

    private void Start() => LoadSettings();

    private void LoadSettings()
    {
        string savedLanguage = PlayerPrefs.GetString("language", "");

        // Если язык сохранён и поддерживается
        if (!string.IsNullOrEmpty(savedLanguage) && _languageSprites.ContainsKey(savedLanguage))
        { ApplyLanguage(savedLanguage); }
        else
        {
            // Или определяем по спрайту (обратная совместимость)
            string fallbackLanguage = DetectLanguageFromSprite();
            ApplyLanguage(fallbackLanguage);
        }
    }

    private string DetectLanguageFromSprite()
    {
        if (_languageImage.sprite == _russian) return "Russian";
        if (_languageImage.sprite == _english) return "English";
        if (_languageImage.sprite == _german) return "German";
        if (_languageImage.sprite == _chinese) return "Chinese";
        if (_languageImage.sprite == _japanese) return "Japanese";
        if (_languageImage.sprite == _french) return "French";
        if (_languageImage.sprite == _portuguese) return "Portuguese";
        if (_languageImage.sprite == _italian) return "Italian";
        if (_languageImage.sprite == _hindi) return "Hindi";

        return "Russian";
    }

    public void ChangeLanguage()
    {
        string current = PlayerPrefs.GetString("language", "Russian");
        int currentIndex = System.Array.IndexOf(_languages, current);
        if (currentIndex == -1) currentIndex = 0;

        int nextIndex = (currentIndex + 1) % _languages.Length;
        string nextLanguage = _languages[nextIndex];

        ApplyLanguage(nextLanguage);
    }

    private void ApplyLanguage(string language)
    {
        if (!_languageSprites.TryGetValue(language, out Sprite sprite))
        {
            Debug.LogWarning($"[UILanguage] Спрайт для языка '{language}' не найден.");
            return;
        }

        _languageImage.sprite = sprite;
        PlayerPrefs.SetString("language", language);
        PlayerPrefs.Save();

        LocalizationManager.CurrentLanguage = language;

        OnLanguageChanged?.Invoke();
    }
}