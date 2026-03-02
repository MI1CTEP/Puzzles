using UnityEngine;

namespace MyGame
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ExtraImages")]
    public sealed class ExtraImages : ScriptableObject
    {
        public Sprite[] sprites;
    }
}