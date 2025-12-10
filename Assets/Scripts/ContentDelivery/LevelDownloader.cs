using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace MyGame.ContentDelivery
{
    public sealed class LevelDownloader
    {
        private ContentDeliveryController _conrtoller;
        private LevelsInfo _levelsInfo;
        private readonly string _url;

        public LevelDownloader(ContentDeliveryController conrtoller, LevelsInfo levelsInfo, string url)
        {
            _conrtoller = conrtoller;
            _levelsInfo = levelsInfo;
            _url = url;
        }

        public void Download(int id, UnityAction<float> onProgress, UnityAction onEnd)
        {
            _conrtoller.RunCoroutine(DownloadCor(id, onProgress, onEnd));
        }

        private IEnumerator DownloadCor(int id, UnityAction<float> onProgress, UnityAction onEnd)
        {
            yield return _conrtoller.RunCoroutine(DownloadBundle(_conrtoller.GetBundleNameSprites(id), id, onProgress, 0f));
            yield return _conrtoller.RunCoroutine(DownloadBundle(_conrtoller.GetBundleNameVideos(id), id, onProgress, 0.5f));
            onEnd?.Invoke();
            _levelsInfo.MarkLevelAsDownloaded(id);
        }

        private IEnumerator DownloadBundle(string bundleName, int id, UnityAction<float> onProgress, float startProgress)
        {
            string pathToBundle = _url + $"android/scenario_{id}/" + bundleName;

            CachedAssetBundle cached = new()
            {
                name = bundleName
            };

            UnityWebRequest uWR = UnityWebRequestAssetBundle.GetAssetBundle(pathToBundle, cached);
            UnityWebRequestAsyncOperation operation = uWR.SendWebRequest();

            while (!operation.isDone)
            {
                onProgress?.Invoke(operation.progress / 2f + startProgress);
                yield return null;
            }
            onProgress?.Invoke(0.5f + startProgress);

            AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(uWR);
            assetBundle.Unload(true);
        }
    }
}