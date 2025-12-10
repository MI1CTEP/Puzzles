using System.Collections;
using UnityEngine;

namespace MyGame.ContentDelivery
{
    public sealed class ContentDeliveryController : MonoBehaviour
    {
        private LevelsInfo _levelsInfo;
        private LevelDownloader _levelDownloader;
        private readonly string _url = "https://storage.yandexcloud.net/digitalmountains.coloring/puzzles/";

        public string GetBundleNameSprites(int id) => $"sprites_{id}.bundle";
        public string GetBundleNameVideos(int id) => $"videos_{id}.bundle";

        private void Start()
        {
            _levelsInfo = new(this, _url);
            _levelDownloader = new(this, _levelsInfo, _url);
        }

        public Coroutine RunCoroutine(IEnumerator coroutine) => StartCoroutine(coroutine);

        public bool IsBundleCached(string nameBundle)
        {
            CachedAssetBundle cached = new()
            {
                name = nameBundle
            };

            return Caching.IsVersionCached(cached);
        }

        public void DeleteBundle(string nameBundle)
        {
            CachedAssetBundle cached = new()
            {
                name = nameBundle
            };

            if (Caching.IsVersionCached(cached))
                Caching.ClearAllCachedVersions(cached.name);
        }
    }
}