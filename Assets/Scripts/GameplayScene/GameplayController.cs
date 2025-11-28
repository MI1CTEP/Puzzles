using UnityEngine;
using MyGame.Gameplay.Puzzle;
using MyGame.Gameplay.Dialogue;
using MyGame.Gifts;

namespace MyGame.Gameplay
{
    public sealed class GameplayController : MonoBehaviour
    {
        [SerializeField] private Scenario _scenario;
        [SerializeField] private ScaleOfSympathy _scaleOfSympathy;
        [SerializeField] private VideoController _videoController;
        [SerializeField] private PuzzleController _puzzleController;
        [SerializeField] private BackgroundController _backgroundController;
        [SerializeField] private DialogueController _dialogueController;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private GiftController _giftController;
        [SerializeField] private GiftsGiver _giftsGiver;

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

            switch (_currentScenarioStage.TypeStage)
            {
                case TypeStage.SetPuzzle:
                    _currentGameStage = _puzzleController;
                    _scaleOfSympathy.Hide();
                    _cameraController.UpdateSize(_currentScenarioStage.Sprite.texture.width);
                    _backgroundController.SetSprite(_currentScenarioStage.Sprite, _currentScenarioStage.IsAnim);
                    StartNextStage();
                    break;
                case TypeStage.SetVideo:
                    _currentGameStage = _videoController;
                    _scaleOfSympathy.Hide();
                    StartNextStage();
                    break;
                case TypeStage.SetDialogue:
                    _currentGameStage = _dialogueController;
                    _scaleOfSympathy.Show();
                    _cameraController.UpdateSize(_currentScenarioStage.Sprite.texture.width);
                    _backgroundController.SetSprite(_currentScenarioStage.Sprite, _currentScenarioStage.IsAnim);
                    StartNextStage();
                    break;
                case TypeStage.SetGiftsGiver:
                    _currentGameStage = _giftsGiver;
                    _scaleOfSympathy.Hide();
                    _cameraController.UpdateSize(_currentScenarioStage.Sprite.texture.width);
                    _backgroundController.SetSprite(_currentScenarioStage.Sprite, _currentScenarioStage.IsAnim);
                    StartNextStage();
                    break;
                default:
                    End();
                    break;
            }
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