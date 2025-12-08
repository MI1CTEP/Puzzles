using UnityEngine;
using TMPro;

namespace MyGame.Gameplay.ExtraLevel
{
    public sealed class ExtraLevelValue : MonoBehaviour
    {
        [SerializeField] private RectTransform _valueTransform;
        [SerializeField] private TextMeshProUGUI _valueText;

        private int _maxValue;
        private int _currentValue;
        private float _valueStep;

        public void Init(int maxValue)
        {
            _maxValue = maxValue;
            _valueStep = _valueTransform.anchoredPosition.x / _maxValue;
        }

        public void Addvalue()
        {
            _currentValue++;
        }

        public void UpdateValue()
        {
            Addvalue();
            _valueTransform.anchoredPosition = new Vector2(_valueStep * _currentValue, 0);
            _valueText.text = $"{_currentValue}/{_maxValue}";
        }
    }
}