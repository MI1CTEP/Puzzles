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
        [SerializeField] private UIEnergyTimer _energyTimer;

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

        public void StartGameplay(TypeDifficulty typeDifficulty, int puzzleValueX, float chanceGetDetail)
        {
            if (!_energyTimer.CheckEnergy(5)) return;
            
            gameObject.SetActive(false);
            _puzzleController.StartGameplay(puzzleValueX, chanceGetDetail);

            if(_puzzleController.TypeDifficulty == TypeDifficulty.None || _puzzleController.TypeDifficulty == typeDifficulty)
                _puzzleController.TypeDifficulty = typeDifficulty;
            else
                _puzzleController.TypeDifficulty = TypeDifficulty.Mixed;
        }
    }

    public enum TypeDifficulty
    {
        None, Easy, Medium, Hard, Mixed
    }
}