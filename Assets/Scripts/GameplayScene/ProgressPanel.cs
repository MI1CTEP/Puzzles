using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyGame.Gameplay.Dialogue;
using DG.Tweening;

namespace MyGame.Gameplay
{
    public sealed class ProgressPanel : MonoBehaviour
    {
        [SerializeField] private GameObject[] _nameTextes;
        [SerializeField] private RectTransform _scale;
        [SerializeField] private RectTransform _scaleOld;
        [SerializeField] private Image _scaleImage;
        [SerializeField] private TextMeshProUGUI _valueText;

        private RectTransform _rectTransform;
        private Sequence _seqValue;
        private Sequence _seqPosition;
        private float _maxAnchoredPositionX;
        private float _maxValue;
        private float _currentValue;
        private float _startPositionY;
        private float _valueSize;
        private readonly float _timeValueAnim = 0.3f;
        private readonly float _timePositionAnim = 0.3f;
        private bool _isActive = true;

        public void Init()
        {
            UpdateValueText();
            _rectTransform = GetComponent<RectTransform>();
            _maxAnchoredPositionX = _scale.anchoredPosition.x;
            _startPositionY = _rectTransform.anchoredPosition.y;
            Hide(false);
        }

        public void SetSympathy(float currentValue, float maxValue)
        {
            for (int i = 0; i < _nameTextes.Length; i++)
                _nameTextes[i].SetActive(false);
            _nameTextes[0].SetActive(true);
            SetStartValues(currentValue, maxValue);
        }

        public void SetPuzzle(float currentValue, float maxValue)
        {
            for (int i = 0; i < _nameTextes.Length; i++)
                _nameTextes[i].SetActive(false);
            _nameTextes[1].SetActive(true);
            SetStartValues(currentValue, maxValue);
        }

        private void SetStartValues(float currentValue, float maxValue)
        {
            _currentValue = currentValue;
            _maxValue = maxValue;
            if (_maxValue != 0)
                _valueSize = _maxAnchoredPositionX / _maxValue;
            _scale.anchoredPosition = new Vector2(_currentValue * _valueSize, 0);
            UpdateValueText();
        }

        public void Show(bool isAnim)
        {
            if (_isActive) return;

            _isActive = true;
            TryStopPositionAnim();
            Move(isAnim, _startPositionY);
        }

        public void ShowEnd(Transform parent, float canvasSizeY, float currentValue, float oldValue, float maxValue, float timeAnim, ref float waitTimeAnim)
        {
            TryStopPositionAnim();
            TryStopValueAnim();

            transform.SetParent(parent);

            for (int i = 0; i < _nameTextes.Length; i++)
                _nameTextes[i].SetActive(false);
            _nameTextes[2].SetActive(true);

            SetStartValues(0, maxValue);

            _currentValue = currentValue;
            UpdateValueText();

            _scaleOld.gameObject.SetActive(true);
            _scaleOld.anchoredPosition = new Vector2(oldValue * _valueSize, 0);

            _rectTransform.anchoredPosition = new Vector2(0, -canvasSizeY + _startPositionY);

            _seqPosition = DOTween.Sequence();
            _seqPosition.Insert(waitTimeAnim, _rectTransform.DOLocalMoveY(0, timeAnim / 2).SetEase(Ease.OutExpo));
            _seqPosition.Insert(waitTimeAnim + timeAnim / 4, _scale.DOAnchorPosX(currentValue * _valueSize, timeAnim / 2));
            _seqPosition.Insert(waitTimeAnim + timeAnim / 2, _rectTransform.DOAnchorPosY(_startPositionY, timeAnim / 2).SetEase(Ease.InExpo));
            waitTimeAnim += timeAnim;
        }

        public void Hide(bool isAnim)
        {
            if (!_isActive) return;

            _isActive = false;
            TryStopPositionAnim();
            Move(isAnim, -_startPositionY);
        }

        public void ChangeValue(float value)
        {
            _currentValue = value;
            UpdateValueText();
            TryStopValueAnim();
            _seqValue = DOTween.Sequence();
            _seqValue.Insert(0, _scale.DOAnchorPosX(_currentValue * _valueSize, _timeValueAnim));
        }

        private void Move(bool isAnim, float positionY)
        {
            TryStopPositionAnim();
            if (isAnim)
            {
                _seqPosition = DOTween.Sequence();
                _seqPosition.Insert(0, _rectTransform.DOAnchorPosY(positionY, _timePositionAnim));
            }
            else
            {
                _rectTransform.anchoredPosition = new Vector2(0, positionY);
            }
        }

        private void UpdateValueText()
        {
            _valueText.text = $"{_currentValue}/{_maxValue}";
        }

        private void TryStopValueAnim()
        {
            if (_seqValue != null)
                _seqValue.Kill();
        }

        private void TryStopPositionAnim()
        {
            if (_seqPosition != null)
                _seqPosition.Kill();
        }

        private void OnDestroy()
        {
            TryStopValueAnim();
            TryStopPositionAnim();
        }
    }
}