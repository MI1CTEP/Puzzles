using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

namespace MyGame.Bundles
{
    public abstract class Bundle
    {
        private LevelsInfo _levelsInfo;
        protected AssetBundle _bundle;

        public void Init(LevelsInfo levelsInfo)
        {
            _levelsInfo = levelsInfo;
        }

        public abstract string GetFileName(int id);

        public async UniTask Load(int id, UnityAction onEnd)
        {
            TryUnload();
            CachedAssetBundle cached = new(GetFileName(id), Hash128.Compute(_levelsInfo.Level(id).version));
            UnityWebRequest uWR = UnityWebRequestAssetBundle.GetAssetBundle("", cached);
            await uWR.SendWebRequest();
            _bundle = DownloadHandlerAssetBundle.GetContent(uWR);
            LoadResources();
            onEnd?.Invoke();
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