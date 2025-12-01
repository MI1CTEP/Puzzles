using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyGame.Gameplay.Dialogue;
using DG.Tweening;

namespace MyGame.Gameplay
{
    public sealed class ScaleOfSympathy : MonoBehaviour
    {
        [SerializeField] private RectTransform _scale;
        [SerializeField] private TextMeshProUGUI _valueText;

        private RectTransform _rectTransform;
        private Sequence _seqValue;
        private Sequence _seqPosition;
        private float _maxValue;
        private float _currentValue;
        private float _startPositionY;
        private float _valueSize;
        private readonly float _timeValueAnim = 0.3f;
        private readonly float _timePositionAnim = 0.3f;
        private bool _isActive = true;

        public void Init(Scenario scenario)
        {
            CountMaxValues(scenario);
            _valueSize = _scale.anchoredPosition.x / _maxValue;
            _scale.anchoredPosition = Vector2.zero;
            UpdateValueText();
            _rectTransform = GetComponent<RectTransform>();
            _startPositionY = _rectTransform.anchoredPosition.y;
            Hide(false);
        }

        public void Show(bool isAnim)
        {
            if (_isActive) return;

            _isActive = true;
            TryStopPositionAnim();
            Move(isAnim, _startPositionY);
        }

        public void Hide(bool isAnim)
        {
            if (!_isActive) return;

            _isActive = false;
            TryStopPositionAnim();
            Move(isAnim, -_startPositionY);
        }

        public void AddValue(float value)
        {
            _currentValue += value;
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

        private void CountMaxValues(Scenario scenario)
        {
            ScenarioStage scenarioStage;
            int id = 0;
            while (true)
            {
                scenarioStage = scenario.TryGetScenarioStage(id);
                id++;
                if (scenarioStage == null)
                    return;
                if (scenarioStage.typeStage == "Dialogue")
                {
                    List<PhraseVariant> phraseVariants = scenarioStage.Dialogue.phraseVariants;

                    if (phraseVariants == null || phraseVariants.Count == 0)
                        continue;

                    float max = float.MinValue;
                    for (int i = 0; i < phraseVariants.Count; i++)
                    {
                        if (phraseVariants[i].respect > max)
                            max = phraseVariants[i].respect;
                    }
                    _maxValue += max;
                }
            }
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