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

        public void Open(ScenarioStage scenarioStage, bool isIntoScenarioMenu = false)
        {
            gameObject.SetActive(true);

            if (isIntoScenarioMenu) 
            {
                _panelEasy.ActivateIntoScenarioMenu(this, scenarioStage, _grids, TypeDifficulty.Easy);
                _panelMedium.ActivateIntoScenarioMenu(this, scenarioStage, _grids, TypeDifficulty.Medium);
                _panelHard.ActivateIntoScenarioMenu(this, scenarioStage, _grids, TypeDifficulty.Hard);
            }
            else
            {
                _panelEasy.Activate(this, scenarioStage, _grids, TypeDifficulty.Easy);
                _panelMedium.Activate(this, scenarioStage, _grids, TypeDifficulty.Medium);
                _panelHard.Activate(this, scenarioStage, _grids, TypeDifficulty.Hard);
            }

        }



        public void StartGameplay(TypeDifficulty typeDifficulty, int puzzleValueX, float chanceGetDetail)
        {
            gameObject.SetActive(false);
            _puzzleController.StartGameplay(puzzleValueX, chanceGetDetail);

            if(_puzzleController.TypeDifficulty == TypeDifficulty.None || _puzzleController.TypeDifficulty == typeDifficulty)
                _puzzleController.TypeDifficulty = typeDifficulty;
            else
                _puzzleController.TypeDifficulty = TypeDifficulty.Mixed;
        }

        public void StartGameplayIntoScenarioMenu(TypeDifficulty typeDifficulty, int puzzleValueX, float chanceGetDetail)
        {
            //gameObject.SetActive(false);

            NutakuAPIInitializator.instance.IsOpenGameplayIntoScenarioMenu = true;
            SceneLoader.LoadGameplay();

            //_puzzleController.StartGameplay(puzzleValueX, chanceGetDetail);

            //if (_puzzleController.TypeDifficulty == TypeDifficulty.None || _puzzleController.TypeDifficulty == typeDifficulty)
            //    _puzzleController.TypeDifficulty = typeDifficulty;
            //else
            //    _puzzleController.TypeDifficulty = TypeDifficulty.Mixed;
        }
    }

    public enum TypeDifficulty
    {
        None, Easy, Medium, Hard, Mixed
    }
}