using UnityEngine;

namespace MyGame
{
    [CreateAssetMenu(menuName = "ScriptableObjects/LevelInfo")]
    public sealed class LevelInfo : ScriptableObject
    {
        public string nameLevel;
        public Languages languages;
    }
}