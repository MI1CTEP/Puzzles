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

        public static class Dialogue
        {
            private static readonly string _key = "Dialogue_";

            public static void Unlock(int levelId, int stepId, int positionId)
            {
                PlayerPrefs.SetInt($"{_key}_{levelId}_{stepId}_{positionId}", 1);
            }

            public static bool IsUnlock(int levelId, int dialogueId, int positionId)
            {
                return PlayerPrefs.GetInt($"{_key}_{levelId}_{dialogueId}_{positionId}") == 1;
            }
        }
    }
}