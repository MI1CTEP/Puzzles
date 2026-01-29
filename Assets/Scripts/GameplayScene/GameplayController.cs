using UnityEngine;
using MyGame.Gameplay.Puzzle;
using MyGame.Gameplay.Dialogue;
using MyGame.Gifts;
using MyGame.Gameplay.ExtraLevel;
using MyGame.Shop;
using Cysharp.Threading.Tasks;

namespace MyGame.Gameplay
{
    public sealed class GameplayController : MonoBehaviour
    {
        [SerializeField] private ProgressPanel _progressPanel;
        [SerializeField] private VideoController _videoController;
        [SerializeField] private ExtraLevelUnlocker _extraLevelUnlocker;
        [SerializeField] private PuzzleController _puzzleController;
        [SerializeField] private BackgroundController _backgroundController;
        [SerializeField] private DialogueController _dialogueController;
        [SerializeField] private PaidContentOpenedPanel _paidContentOpenedPanel;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private GiftController _giftController;
        [SerializeField] private ShopController _shopController;
        [SerializeField] private GiftsGiver _giftsGiver;
        [SerializeField] private Achievements _achievements;
        [SerializeField] private EndPanel _endPanel;

        private ScenarioLoader _scenarioLoader;
        private Scenario _scenario;
        private ScenarioStage _currentScenarioStage;
        private IGameStage _currentGameStage;
        private int _currentGameStageId;
        private bool _isPaidContent = false;

        private void Start()
        {
            Init();
        }

        private async UniTask Init()
        {
            _scenarioLoader = new();
            _scenario = await _scenarioLoader.GetScenario();
            _progressPanel.Init();
            _videoController.Init();
            _extraLevelUnlocker.Init();
            _puzzleController.Init(_progressPanel, _extraLevelUnlocker);
            _dialogueController.Init(_scenario, _progressPanel);
            _paidContentOpenedPanel.Init();
            _cameraController.Init();
            _giftController.Init();
            _shopController.Init();
            _giftsGiver.Init(_giftController, _shopController);
            _achievements.Init(_puzzleController, _giftsGiver, _dialogueController);
            _endPanel.Init(_dialogueController, _giftController, _achievements);
            TryStartNextStage();
        }

        private void TryStartNextStage()
        {
            _currentScenarioStage = _scenario.TryGetScenarioStage(_currentGameStageId);
            _currentGameStageId++;
            if (_currentScenarioStage == null || _isPaidContent && !GameData.PaidContent.IsUnlock(GameData.CurrentLevel))
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
                case "PaidContent":
                    _currentGameStage = _paidContentOpenedPanel;
                    _progressPanel.Hide(true);
                    StartNextStage();
                    _isPaidContent = true;
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

        public void LoadMenu()
        {
            SceneLoader.LoadMenu();
        }

        private void OnDestroy()
        {
            TryStopLastStage();
        }
    }
}