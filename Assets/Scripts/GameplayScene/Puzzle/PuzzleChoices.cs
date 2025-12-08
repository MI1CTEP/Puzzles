using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Gameplay.Puzzle
{
    public sealed class PuzzleChoices : MonoBehaviour
    {
        [SerializeField] private PuzzleChoiceLevelPanel _panelEasy;
        [SerializeField] private PuzzleChoiceLevelPanel _panelMedium;
        [SerializeField] private PuzzleChoiceLevelPanel _panelHard;
        [SerializeField] private Sprite[] _grids;

        private PuzzleController _puzzleController;

        public void Init(PuzzleController puzzleController)
        {
            _puzzleController = puzzleController;
        }

        public void Open(ScenarioStage scenarioStage)
        {
            gameObject.SetActive(true);
            _panelEasy.Activate(this, scenarioStage, _grids, TypeDifficulty.Easy);
            _panelMedium.Activate(this, scenarioStage, _grids, TypeDifficulty.Medium);
            _panelHard.Activate(this, scenarioStage, _grids, TypeDifficulty.Hard);
        }

        public void StartGameplay(int puzzleValueX, float chanceGetDetail)
        {
            gameObject.SetActive(false);
            _puzzleController.StartGameplay(puzzleValueX, chanceGetDetail);
        }
    }

    public enum TypeDifficulty
    {
        Easy, Medium, Hard
    }
}