using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MyGame.Gameplay.Puzzle
{
    public sealed class PuzzleChoiceLevelPanel : MonoBehaviour
    {
        [SerializeField] private Image _grid;
        [SerializeField] private Button _buttonPlay;
        [SerializeField] private TextMeshProUGUI _detailChanceValueText;

        public void Activate(PuzzleChoices puzzzleChoices, ScenarioStage scenarioStage, Sprite[] grids, TypeDifficulty typeDifficulty)
        {
            _buttonPlay.onClick.RemoveAllListeners();
            switch (typeDifficulty)
            {
                case TypeDifficulty.Easy:
                    _grid.sprite = grids[GameData.EasyPuzzle - 2];
                    _detailChanceValueText.text = (GameData.ExtraLevel.ChanceEasy * 100).ToString("0") + "%";
                    _buttonPlay.onClick.AddListener(() => puzzzleChoices.StartGameplay(TypeDifficulty.Easy, GameData.EasyPuzzle, GameData.ExtraLevel.ChanceEasy));
                    break;
                case TypeDifficulty.Medium:
                    _grid.sprite = grids[GameData.MediumPuzzle - 2];
                    _detailChanceValueText.text = (GameData.ExtraLevel.ChanceMedium * 100).ToString("0") + "%";
                    _buttonPlay.onClick.AddListener(() => puzzzleChoices.StartGameplay(TypeDifficulty.Medium, GameData.MediumPuzzle, GameData.ExtraLevel.ChanceMedium));
                    break;
                case TypeDifficulty.Hard:
                    _grid.sprite = grids[GameData.HardPuzzle- 2];
                    _detailChanceValueText.text = (GameData.ExtraLevel.ChanceHard * 100).ToString("0") + "%";
                    _buttonPlay.onClick.AddListener(() => puzzzleChoices.StartGameplay(TypeDifficulty.Hard, GameData.HardPuzzle, GameData.ExtraLevel.ChanceHard));
                    break;
                default:
                    break;
            }
        }


        public void ActivateIntoScenarioMenu(PuzzleChoices puzzzleChoices, ScenarioStage scenarioStage, Sprite[] grids, TypeDifficulty typeDifficulty)
        {
            _buttonPlay.onClick.RemoveAllListeners();
            switch (typeDifficulty)
            {
                case TypeDifficulty.Easy:
                    _grid.sprite = grids[GameData.EasyPuzzle - 2];
                    _detailChanceValueText.text = (GameData.ExtraLevel.ChanceEasy * 100).ToString("0") + "%";
                    _buttonPlay.onClick.AddListener(() => puzzzleChoices.StartGameplayIntoScenarioMenu(TypeDifficulty.Easy, GameData.EasyPuzzle, GameData.ExtraLevel.ChanceEasy));
                    break;
                case TypeDifficulty.Medium:
                    _grid.sprite = grids[GameData.MediumPuzzle - 2];
                    _detailChanceValueText.text = (GameData.ExtraLevel.ChanceMedium * 100).ToString("0") + "%";
                    _buttonPlay.onClick.AddListener(() => puzzzleChoices.StartGameplayIntoScenarioMenu(TypeDifficulty.Medium, GameData.MediumPuzzle, GameData.ExtraLevel.ChanceMedium));
                    break;
                case TypeDifficulty.Hard:
                    _grid.sprite = grids[GameData.HardPuzzle - 2];
                    _detailChanceValueText.text = (GameData.ExtraLevel.ChanceHard * 100).ToString("0") + "%";
                    _buttonPlay.onClick.AddListener(() => puzzzleChoices.StartGameplayIntoScenarioMenu(TypeDifficulty.Hard, GameData.HardPuzzle, GameData.ExtraLevel.ChanceHard));
                    break;
                default:
                    break;
            }
        }
    }
}