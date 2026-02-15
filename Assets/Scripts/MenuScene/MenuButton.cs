using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace MyGame.Menu
{
    public sealed class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Button _button;
        private Sequence _seq;
        private readonly float _timeAnim = 0.2f;
        private bool _isHide;

        public void Init(UnityAction onClick)
        {
            _button = GetComponent<Button>();
            if (onClick != null)
                _button.onClick.AddListener(onClick);
            _isHide = true;
            gameObject.SetActive(false);
        }

        public void Show()
        {
            if (!_isHide)
                return;
            _isHide = false;
            SetInteractable(true);
            TryStopAnim();
            _seq = DOTween.Sequence();
            transform.localScale = Vector3.one * 0.8f;
            gameObject.SetActive(true);
            _seq.Insert(0, transform.DOScale(Vector3.one, _timeAnim));
        }

        public void Hide()
        {
            if (_isHide)
                return;
            _isHide = true;
            TryStopAnim();
            SetInteractable(false);
            _seq = DOTween.Sequence();
            _seq.Insert(0, transform.DOScale(Vector3.one * 0.8f, _timeAnim));
            _seq.InsertCallback(_timeAnim, () => gameObject.SetActive(false));
        }

        public void SetInteractable(bool value)
        {
            _button.interactable = value;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isHide)
                return;
            TryStopAnim();
            _seq = DOTween.Sequence();
            _seq.Insert(0, transform.DOScale(Vector3.one * 1.1f, _timeAnim));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isHide)
                return;
            TryStopAnim();
            _seq = DOTween.Sequence();
            _seq.Insert(0, transform.DOScale(Vector3.one, _timeAnim));
        }

        private void TryStopAnim()
        {
            if (_seq != null)
                _seq.Kill();
        }

        private void OnDestroy()
        {
            TryStopAnim();
        }
    }
}