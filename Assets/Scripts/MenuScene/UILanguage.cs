using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Menu
{
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
        [SerializeField] private Sprite _spanish;

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
        "Hindi",
        "Spanish"
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
            _languageSprites["Spanish"] = _spanish;
        }

        private void Start() => LoadSettings();

        private void LoadSettings() // Загрузка настроек языка из PlayerPrefs
        {
            string savedLanguage = PlayerPrefs.GetString("language", "");

            if (!string.IsNullOrEmpty(savedLanguage) && _languageSprites.ContainsKey(savedLanguage))
            { ApplyLanguage(savedLanguage); }   // Если язык сохранён и поддерживается
            else                                // Или определяем по спрайту
            {
                string fallbackLanguage = DetectLanguageFromSprite();
                ApplyLanguage(fallbackLanguage);
            }
        }

        private string DetectLanguageFromSprite()   // Определение языка по спрайту
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
            if (_languageImage.sprite == _spanish) return "Spanish";

            return "Russian";
        }

        public void ChangeLanguage()    // Смена языка по нажатию кнопки
        {
            string current = PlayerPrefs.GetString("language", "Russian");
            int currentIndex = System.Array.IndexOf(_languages, current);
            if (currentIndex == -1) currentIndex = 0;

            int nextIndex = (currentIndex + 1) % _languages.Length;
            string nextLanguage = _languages[nextIndex];

            ApplyLanguage(nextLanguage);
        }

        private void ApplyLanguage(string language) // Применение выбранного языка
        {
            if (!_languageSprites.TryGetValue(language, out Sprite sprite)) return;

            _languageImage.sprite = sprite;
            PlayerPrefs.SetString("language", language);
            PlayerPrefs.Save();
            LocalizationManager.CurrentLanguage = language;

            OnLanguageChanged?.Invoke();
        }
    }
}