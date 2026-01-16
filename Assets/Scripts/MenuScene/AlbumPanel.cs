using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using MyGame.Bundles;
using I2;

namespace MyGame.Menu
{
    public sealed class AlbumPanel : MenuPanel
    {
        [SerializeField] private Image _content;
        [SerializeField] private GameObject _noImage;
        [SerializeField] private GameObject _infoBackground;
        [SerializeField] private TextMeshProUGUI _infoText;
        [SerializeField] private MenuButton _buttonPrevious;
        [SerializeField] private MenuButton _buttonNext;
        [SerializeField] private MenuButton _buttonClose;
        [SerializeField] private Sprite _grayBackground;
        [SerializeField] private Languages[] _infosLanguages;

        private int _currentId;

        public void Init(UnityAction onShowGallaryPanel)
        {
            gameObject.SetActive(false);
            MenuPanelInit();
            _buttonPrevious.Init(()=> SetIdAndUpdate(-1));
            _buttonNext.Init(() => SetIdAndUpdate(1));
            _buttonClose.Init(onShowGallaryPanel);
        }

        protected override void OnStartShow()
        {
            BundlesController.Instance.ExtraImagesBundle.Load(GameData.CurrentLevel, null);
        }

        protected override void OnEndShow()
        {
            _currentId = 0;
            _content.gameObject.SetActive(true);
            _buttonPrevious.Show();
            _buttonNext.Show();
            _buttonClose.Show();
            SetIdAndUpdate(0);
        }

        protected override void OnHide()
        {
            BundlesController.Instance.ExtraImagesBundle.TryUnload();
            _content.gameObject.SetActive(false);
            _infoBackground.SetActive(false);
            _buttonPrevious.Hide();
            _buttonNext.Hide();
            _buttonClose.Hide();
        }

        private void SetIdAndUpdate(int value)
        {
            _currentId += value;
            if (_currentId < 0)
                _currentId = 9;
            else if (_currentId > 9)
                _currentId = 0;

            if(GameData.Achievements.IsUnlock(GameData.CurrentLevel, _currentId))
            {
                _noImage.SetActive(false);
                _content.sprite = BundlesController.Instance.ExtraImagesBundle.Sprites[_currentId];
                _infoBackground.SetActive(false);
            }
            else
            {
                _noImage.SetActive(true);
                _content.sprite = _grayBackground;
                _infoBackground.SetActive(true);
                _infoText.text = _infosLanguages[_currentId].ru;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            BundlesController.Instance.ExtraImagesBundle.TryUnload();
        }
    }
}