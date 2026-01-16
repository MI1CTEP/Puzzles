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
            _text.text = BundlesController.Instance.MainResourcesBundle.GetInfoLanguages.ru;
        }

        protected override void OnHide()
        {
            _buttonClose.Hide();
            _backgroundImage.SetActive(false);
            _scrollView.SetActive(false);
        }
    }
}