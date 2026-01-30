using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MyGame.Gameplay.Dialogue
{
    public sealed class DialogueController : MonoBehaviour, IGameStage
    {
        [SerializeField] private PhraseImage _phraseImagePrefab;
        [SerializeField] private PhraseButton _phraseButtonPrefab;
        [SerializeField] private RectTransform _messageHistory;
        [SerializeField] private Button _continueButton;
        [SerializeField] private float _offsetAnswers;

        private Button _button;
        private ProgressPanel _progressPanel;
        private SimpleDialogue _simpleDialogue;
        private PhraseButton[] _phraseButtons;
        private DialogueAnim _anim;
        private RectTransform _continueButtonRect;
        private int _currentSympathy;
        private int _maxSympathy;

        public int CurrentSympathy => _currentSympathy;
        public int MaxSympathy => _maxSympathy;

        public UnityAction OnEnd { get; set; }
        public UnityAction OnSkipTextAnim { get; set; }

        public void Init(Scenario scenario, ProgressPanel progressPanel)
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(TrySkipTextAnim);
            _progressPanel = progressPanel;
            _anim = new(_messageHistory);
            _continueButton.onClick.AddListener(End);
            _continueButton.gameObject.SetActive(false);
            _continueButtonRect = _continueButton.GetComponent<RectTransform>();
            _continueButtonRect.anchoredPosition = new Vector2(0, _offsetAnswers);
            CountMaxSympathy(scenario);
        }

        public void Play(ScenarioStage scenarioStage)
        {
            _progressPanel.SetSympathy(_currentSympathy, _maxSympathy);
            _messageHistory.anchoredPosition = new Vector2(0, _offsetAnswers);
            gameObject.SetActive(true);
            _simpleDialogue = scenarioStage.Dialogue;
            SetFirstPhrase();
        }

        private void SetFirstPhrase()
        {
            if(_simpleDialogue.firstPhrase == null)
            {
                End();
                return;
            }

            SetPhrase(_simpleDialogue.firstPhrase, false, SetVariants);
        }

        private void SetVariants()
        {
            if (_simpleDialogue.phraseVariants == null || _simpleDialogue.phraseVariants.Count == 0)
            {
                EnableContinueButton();
                return;
            }

            TryDestroyPhraseButtons();

            _phraseButtons = new PhraseButton[_simpleDialogue.phraseVariants.Count];

            float anchorPositionY = _offsetAnswers;
            
            int[] ids = new int[_simpleDialogue.phraseVariants.Count];
            for (int i = 0; i < ids.Length; i++)
                ids[i] = i;
            for (int i = 0; i < ids.Length; i++)
            {
                int r = Random.Range(0, ids.Length);
                (ids[i], ids[r]) = (ids[r], ids[i]);
            }

            for (int i = 0; i < _simpleDialogue.phraseVariants.Count; i++)
            {
                int id = ids[i];
                _phraseButtons[id] = Instantiate(_phraseButtonPrefab, transform);
                _phraseButtons[id].Init(_simpleDialogue.phraseVariants[id].answer, ref anchorPositionY, () => SetAnswer(id));
                _phraseButtons[id].TryShowValue(_simpleDialogue.id, id, _simpleDialogue.phraseVariants[id].respect);
                anchorPositionY += _offsetAnswers;
            }
            _anim.MoveMessageHistory(anchorPositionY);
        }

        private void SetAnswer(int id)
        {
            SetPhrase(_simpleDialogue.phraseVariants[id].answer, true, () => SetSecondPhrase(id));
            _currentSympathy += _simpleDialogue.phraseVariants[id].respect;
            _progressPanel.ChangeValue(_currentSympathy);
            GameData.Dialogues.Unlock(GameData.CurrentLevel, _simpleDialogue.id, id);
            TryDestroyPhraseButtons();
        }

        private void TryDestroyPhraseButtons()
        {
            if (_phraseButtons != null)
                for (int i = 0; i < _phraseButtons.Length; i++)
                    Destroy(_phraseButtons[i].gameObject);
            _phraseButtons = null;
        }

        private void SetSecondPhrase(int id)
        {
            if(_simpleDialogue.phraseVariants[id].secondPhrase == null)
            {
                EnableContinueButton();
                return;
            }
            _anim.MoveMessageHistory(_offsetAnswers);
            SetPhrase(_simpleDialogue.phraseVariants[id].secondPhrase, false, EnableContinueButton);
        }

        private void SetPhrase(Languages phrase, bool isPlayerPhrase, UnityAction onSetPhrase)
        {
            PhraseImage phraseImage = Instantiate(_phraseImagePrefab, _messageHistory);
            phraseImage.Init(this, phrase, isPlayerPhrase, onSetPhrase);
        }

        private void EnableContinueButton()
        {
            float anchorPositionY = _offsetAnswers * 2 + _continueButtonRect.sizeDelta.y;
            _continueButton.gameObject.SetActive(true);
            _anim.MoveMessageHistory(anchorPositionY);
        }

        private void TrySkipTextAnim()
        {
            OnSkipTextAnim?.Invoke();
        }

        private void CountMaxSympathy(Scenario scenario)
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

                    int max = 0;
                    for (int i = 0; i < phraseVariants.Count; i++)
                    {
                        if (phraseVariants[i].respect > max)
                            max = phraseVariants[i].respect;
                    }
                    _maxSympathy += max;
                }
            }
        }

        private void End()
        {
            _continueButton.gameObject.SetActive(false);
            gameObject.SetActive(false);
            OnEnd?.Invoke();
        }

        private void OnDestroy()
        {
            _anim.TryStopAnim();
        }
    }
}