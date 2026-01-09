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
                    _grid.sprite = grids[scenarioStage.easyValueX - 2];
                    _detailChanceValueText.text = (GameData.ExtraLevel.ChanceEasy * 100).ToString("0") + "%";
                    _buttonPlay.onClick.AddListener(() => puzzzleChoices.StartGameplay(TypeDifficulty.Easy, scenarioStage.easyValueX, GameData.ExtraLevel.ChanceEasy));
                    break;
                case TypeDifficulty.Medium:
                    _grid.sprite = grids[scenarioStage.mediumValueX - 2];
                    _detailChanceValueText.text = (GameData.ExtraLevel.ChanceMedium * 100).ToString("0") + "%";
                    _buttonPlay.onClick.AddListener(() => puzzzleChoices.StartGameplay(TypeDifficulty.Medium, scenarioStage.mediumValueX, GameData.ExtraLevel.ChanceMedium));
                    break;
                case TypeDifficulty.Hard:
                    _grid.sprite = grids[scenarioStage.hardValueX - 2];
                    _detailChanceValueText.text = (GameData.ExtraLevel.ChanceHard * 100).ToString("0") + "%";
                    _buttonPlay.onClick.AddListener(() => puzzzleChoices.StartGameplay(TypeDifficulty.Hard, scenarioStage.hardValueX, GameData.ExtraLevel.ChanceHard));
                    break;
                default:
                    break;
            }
        }
    }
}