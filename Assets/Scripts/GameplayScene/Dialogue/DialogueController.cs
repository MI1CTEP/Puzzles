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

        private SimpleDialogue _simpleDialogue;
        private PhraseButton[] _phraseButtons;
        private DialogueAnim _anim;
        private RectTransform _continueButtonRect;

        public UnityAction OnEnd { get; set; }

        public void Init()
        {
            _anim = new(_messageHistory);
            _continueButton.onClick.AddListener(End);
            _continueButton.gameObject.SetActive(false);
            _continueButtonRect = _continueButton.GetComponent<RectTransform>();
            _continueButtonRect.anchoredPosition = new Vector2(0, _offsetAnswers);
        }

        public void Play(ScenarioStage scenarioStage)
        {
            _messageHistory.anchoredPosition = new Vector2(0, _offsetAnswers);
            gameObject.SetActive(true);
            _simpleDialogue = scenarioStage.SimpleDialogue;
            SetFirstPhrase();
        }

        private void SetFirstPhrase()
        {
            if(_simpleDialogue.FirstPhrase == null)
            {
                End();
                return;
            }

            SetPhrase(_simpleDialogue.FirstPhrase, false, SetVariants);
        }

        private void SetVariants()
        {
            if (_simpleDialogue.PhraseVariants == null || _simpleDialogue.PhraseVariants.Count == 0)
            {
                EnableContinueButton();
                return;
            }

            TryDestroyPhraseButtons();

            _phraseButtons = new PhraseButton[_simpleDialogue.PhraseVariants.Count];

            float anchorPositionY = _offsetAnswers;
            for (int i = 0; i < _simpleDialogue.PhraseVariants.Count; i++)
            {
                int id = i;
                _phraseButtons[i] = Instantiate(_phraseButtonPrefab, transform);
                _phraseButtons[i].Init(_simpleDialogue.PhraseVariants[i].Answer, ref anchorPositionY, () => SetAnswer(id));
                anchorPositionY += _offsetAnswers;
            }
            _anim.MoveMessageHistory(anchorPositionY);
        }

        private void SetAnswer(int id)
        {
            SetPhrase(_simpleDialogue.PhraseVariants[id].Answer, true, () => SetSecondPhrase(id));
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
            if(_simpleDialogue.PhraseVariants[id].SecondPhrase == null)
            {
                EnableContinueButton();
                return;
            }
            _anim.MoveMessageHistory(_offsetAnswers);
            SetPhrase(_simpleDialogue.PhraseVariants[id].SecondPhrase, false, EnableContinueButton);
        }

        private void SetPhrase(Phrase phrase, bool isPlayerPhrase, UnityAction onSetPhrase)
        {
            PhraseImage phraseImage = Instantiate(_phraseImagePrefab, _messageHistory);
            phraseImage.Init(phrase, isPlayerPhrase, onSetPhrase);
        }

        private void EnableContinueButton()
        {
            float anchorPositionY = _offsetAnswers * 2 + _continueButtonRect.sizeDelta.y;
            _continueButton.gameObject.SetActive(true);
            _anim.MoveMessageHistory(anchorPositionY);
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