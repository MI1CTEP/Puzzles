using UnityEngine;

namespace MyGame
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Images")]
    public sealed class Images : ScriptableObject
    {
        public Sprite[] sprites;
    }
}