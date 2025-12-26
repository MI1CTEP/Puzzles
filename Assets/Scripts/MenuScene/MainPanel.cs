using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace MyGame.Menu
{
    public sealed class MainPanel : MenuPanel
    {
        [SerializeField] private MenuButton _buttonGallary;
        [SerializeField] private MenuButton _buttonStore;
        [SerializeField] private MenuButton _buttonSettings;
        [SerializeField] private MenuButton _buttonExit;

        private RectTransform _rectTransform;
        private Vector2 _startSize;

        public void Init(UnityAction onShowGallaryPanel, UnityAction onShowSettingsPanel)
        {
            gameObject.SetActive(true);
            MenuPanelInit();
            _rectTransform = GetComponent<RectTransform>();
            _startSize = _rectTransform.sizeDelta;
            _rectTransform.sizeDelta = Vector2.zero;

            _buttonGallary.Init(onShowGallaryPanel);
            _buttonStore.Init(null);
            _buttonSettings.Init(onShowSettingsPanel);
            _buttonExit.Init(null);

            FirstShow();
        }

        protected override void OnStartShow() { }

        protected override void OnEndShow()
        {
            _seq = DOTween.Sequence();
            _seq.InsertCallback(0, _buttonGallary.Show);
            _seq.InsertCallback(0.1f, _buttonStore.Show);
            _seq.InsertCallback(0.2f, _buttonSettings.Show);
            _seq.InsertCallback(0.3f, _buttonExit.Show);
        }

        protected override void OnHide()
        {
            _seq.InsertCallback(0, _buttonExit.Hide);
            _seq.InsertCallback(0.1f, _buttonSettings.Hide);
            _seq.InsertCallback(0.2f, _buttonStore.Hide);
            _seq.InsertCallback(0.3f, _buttonGallary.Hide);
        }
    }
}