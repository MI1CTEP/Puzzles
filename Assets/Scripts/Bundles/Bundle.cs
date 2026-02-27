using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

namespace MyGame.Bundles
{
    public abstract class Bundle
    {
        protected LevelsInfo _levelsInfo;
        protected AssetBundle _bundle;

        public void Init(LevelsInfo levelsInfo)
        {
            _levelsInfo = levelsInfo;
        }

        public abstract string GetFileName(int id);



        //public async UniTask Load(int id, UnityAction onEnd)
        //{
        //    TryUnload();

        //    CachedAssetBundle cached = new(GetFileName(id), Hash128.Compute(_levelsInfo.Level(id).version));


        //    UnityWebRequest uWR = UnityWebRequestAssetBundle.GetAssetBundle("", cached);

        //    await uWR.SendWebRequest();

        //    _bundle = DownloadHandlerAssetBundle.GetContent(uWR);

        //    LoadResources();

        //    if(onEnd != null)
        //        onEnd.Invoke();
        //}


        public async UniTask Load(int id, UnityAction onEnd)
        {
            //https://storage.yandexcloud.net/digitalmountains.coloring/puzzles/webGL/scenario_1/scenario_1.main_resources



            TryUnload();
            
            CachedAssetBundle cached = new(GetFileName(id), Hash128.Compute(_levelsInfo.Level(id).version));

            string url = $"https://storage.yandexcloud.net/digitalmountains.coloring/puzzles/webGL/scenario_{id + 1}/{GetFileName(id)}";

            UnityWebRequest uWR = UnityWebRequestAssetBundle.GetAssetBundle(url, cached);

            await uWR.SendWebRequest();

            if (uWR.result == UnityWebRequest.Result.Success)
            {
                _bundle = DownloadHandlerAssetBundle.GetContent(uWR);
                LoadResources();
                onEnd?.Invoke();
            }
        }


        public void TryUnload()
        {

            if (_bundle != null)
            {
                _bundle.Unload(true);
                _bundle = null;
            }
        }

        protected abstract void LoadResources(); 
    }
}