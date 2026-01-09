using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

namespace MyGame.Bundles
{
    public sealed class ForCollectBundle : Bundle
    {
        public override string GetFileName(int id) => $"scenario_{id + 1}.for_collect";
        public bool IsHaveBundle => _bundle != null;

        public Sprite TryGetSprite()
        {
            if (_bundle == null)
                return null;

            return _bundle.LoadAsset<Sprite>("for_collect_image");
        }

        public async UniTask TryLoadLast(UnityAction onEnd)
        {
            int lastLevel = 0;
            for (int i = 0; i < _levelsInfo.LevelsCount; i++)
            {
                if (_levelsInfo.Level(i).type == "Extra")
                {
                    if(lastLevel < GameData.ExtraLevel.UnlockedLevels())
                    {
                        lastLevel++;
                        continue;
                    }
                    await Load(i, onEnd);
                    return;
                }
            }
            TryUnload();
        }

        protected override void LoadResources() { }
    }
}