using UnityEngine;
using MyGame.Gameplay.Puzzle;
using MyGame.Gameplay.Dialogue;
using MyGame.Gifts;

namespace MyGame.Gameplay
{
    public sealed class GameplayController : MonoBehaviour
    {
        [SerializeField] private ProgressPanel _progressPanel;
        [SerializeField] private VideoController _videoController;
        [SerializeField] private PuzzleController _puzzleController;
        [SerializeField] private BackgroundController _backgroundController;
        [SerializeField] private DialogueController _dialogueController;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private GiftController _giftController;
        [SerializeField] private GiftsGiver _giftsGiver;
        [SerializeField] private EndPanel _endPanel;
        [SerializeField] private int _level;

        private ScenarioLoader _scenarioLoader;
        private Scenario _scenario;
        private ScenarioStage _currentScenarioStage;
        private IGameStage _currentGameStage;
        private int _currentGameStageId;

        private void Start()
        {
            GameData.CurrentLevel = _level;
            Init();
            TryStartNextStage();
        }

        private void Init()
        {
            _scenarioLoader = new();
            _scenario = _scenarioLoader.GetScenario(GameData.CurrentLevel);
            _progressPanel.Init();
            _videoController.Init();
            _puzzleController.Init(_progressPanel);
            _dialogueController.Init(_scenario, _progressPanel);
            _cameraController.Init();
            _giftController.Init();
            _giftsGiver.Init(_giftController);
            _endPanel.Init(_dialogueController, _giftController);
        }

        private void TryStartNextStage()
        {
            _currentScenarioStage = _scenario.TryGetScenarioStage(_currentGameStageId);
            _currentGameStageId++;
            if (_currentScenarioStage == null)
            {
                End();
                return;
            }

            TryStopLastStage();

            switch (_currentScenarioStage.typeStage)
            {
                case "Puzzle":
                    _videoController.Disable();
                    _currentGameStage = _puzzleController;
                    _progressPanel.Hide(true);
                    UpdateCameraSize();
                    SetBackgroundImage();
                    StartNextStage();
                    break;
                case "Video":
                    _currentGameStage = _videoController;
                    _progressPanel.Hide(true);
                    StartNextStage();
                    break;
                case "Dialogue":
                    _videoController.Disable();
                    _currentGameStage = _dialogueController;
                    _progressPanel.Show(true);
                    UpdateCameraSize();
                    SetBackgroundImage();
                    StartNextStage();
                    break;
                case "Gifts":
                    _videoController.Disable();
                    _currentGameStage = _giftsGiver;
                    _progressPanel.Hide(true);
                    UpdateCameraSize();
                    SetBackgroundImage();
                    StartNextStage();
                    break;
                default:
                    End();
                    break;
            }
        }

        private void UpdateCameraSize()
        {
            _cameraController.UpdateSize(_currentScenarioStage.Image.texture.height, _currentScenarioStage.Image.texture.width);
        }

        private void SetBackgroundImage()
        {
            _backgroundController.SetSprite(_currentScenarioStage.Image, _currentScenarioStage.isAnimImage);
        }

        private void StartNextStage()
        {
            _currentGameStage.OnEnd += TryStartNextStage;
            _currentGameStage.Play(_currentScenarioStage);
        }

        private void End()
        {
            _endPanel.Show();
        }

        private void TryStopLastStage()
        {
            if (_currentGameStage != null)
            {
                _currentGameStage.OnEnd -= TryStartNextStage;
                _currentGameStage = null;
            }
        }

        private void OnDestroy()
        {
            TryStopLastStage();
        }
    }
}