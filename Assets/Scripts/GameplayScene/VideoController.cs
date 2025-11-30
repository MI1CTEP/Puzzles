using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

namespace MyGame.Gameplay
{
    public sealed class VideoController : MonoBehaviour, IGameStage
    {
        private VideoPlayer _videoPlayer;

        public UnityAction OnEnd { get; set; }

        public void Init()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
            _videoPlayer.loopPointReached += OnVideoFinished;
        }

        public void Play(ScenarioStage scenarioStage)
        {
            gameObject.SetActive(true);
            _videoPlayer.clip = scenarioStage.Video;
            _videoPlayer.Play();
        }

        private void OnVideoFinished(VideoPlayer vp)
        {
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