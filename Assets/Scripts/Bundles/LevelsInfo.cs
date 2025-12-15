using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace MyGame.Bundles
{
    public sealed class LevelsInfo
    {
        private BundlesController _controller;
        private Levels _savedLevels;
        private Levels _downloadedLevels;
        private List<int> _levelsForDelete;
        private string _savedLevelsJsonText;
        private string _dovnloadeLevelsJsonText;
        private readonly string _keyForSave = "AllLevelsData";
        private readonly string _url;
        private int _notDownloadedCount;

        public List<int> LevelsForDelete => _levelsForDelete;
        public Level Level(int id) => _downloadedLevels.levels[id];
        public int LevelsCount => _downloadedLevels.levels.Count;
        public int NotDownloadedCount => _notDownloadedCount;

        public LevelsInfo(BundlesController controller, string url)
        {
            _levelsForDelete = new();
            _controller = controller;
            _url = url;
            if (PlayerPrefs.HasKey(_keyForSave))
            {
                _savedLevelsJsonText = PlayerPrefs.GetString(_keyForSave);
                _savedLevels = JsonConvert.DeserializeObject<Levels>(_savedLevelsJsonText);
            }
        }

        public async UniTask Init(UnityAction onEndInit)
        {
            await DownloadNewLevels();
            CheckVersions();
            onEndInit?.Invoke();
        }

        public void SetLevelStatus(TypeLevelStatus typeLevelStatus, int id)
        {
            _downloadedLevels.levels[id].typeLevelStatus = typeLevelStatus;
        }

        private async UniTask DownloadNewLevels()
        {
            UnityWebRequest uWR = UnityWebRequest.Get(_url + "levels.json");
            await uWR.SendWebRequest();
            if (uWR.result == UnityWebRequest.Result.Success)
            {
                _dovnloadeLevelsJsonText = uWR.downloadHandler.text;
                _downloadedLevels = JsonConvert.DeserializeObject<Levels>(_dovnloadeLevelsJsonText);
                Save();
            }
            else
            {
                _dovnloadeLevelsJsonText = _savedLevelsJsonText;
                _downloadedLevels = _savedLevels;
            }
        }

        private void Save()
        {
            _dovnloadeLevelsJsonText = JsonConvert.SerializeObject(_downloadedLevels);
            PlayerPrefs.SetString(_keyForSave, _dovnloadeLevelsJsonText);
        }

        private void CheckVersions()
        {
            for (int i = 0; i < _downloadedLevels.levels.Count; i++)
            {
                SetLevelStatus(TypeLevelStatus.Downloaded, i);
                if (_savedLevels == null || 
                    _savedLevels.levels.Count <= i || 
                    IsNeedToDownloadLevel(i, _downloadedLevels.levels[i].version))
                {
                    _levelsForDelete.Add(i);
                    SetLevelStatus(TypeLevelStatus.NotDownloaded, i);
                    _notDownloadedCount++;
                }
            }
        }

        private bool IsNeedToDownloadLevel(int id, int version)
        {
            if (IsBundleCached(_controller.MainResourcesBundle.GetFileName(id), version) == false)
                return true;
            if (IsBundleCached(_controller.ExtraImagesBundle.GetFileName(id), version) == false)
                return true;
            if (IsBundleCached(_controller.OnlyGameplayBundle.GetFileName(id), version) == false)
                return true;

            return false;
        }

        private bool IsBundleCached(string bundleName, int version)
        {
            CachedAssetBundle cached = new(bundleName, Hash128.Compute(version));
            return Caching.IsVersionCached(cached);
        }
    }
}