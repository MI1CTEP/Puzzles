using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace MyGame.Menu
{
    public sealed class StoryPanel : MenuPanel
    {
        [SerializeField] private GameObject _backgroundImage;
        [SerializeField] private GameObject _scrollView;
        [SerializeField] private MenuButton _buttonClose;
        [SerializeField] private TextMeshProUGUI _text;

        public void Init(UnityAction onShowGallaryPanel)
        {
            MenuPanelInit();
            gameObject.SetActive(false);
            _buttonClose.Init(onShowGallaryPanel);
        }

        protected override void OnStartShow() { }

        protected override void OnEndShow()
        {
            _buttonClose.Show();
            _backgroundImage.SetActive(true);
            _scrollView.SetActive(true);

            string currentLang = I2.Loc.LocalizationManager.CurrentLanguage;
                string text = currentLang switch
                {
                    "Russian" => AsyncContent.LevelInfo.languages.ru,
                    "English" => AsyncContent.LevelInfo.languages.en,
                    "German" => AsyncContent.LevelInfo.languages.de,
                    "Chinese" => AsyncContent.LevelInfo.languages.zh,
                    "French" => AsyncContent.LevelInfo.languages.fr,
                    "Hindi" => AsyncContent.LevelInfo.languages.hi,
                    "Italian" => AsyncContent.LevelInfo.languages.it,
                    "Japanese" => AsyncContent.LevelInfo.languages.ja,
                    "Portuguese" => AsyncContent.LevelInfo.languages.pt,
                    "Spanish" => AsyncContent.LevelInfo.languages.es,
                    _ => AsyncContent.LevelInfo.languages.en
                };
            _text.text = text;
        }

        protected override void OnHide()
        {
            _buttonClose.Hide();
            _backgroundImage.SetActive(false);
            _scrollView.SetActive(false);
        }
    }
}