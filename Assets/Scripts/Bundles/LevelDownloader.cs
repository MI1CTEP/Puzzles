using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MyGame.Bundles
{
    public sealed class LevelDownloader
    {
        private WaitForSeconds _waitDeleteOldVersion = new(0.1f);
        private readonly BundlesController _controller;
        private readonly LevelsInfo _levelsInfo;
        private readonly string _url;
        private int _downloadedCount;

        public int DownloadedCount => _downloadedCount;

        public LevelDownloader(BundlesController controller, LevelsInfo levelsInfo, string url)
        {
            _controller = controller;
            _levelsInfo = levelsInfo;
            _url = url;
        }

        public async UniTask DownloadAllLevels()
        {
            int levelId = 0;
            for (int i = 0; i < _levelsInfo.NotDownloadedCount; i++)
            {
                while (_levelsInfo.Level(levelId).typeLevelStatus == TypeLevelStatus.Downloaded)
                    levelId++;
                await DownloadLevel(levelId);
            }
            _controller.OnEndDownload?.Invoke();
        }

        private async UniTask DownloadLevel(int id)
        {
            _controller.OnStartDownloadLevel?.Invoke(id);
            _controller.DownloadingLevel = id;
            _levelsInfo.SetLevelStatus(TypeLevelStatus.Downloading, id);
            UpdateProgress(0);
            await DownloadBundle(_controller.MainResourcesBundle.GetFileName(id), id, 0f, 0.2f);
            await DownloadBundle(_controller.ExtraImagesBundle.GetFileName(id), id, 0.2f, 0.3f);
            await DownloadBundle(_controller.OnlyGameplayBundle.GetFileName(id), id, 0.5f, 0.5f);
            if (_levelsInfo.Level(id).type == "Extra")
                await DownloadBundle(_controller.ForCollectBundle.GetFileName(id), id, 1, 0);
            _levelsInfo.SetLevelStatus(TypeLevelStatus.Downloaded, id);
            _downloadedCount++;
            _controller.OnEndDownloadLevel?.Invoke(id);
            _controller.LastDownloadedLevel = id;
        }

        private async UniTask DownloadBundle(string bundleName, int id, float startProgress = 0, float maxProgress = 0)
        {
            string pathToBundle = _url + $"android/scenario_{id + 1}/" + bundleName;

            CachedAssetBundle cached = new(bundleName, Hash128.Compute(_levelsInfo.Level(id).version));

            UnityWebRequest uWR = UnityWebRequestAssetBundle.GetAssetBundle(pathToBundle, cached);
            UnityWebRequestAsyncOperation operation = uWR.SendWebRequest();

            while (!operation.isDone)
            {
                UpdateProgress(operation.progress * maxProgress + startProgress);
                await UniTask.DelayFrame(1);
            }

            if (uWR.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Ошибка загрузки бандла: {bundleName} для уровня {id}. Ошибка: {uWR.error}");
                uWR.Dispose();
                return;
            }

            UpdateProgress(maxProgress + startProgress);

            AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(uWR);
            uWR.Dispose();
            assetBundle.Unload(true);
        }

        private void UpdateProgress(float value)
        {
            _controller.OnChangeLevelProgress?.Invoke(value);
            value = (_downloadedCount + value) / _levelsInfo.NotDownloadedCount;
            _controller.OnChangeAllProgress?.Invoke(value);
        }
    }
}