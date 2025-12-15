using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace MyGame.Bundles
{
    public sealed class BundlesController : MonoBehaviour
    {
        private LevelsInfo _levelsInfo;
        private MainResourcesBundle _mainResourcesBundle;
        private ExtraImagesBundle _extraImagesBundle;
        private OnlyGameplayBundle _onlyGameplayBundle;
        private LevelDownloader _levelDownloader;
        private readonly string _url = "https://storage.yandexcloud.net/digitalmountains.coloring/puzzles/";

        public static BundlesController Instance { get; private set; }
        public int LevelsCount => _levelsInfo.LevelsCount;
        public int NotDownloadedCount => _levelsInfo.NotDownloadedCount;
        public int DownloadedCount => _levelDownloader.DownloadedCount;
        public int LastDownloadedLevel { get; set; }
        public int DownloadingLevel { get; set; }

        public MainResourcesBundle MainResourcesBundle => _mainResourcesBundle;
        public ExtraImagesBundle ExtraImagesBundle => _extraImagesBundle;
        public OnlyGameplayBundle OnlyGameplayBundle => _onlyGameplayBundle;

        public UnityAction<float> OnChangeLevelProgress { get; set; }
        public UnityAction<float> OnChangeAllProgress { get; set; }
        public UnityAction<int> OnStartDownloadLevel { get; set; }
        public UnityAction<int> OnEndDownloadLevel { get; set; }
        public UnityAction OnEndDownload { get; set; }

        public async UniTask Init(UnityAction onEndInit)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _levelsInfo = new(this, _url);
            _mainResourcesBundle = new();
            _mainResourcesBundle.Init(_levelsInfo);
            _extraImagesBundle = new();
            _extraImagesBundle.Init(_levelsInfo);
            _onlyGameplayBundle = new();
            _onlyGameplayBundle.Init(_levelsInfo);
            _levelDownloader = new(this, _levelsInfo, _url);
            await _levelsInfo.Init(onEndInit);
            await _levelDownloader.DownloadAllLevels();
        }

        public TypeLevelStatus TypeLevelStatus(int id) => _levelsInfo.Level(id).typeLevelStatus;
    }
}