using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Events;

namespace MyGame.Gameplay
{
    public sealed class VideoController : MonoBehaviour, IGameStage
    {
        [SerializeField] private GameObject _buttons;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _continueButton;

        private ScenarioStage _scenarioStage;
        private VideoPlayer _videoPlayer;

        public UnityAction OnEnd { get; set; }

        public void Init()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
            _videoPlayer.loopPointReached += OnVideoFinished;
            _restartButton.onClick.AddListener(() => Play(_scenarioStage));
            _continueButton.onClick.AddListener(End);
        }

        public void Play(ScenarioStage scenarioStage)
        {
            if (scenarioStage == null)
                return;

            _scenarioStage = scenarioStage;
            _buttons.SetActive(false);
            gameObject.SetActive(true);
            _videoPlayer.clip = scenarioStage.Video;
            _videoPlayer.Play();
        }

        private void OnVideoFinished(VideoPlayer vp)
        {
            _buttons.SetActive(true);
        }

        private void End()
        {
            _buttons.SetActive(false);
            gameObject.SetActive(false);
            OnEnd?.Invoke();
        }

        private void OnDestroy()
        {
            if (_videoPlayer != null)
                _videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}