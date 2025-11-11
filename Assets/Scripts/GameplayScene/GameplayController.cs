using UnityEngine;
using MyGame.Gameplay.Puzzle;

namespace MyGame.Gameplay
{
    public sealed class GameplayController : MonoBehaviour
    {
        [SerializeField] private Scenario _scenario;
        [SerializeField] private VideoController _videoController;
        [SerializeField] private PuzzleController _puzzleController;
        [SerializeField] private BackgroundController _backgroundController;
        [SerializeField] private CameraController _cameraController;

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
            _videoController.Init();
            _puzzleController.Init();
            _backgroundController.Init();
            _cameraController.Init();
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
                case TypeStage.SetSprite:
                    _cameraController.UpdateSize(_currentScenarioStage.Sprite.texture.width);
                    _currentGameStage = _backgroundController;
                    StartNextStage();
                    break;
                case TypeStage.SetPuzzle:
                    _currentGameStage = _puzzleController;
                    StartNextStage();
                    break;
                case TypeStage.SetVideo:
                    _currentGameStage = _videoController;
                    StartNextStage();
                    break;
                case TypeStage.SetDialogue:
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
            Debug.Log("FINISH");
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