using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace MyGame.Menu
{
    public sealed class SympathyPanel : MonoBehaviour
    {
        [SerializeField] private Image _valueImage;
        [SerializeField] private TextMeshProUGUI _valueText;

        private RectTransform _rectTransform;
        private Sequence _seqMove;
        private Sequence _seqValue;
        private readonly float _showTimeAnim = 0.2f;
        private readonly float _valueTimeAnim = 0.3f;
        private bool _isActive;

        public void Init()
        {
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.anchoredPosition = new Vector2(0, -200);
        }

        public void Show()
        {
            if (_isActive)
                return;
            _isActive = true;
            TryStopMoveAnim();
            _seqMove = DOTween.Sequence();
            _seqMove.Insert(0, _rectTransform.DOAnchorPosY(0, _showTimeAnim));
        }

        public void Hide()
        {
            if (!_isActive)
                return;
            _isActive = false;
            TryStopMoveAnim();
            _seqMove = DOTween.Sequence();
            _seqMove.Insert(0, _rectTransform.DOAnchorPosY(-200, _showTimeAnim));
        }

        public void UpdateValue(int value)
        {
            float maxValue = 20; //Максимальный уровень симпатии. В дальнейшем нужно будет высчитывать это значение
            TryStopValueAnim();
            _valueText.text = $"{value}/{maxValue}";
            _seqValue = DOTween.Sequence();
            float timeWait = 0;
            if (!_isActive)
                timeWait = _showTimeAnim;
            _seqValue.Insert(timeWait, _valueImage.DOFillAmount(value / (maxValue * 2) + 0.5f, _valueTimeAnim));
        }

        private void TryStopMoveAnim()
        {
            if (_seqMove != null)
                _seqMove.Kill();
        }

        private void TryStopValueAnim()
        {
            if (_seqValue != null)
                _seqValue.Kill();
        }

        private void OnDestroy()
        {
            TryStopMoveAnim();
            TryStopValueAnim();
        }
    }
}