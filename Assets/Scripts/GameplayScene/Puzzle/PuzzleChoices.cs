using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Gameplay.Puzzle
{
    public sealed class PuzzleChoices : MonoBehaviour
    {
        [SerializeField] private Image _gridEasy;
        [SerializeField] private Image _gridMedium;
        [SerializeField] private Image _gridHard;
        [SerializeField] private Button _buttonPlayEasy;
        [SerializeField] private Button _buttonPlayMedium;
        [SerializeField] private Button _buttonPlayHard;
        [SerializeField] private Sprite[] _grids;

        private PuzzleController _puzzleController;

        public void Init(PuzzleController puzzleController)
        {
            _puzzleController = puzzleController;
        }

        public void Open(ScenarioStage scenarioStage)
        {
            gameObject.SetActive(true);

            _gridEasy.sprite = _grids[scenarioStage.PuzzleValueXEasy - 2];
            _buttonPlayEasy.onClick.RemoveAllListeners();
            _buttonPlayEasy.onClick.AddListener(() => StartGameplay(scenarioStage.PuzzleValueXEasy));

            _gridMedium.sprite = _grids[scenarioStage.PuzzleValueXMedium - 2];
            _buttonPlayMedium.onClick.RemoveAllListeners();
            _buttonPlayMedium.onClick.AddListener(() => StartGameplay(scenarioStage.PuzzleValueXMedium));

            _gridHard.sprite = _grids[scenarioStage.PuzzleValueXHard - 2];
            _buttonPlayHard.onClick.RemoveAllListeners();
            _buttonPlayHard.onClick.AddListener(() => StartGameplay(scenarioStage.PuzzleValueXHard));
        }

        private void StartGameplay(int puzzleValueX)
        {
            gameObject.SetActive(false);
            _puzzleController.StartGameplay(puzzleValueX);
        }
    }
}