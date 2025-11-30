using UnityEngine;
using MyGame.Gameplay.Puzzle;
using MyGame.Gameplay.Dialogue;
using MyGame.Gifts;

namespace MyGame.Gameplay
{
    public sealed class GameplayController : MonoBehaviour
    {
        [SerializeField] private ScaleOfSympathy _scaleOfSympathy;
        [SerializeField] private VideoController _videoController;
        [SerializeField] private PuzzleController _puzzleController;
        [SerializeField] private BackgroundController _backgroundController;
        [SerializeField] private DialogueController _dialogueController;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private GiftController _giftController;
        [SerializeField] private GiftsGiver _giftsGiver;

        private ScenarioLoader _scenarioLoader;
        private Scenario _scenario;
        private ScenarioStage _currentScenarioStage;
        private IGameStage _currentGameStage;
        private int _currentGameStageId;

        private void Start()
        {
            Init();
            TryStartNextStage();
        }

        private void Init()
        {
            _scenarioLoader = new();
            _scenario = _scenarioLoader.GetScenario(1);
            _scaleOfSympathy.Init(_scenario);
            _videoController.Init();
            _puzzleController.Init();
            _dialogueController.Init(_scaleOfSympathy);
            _cameraController.Init();
            _giftController.Init();
            _giftsGiver.Init(_giftController);
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
                    _currentGameStage = _puzzleController;
                    _scaleOfSympathy.Hide();
                    UpdateCameraSize();
                    SetBackgroundImage();
                    StartNextStage();
                    break;
                case "Video":
                    _currentGameStage = _videoController;
                    _scaleOfSympathy.Hide();
                    StartNextStage();
                    break;
                case "Dialogue":
                    _currentGameStage = _dialogueController;
                    _scaleOfSympathy.Show();
                    UpdateCameraSize();
                    SetBackgroundImage();
                    StartNextStage();
                    break;
                case "Gifts":
                    _currentGameStage = _giftsGiver;
                    _scaleOfSympathy.Hide();
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
            _cameraController.UpdateSize(_currentScenarioStage.Image.texture.width);
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
            _giftController.ShowRoulette(null);
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