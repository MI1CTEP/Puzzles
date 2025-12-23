using UnityEngine;

namespace MyGame
{
    public static class GameData
    {
        public static int CurrentLevel { get; set; }

        public static class Score
        {
            private static readonly string _key = "Score";

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

        public static class Details
        {
            public static float chanceEasy = 0.1f;
            public static float chanceMedium = 0.2f;
            public static float chanceHard = 0.5f;

            private static readonly string _key = "Detail";

            public static void Unlock(int id)
            {
                PlayerPrefs.SetInt($"{_key}_{id}", 1);
            }

            public static bool IsUnlock(int id)
            {
                return PlayerPrefs.GetInt($"{_key}_{id}") == 1;
            }
        }

        public static class Achievements
        {
            private static readonly string _key = "Achievement";

            public static void Save(int id)
            {
                PlayerPrefs.SetInt($"{_key}_{id}", 1);
            }

            public static bool IsUnlock(int id)
            {
                return PlayerPrefs.GetInt($"{_key}_{id}") == 1;
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
    }
}