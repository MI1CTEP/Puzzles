using UnityEngine;
using UnityEngine.Events;
using TMPro;
using MyGame.Bundles;

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
                    "Russian" => BundlesController.Instance.MainResourcesBundle.GetInfoLanguages.ru,
                    "English" => BundlesController.Instance.MainResourcesBundle.GetInfoLanguages.en,
                    "German" => BundlesController.Instance.MainResourcesBundle.GetInfoLanguages.de,
                    "Chinese" => BundlesController.Instance.MainResourcesBundle.GetInfoLanguages.zh,
                    "French" => BundlesController.Instance.MainResourcesBundle.GetInfoLanguages.fr,
                    "Hindi" => BundlesController.Instance.MainResourcesBundle.GetInfoLanguages.hi,
                    "Italian" => BundlesController.Instance.MainResourcesBundle.GetInfoLanguages.it,
                    "Japanese" => BundlesController.Instance.MainResourcesBundle.GetInfoLanguages.ja,
                    "Portuguese" => BundlesController.Instance.MainResourcesBundle.GetInfoLanguages.pt,
                    "Spanish" => BundlesController.Instance.MainResourcesBundle.GetInfoLanguages.es,
                    _ => BundlesController.Instance.MainResourcesBundle.GetInfoLanguages.en
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