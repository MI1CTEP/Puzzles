using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MyGame.Menu
{
    public abstract class MenuPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransformBackground;

        private Image _image;
        protected Sequence _seq;
        private Vector2 _startSizeDelta;
        private Vector2 _startPosition;
        private Color _startColor;
        protected readonly float _timeAnim = 0.5f;

        private Vector2 SizeDelta => _rectTransformBackground.sizeDelta;
        private Vector2 Position => _rectTransformBackground.position;
        private Color Color => _startColor;

        public static MenuPanel CurrentMenuPanel { get; set; }

        public void Show()
        {
            CurrentMenuPanel.Hide(this);
        }

        protected void MenuPanelInit()
        {
            _image = _rectTransformBackground.GetComponent<Image>();
            _startSizeDelta = _rectTransformBackground.sizeDelta;
            _startPosition = _rectTransformBackground.position;
            _startColor = _image.color;
        }

        protected void FirstShow()
        {
            _seq = DOTween.Sequence();
            _seq.Insert(0, _rectTransformBackground.DOSizeDelta(_startSizeDelta, _timeAnim).SetEase(Ease.OutBounce));
            _seq.InsertCallback(_timeAnim, OnEndShow);
        }

        private void ResetPanel()
        {
            _rectTransformBackground.sizeDelta = _startSizeDelta;
            _rectTransformBackground.position = _startPosition;
            _image.color = _startColor;
        }

        private void Hide(MenuPanel panelBackgroundTarget)
        {
            panelBackgroundTarget.OnStartShow();
            TryStopAnim();
            _seq = DOTween.Sequence();
            OnHide();
            _seq.Insert(0, _rectTransformBackground.DOSizeDelta(panelBackgroundTarget.SizeDelta, _timeAnim));
            _seq.Insert(0, _rectTransformBackground.DOMove(panelBackgroundTarget.Position, _timeAnim));
            _seq.Insert(0, _image.DOColor(panelBackgroundTarget.Color, _timeAnim));
            _seq.InsertCallback(_timeAnim, () => 
            {
                gameObject.SetActive(false);
                ResetPanel();
                CurrentMenuPanel = panelBackgroundTarget;
                panelBackgroundTarget.gameObject.SetActive(true);
                panelBackgroundTarget.OnEndShow();
            });
        }

        protected abstract void OnStartShow();

        protected abstract void OnEndShow();

        protected abstract void OnHide();

        private void TryStopAnim()
        {
            if (_seq != null)
                _seq.Kill();
        }

        protected virtual void OnDestroy()
        {
            TryStopAnim();
        }
    }
}