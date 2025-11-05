using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;
using Zenject;

namespace MyGame
{
    public sealed class VideoController : MonoBehaviour
    {
        private VideoPlayer _videoPlayer;

        public UnityAction OnEnd { get; set; }

        [Inject]
        private void Initialize()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
            _videoPlayer.loopPointReached += OnVideoFinished;
        }

        public void SetVideoClip(VideoClip videoClip)
        {
            _videoPlayer.clip = videoClip;
        }

        public void Play()
        {
            _videoPlayer.Play();
        }

        private void OnVideoFinished(VideoPlayer vp)
        {
            OnEnd?.Invoke();
        }

        private void OnDestroy()
        {
            if (_videoPlayer != null)
                _videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}