using UnityEngine;

namespace MyGame
{
    public static class GameData
    {
        public static int CurrentLevel { get; set; }

        public static class Respect
        {
            private static readonly string _key = "Respect";

            public static void Add(int value)
            {
                int currentValue = Load();
                PlayerPrefs.SetInt(_key, currentValue + value);
            }

            public static int Load() => PlayerPrefs.GetInt(_key);
        }

        public static class Sympathy
        {
            private static readonly string _key = "Sympathy_";

            public static void Save(int id, int value)
            {
                float currentValue = Load(id);
                if (currentValue < value)
                    PlayerPrefs.SetInt(_key + id.ToString(), value);
            }

            public static int Load(int id) => PlayerPrefs.GetInt(_key + id.ToString());
        }

        public static class Gifts
        {
            public static void AddValue(int groupId, int id, int value)
            {
                int currentValue = LoadValue(groupId, id);
                PlayerPrefs.SetInt(GetKey(groupId, id), currentValue + value);
            }

            public static int LoadValue(int groupId, int id) => PlayerPrefs.GetInt(GetKey(groupId, id));

            private static string GetKey(int groupId, int id) => $"QuantityGift_{groupId}_{id}";
        }

        public static class Dialogues
        {
            private static readonly string _key = "Dialogue";

            public static void Unlock(int levelId, int stepId, int positionId)
            {
                PlayerPrefs.SetInt($"{_key}_{levelId}_{stepId}_{positionId}", 1);
            }

            public static bool IsUnlock(int levelId, int dialogueId, int positionId)
            {
                return PlayerPrefs.GetInt($"{_key}_{levelId}_{dialogueId}_{positionId}") == 1;
            }
        }

        public static class ExtraLevel
        {
            public static Vector2Int PartSize { get; } = new(7, 9);
            public static float ChanceEasy { get; } = 1f;
            public static float ChanceMedium { get; } = 1f;
            public static float ChanceHard { get; } = 1f;

            private static readonly string _keyOpenedParts = "ExtraLevelOpenedParts";
            private static readonly string _keyPart = "ExtraLevelPart";
            private static readonly string _keyLevel = "ExtraLevel";

            public static void UnlockPart(int partId)
            {
                PlayerPrefs.SetInt($"{_keyPart}_{partId}", 1);
                int openedParts = GetOpenedPartsValue();
                PlayerPrefs.SetInt(_keyOpenedParts, openedParts + 1);
            }

            public static bool IsUnlockPart(int partId)
            {
                return PlayerPrefs.GetInt($"{_keyPart}_{partId}") == 1;
            }

            public static int GetOpenedPartsValue() => PlayerPrefs.GetInt(_keyOpenedParts);

            public static void UnlockLevel()
            {
                int unlockedLevel = UnlockedLevels();
                PlayerPrefs.SetInt(_keyLevel, unlockedLevel + 1);
                for (int i = 0; ; i++)
                {
                    if (PlayerPrefs.HasKey($"{_keyPart}_{i}"))
                        PlayerPrefs.SetInt($"{_keyPart}_{i}", 0);
                    else break;
                }
            }

            public static int UnlockedLevels() => PlayerPrefs.GetInt(_keyLevel);
        }

        public static class Achievements
        {
            private static readonly string _key = "Achievement";

            public static void Save(int levelId, int id)
            {
                PlayerPrefs.SetInt($"{_key}_{levelId}_{id}", 1);
            }

            public static bool IsUnlock(int levelId, int id)
            {
                return PlayerPrefs.GetInt($"{_key}_{levelId}_{id}") == 1;
            }
        }

        public static class PaidContent
        {
            private static readonly string _key = "PaidContent";

            public static void Save(int id)
            {
                PlayerPrefs.SetInt($"{_key}_{id}", 1);
            }

            public static bool IsUnlock(int id)
            {
                return PlayerPrefs.GetInt($"{_key}_{id}") == 1;
            }
        }

        public static class Levels
        {
            private static readonly string _key = "Level";
            private static readonly string _keyAll = "LevelsAll";

            public static void SetOpened(int id)
            {
                PlayerPrefs.SetInt($"{_key}_{id}", 1);
            }

            public static bool IsOpened(int id)
            {
                return PlayerPrefs.GetInt($"{_key}_{id}") == 1;
            }

            public static void OpenAll()
            {
                PlayerPrefs.SetInt(_keyAll, 1);
            }

            public static bool IsOpenedAll()
            {
                return PlayerPrefs.GetInt(_keyAll) == 1;
            }
        }
    }
}