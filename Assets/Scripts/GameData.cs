using UnityEngine;

namespace MyGame
{
    public static class GameData
    {
        public static class Score
        {
            private static string _key = "Score";

            public static void Add(int value)
            {
                int currentValue = Load();
                PlayerPrefs.SetInt(_key, currentValue + value);
            }

            public static int Load() => PlayerPrefs.GetInt(_key);
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
    }
}