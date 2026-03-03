using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;
using static System.Net.WebRequestMethods;

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


            //OnPlayVideo(scenarioStage).Forget();


           // Debug.Log(scenarioStage.videoId);

            _scenarioStage = scenarioStage;
            _buttons.SetActive(false);
            gameObject.SetActive(true);

            if (NutakuAPIInitializator.instance.TypePlatform == TypePlatform.WebGL || NutakuAPIInitializator.instance.TypePlatform == TypePlatform.Editor)
            {
                _videoPlayer.source = VideoSource.Url;
            //string pathClip = $"{Application.streamingAssetsPath}/Videos/scenario_{GameData.CurrentLevel + 1}/video_{_scenarioStage.videoId}.mp4";
           // https://api.tetragon-games.org/
                string pathClip = $"https://api.tetragon-games.org/webgl/StreamingAssets/Videos/scenario_{GameData.CurrentLevel + 1}/video_{_scenarioStage.videoId}.mp4";
                //Debug.Log(pathClip);
                _videoPlayer.url = pathClip;
            }
            else
            {
                _videoPlayer.source = VideoSource.Url;
                _videoPlayer.clip = scenarioStage.Video;
            }


            //_videoPlayer.clip = scenarioStage.Video;
            _videoPlayer.Play();
        }

        //private async UniTask OnPlayVideo(ScenarioStage scenarioStage)
        //{
        //    Debug.Log(scenarioStage.videoId);

        //    _scenarioStage = scenarioStage;
        //    _buttons.SetActive(false);
        //    gameObject.SetActive(true);

        //    if (NutakuAPIInitializator.instance.TypePlatform == TypePlatform.WebGL || NutakuAPIInitializator.instance.TypePlatform == TypePlatform.Editor)
        //    {
        //        _videoPlayer.source = VideoSource.Url;
        //        string pathClip = "";
        //        _videoPlayer.url = pathClip;
        //    }
        //    else
        //    {
        //        _videoPlayer.source = VideoSource.Url;
        //        _videoPlayer.clip = scenarioStage.Video;
        //    }


        //    //_videoPlayer.clip = scenarioStage.Video;

        //    _videoPlayer.Play();
        //}


        public static async UniTask<VideoClip> LoadVideoClipAsync(int videoId)
        {
            string path = $"Videos/scenario_1/video_{videoId}";
            ResourceRequest request = Resources.LoadAsync<VideoClip>(path);

            await UniTask.WaitUntil(() => request.isDone);

            VideoClip clip = request.asset as VideoClip;

            if (clip == null)
                Debug.LogError($"Video not found at path: {path}");

            return clip;
        }


        public void Disable()
        {
            gameObject.SetActive(false);
        }

        private void OnVideoFinished(VideoPlayer vp)
        {
            _buttons.SetActive(true);
        }

        private void End()
        {
            _buttons.SetActive(false);
            OnEnd?.Invoke();
        }

        private void OnDestroy()
        {
            if (_videoPlayer != null)
                _videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}