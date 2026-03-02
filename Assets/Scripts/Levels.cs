using UnityEngine;

namespace MyGame
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Levels")]
    public sealed class Levels : ScriptableObject
    {
        public Level[] levels;
    }

    [System.Serializable]
    public sealed class Level
    {
        public TypeLevel typeLevel;
        public int price;
    }

    public enum TypeLevel
    {
        Free, Extra
    }
}