using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MyGame.ContentDelivery
{
    public sealed class LevelsInfo
    {
        private ContentDeliveryController _controller;
        private Levels _savedLevels;
        private Levels _downloadedLevels;
        private string _savedLevelsJsonText;
        private string _dovnloadeLevelsJsonText;
        private readonly string _keyForSave = "AllLevelsData";
        private readonly string _url;

        public LevelsInfo(ContentDeliveryController controller, string url)
        {
            _controller = controller;
            _url = url;
            if (PlayerPrefs.HasKey(_keyForSave))
            {
                _savedLevelsJsonText = PlayerPrefs.GetString(_keyForSave);
                _savedLevels = JsonConvert.DeserializeObject<Levels>(_savedLevelsJsonText);
            }
            _controller.RunCoroutine(InitCor());
        }

        public void MarkLevelAsDownloaded(int id)
        {
            _downloadedLevels.levels[id].isNeedToDownload = false;
        }

        public void Save()
        {
            _dovnloadeLevelsJsonText = JsonConvert.SerializeObject(_downloadedLevels);
            PlayerPrefs.SetString(_keyForSave, _dovnloadeLevelsJsonText);
        }

        private IEnumerator InitCor()
        {
            yield return _controller.RunCoroutine(DownloadNewLevels());
            CheckVersions();
        }

        private IEnumerator DownloadNewLevels()
        {
            UnityWebRequest getWebRequest = UnityWebRequest.Get(_url + "levels.json");
            yield return getWebRequest.SendWebRequest();
            if (getWebRequest.result == UnityWebRequest.Result.Success)
            {
                _dovnloadeLevelsJsonText = getWebRequest.downloadHandler.text;
                _downloadedLevels = JsonConvert.DeserializeObject<Levels>(_dovnloadeLevelsJsonText);
            }
            else
            {
                _dovnloadeLevelsJsonText = _savedLevelsJsonText;
                _downloadedLevels = _savedLevels;
            }
        }

        private void CheckVersions()
        {
            for (int i = 0; i < _downloadedLevels.levels.Count; i++)
            {
                _downloadedLevels.levels[i].isNeedToDownload = false;
                if (_savedLevels == null || 
                    _savedLevels.levels.Count <= i || 
                    _savedLevels.levels[i].version != _downloadedLevels.levels[i].version ||
                    _savedLevels.levels[i].isNeedToDownload)
                {
                    _downloadedLevels.levels[i].isNeedToDownload = true;
                }
            }
        }
    }
}