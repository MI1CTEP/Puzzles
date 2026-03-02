using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace MyGame
{
    public static class AsyncContent
    {
        public static Images Images { get; private set; }
        public static ExtraImages ExtraImages { get; private set; }
        public static LevelInfo LevelInfo { get; private set; }
        public static Videos Videos { get; private set; }
        public static Dialogues Dialogues { get; private set; }
        public static Sprite ForCollect { get; private set; }


        public static void LoadImages(UnityAction<Images> onEndLoad)
        {
            TryReleaseImages();
            string key = $"images_{GameData.CurrentLevel + 1}";
            Addressables.LoadAssetAsync<Images>(key).Completed += (asyncOperation) =>
            {
                Images = asyncOperation.Result;
                onEndLoad?.Invoke(Images);
            };
        }

        public static void TryReleaseImages()
        {
            if(Images != null)
            {
                Addressables.Release(Images);
                Images = null;
            }
        }

        public static void LoadExtraImages(UnityAction<ExtraImages> onEndLoad)
        {
            TryReleaseExtraImages();
            string key = $"extra_images_{GameData.CurrentLevel + 1}";
            Addressables.LoadAssetAsync<ExtraImages>(key).Completed += (asyncOperation) =>
            {
                ExtraImages = asyncOperation.Result;
                onEndLoad?.Invoke(ExtraImages);
            };
        }

        public static void TryReleaseExtraImages()
        {
            if (ExtraImages != null)
            {
                Addressables.Release(ExtraImages);
                ExtraImages = null;
            }
        }

        public static void LoadLevelInfo(UnityAction<LevelInfo> onEndLoad)
        {
            TryReleaseLevelInfo();
            string key = $"level_info_{GameData.CurrentLevel + 1}";
            Addressables.LoadAssetAsync<LevelInfo>(key).Completed += (asyncOperation) =>
            {
                LevelInfo = asyncOperation.Result;
                onEndLoad?.Invoke(LevelInfo);
            };
        }

        public static void TryReleaseLevelInfo()
        {
            if(LevelInfo != null)
            {
                Addressables.Release(LevelInfo);
                LevelInfo = null;
            }
        }

        public static void LoadVideos(UnityAction<Videos> onEndLoad)
        {
            TryReleaseVideos();
            string key = $"videos_{GameData.CurrentLevel + 1}";
            Addressables.LoadAssetAsync<Videos>(key).Completed += (asyncOperation) =>
            {
                Videos = asyncOperation.Result;
                onEndLoad?.Invoke(Videos);
            };
        }

        public static void TryReleaseVideos()
        {
            if (Videos != null)
            {
                Addressables.Release(Videos);
                Videos = null;
            }
        }

        public static void LoadDialogues(UnityAction<Dialogues> onEndLoad)
        {
            TryReleaseDialogues();
            string key = $"dialogues_{GameData.CurrentLevel + 1}";
            Addressables.LoadAssetAsync<Dialogues>(key).Completed += (asyncOperation) =>
            {
                Dialogues = asyncOperation.Result;
                onEndLoad?.Invoke(Dialogues);
            };
        }

        public static void TryReleaseDialogues()
        {
            if (Dialogues != null)
            {
                Addressables.Release(Dialogues);
                Dialogues = null;
            }
        }

        public static void LoadForCollect(UnityAction onEndLoad)
        {
            TryReleaseForCollect();
            string key = $"for_collect_image_9";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += (asyncOperation) =>
            {
                ForCollect = asyncOperation.Result;
                onEndLoad?.Invoke();
            };
        }

        public static void TryReleaseForCollect()
        {
            if (ForCollect != null)
            {
                Addressables.Release(ForCollect);
                ForCollect = null;
            }
        }
    }
}