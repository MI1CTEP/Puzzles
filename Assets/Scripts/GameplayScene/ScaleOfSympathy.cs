using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyGame.Gameplay.Dialogue;

namespace MyGame.Gameplay
{
    public sealed class ScaleOfSympathy : MonoBehaviour
    {
        [SerializeField] private Image _negativeScale;
        [SerializeField] private Image _positiveScale;

        private RectTransform _rectTransform;
        private float _maxNegativeValue;
        private float _maxPositiveValue;
        private float _value;
        private float _startPositionY;

        public void Init(Scenario scenario)
        {
            CountMaxValues(scenario);
            _rectTransform = GetComponent<RectTransform>();
            _startPositionY = _rectTransform.anchoredPosition.y;
            Hide();
            _negativeScale.fillAmount = 0;
            _positiveScale.fillAmount = 0;
        }

        public void Show()
        {
            _rectTransform.anchoredPosition = new Vector2(0, _startPositionY);
        }

        public void Hide()
        {
            _rectTransform.anchoredPosition = new Vector2(0, -_startPositionY);
        }

        public void AddValue(float value)
        {
            _value += value;
            if(_value > 0)
            {
                _negativeScale.fillAmount = 0;
                _positiveScale.fillAmount = _value / _maxPositiveValue;
            }
            else
            {
                _positiveScale.fillAmount = 0;
                _negativeScale.fillAmount = _value / _maxNegativeValue;
            }
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
                if(scenarioStage.TypeStage == TypeStage.SetDialogue)
                {
                    List<PhraseVariant> phraseVariants = scenarioStage.SimpleDialogue.PhraseVariants;

                    if (phraseVariants == null || phraseVariants.Count == 0)
                        continue;

                    float min = float.MaxValue;
                    float max = float.MinValue;
                    for (int i = 0; i < phraseVariants.Count; i++)
                    {
                        if (phraseVariants[i].Respect < min)
                            min = phraseVariants[i].Respect;
                        if (phraseVariants[i].Respect > max)
                            max = phraseVariants[i].Respect;
                    }

                    _maxNegativeValue += min;
                    _maxPositiveValue += max;
                }
            }
        }
    }
}