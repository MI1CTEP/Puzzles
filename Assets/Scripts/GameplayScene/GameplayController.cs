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
        [SerializeField] private Scenario _scenario;
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
        [SerializeField] private GameObject _continueButtotAfterVideo;

        private ScenarioStage _scenarioStage;
        private IGameStage _currentGameStage;
        private int _currentGameStageId;
        private bool _isPaidContent = false;

        private void Start()
        {
            AsyncContent.LoadVideos((videos) =>
            {
                AsyncContent.LoadDialogues((dialogues) =>
                {
                    Init();
                });
            });
        }

        private void Init()
        {
            if (GameData.CurrentPuzzleStep > 0)
            {
                _continueButtotAfterVideo.SetActive(false);
                int currentStage = 0;
                for (int i = 0; ; i++)
                {
                    _scenarioStage = _scenario.TryGetScenarioStage(i);
                    if (_scenarioStage.typeStage == TypeStage.Puzzle)
                        currentStage++;
                    if (currentStage == GameData.CurrentPuzzleStep)
                    {
                        GameData.CurrentPuzzleStep--;
                        _currentGameStageId = i;
                        break;
                    }
                }
            }

            _progressPanel.Init();
            _videoController.Init();
            _extraLevelUnlocker.Init();
            _puzzleController.Init(_progressPanel, _extraLevelUnlocker);
            _dialogueController.Init(_progressPanel);
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
            _scenarioStage = _scenario.TryGetScenarioStage(_currentGameStageId);
            _currentGameStageId++;
            if (_scenarioStage == null || _isPaidContent && !GameData.PaidContent.IsUnlock(GameData.CurrentLevel))
            {
                End();
                return;
            }

            TryStopLastStage();

            switch (_scenarioStage.typeStage)
            {
                case TypeStage.Puzzle:

                    //Debug.Log($"Puzzle {_currentGameStageId} {GameData.CurrentStep}");
                    //Debug.Log($"dialogueId  {_currentScenarioStage.dialogueId}");
                    GameData.CurrentPuzzleStep++;
                    GameData.StageGirlLevel.UnlockStage(GameData.CurrentLevel, GameData.CurrentPuzzleStep);

                    _videoController.Disable();
                    _currentGameStage = _puzzleController;
                    _progressPanel.Hide(true);
                    UpdateCameraSize();
                    SetBackgroundImage();
                    StartNextStage();
                    break;
                case TypeStage.Video:
                    _currentGameStage = _videoController;
                    _progressPanel.Hide(true);
                    StartNextStage();
                    break;
                case TypeStage.Dialogue:
                    _videoController.Disable();
                    _currentGameStage = _dialogueController;
                    _progressPanel.Show(true);
                    UpdateCameraSize();
                    SetBackgroundImage();
                    StartNextStage();
                    break;
                case TypeStage.Gifts:
                    _videoController.Disable();
                    _currentGameStage = _giftsGiver;
                    _progressPanel.Hide(true);
                    UpdateCameraSize();
                    SetBackgroundImage();
                    StartNextStage();
                    break;
                case TypeStage.PaidContent:

                    Debug.Log("PaidContent");
                    //GameData.StageGirlLevel.UnlockStage(GameData.CurrentLevel, GameData.CurrentStep);


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
            _cameraController.UpdateSize(AsyncContent.Images.sprites[0].texture.height, AsyncContent.Images.sprites[0].texture.width);
        }

        private void SetBackgroundImage()
        {
            _backgroundController.SetSprite(AsyncContent.Images.sprites[_scenarioStage.imageId - 1], false);
        }

        private void StartNextStage()
        {
            _currentGameStage.OnEnd += TryStartNextStage;
            _currentGameStage.Play(_scenarioStage);
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
            AsyncContent.TryReleaseDialogues();
            AsyncContent.TryReleaseVideos();
            TryStopLastStage();
        }
    }
}