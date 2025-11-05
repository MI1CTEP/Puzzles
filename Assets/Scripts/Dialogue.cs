using UnityEngine;

namespace MyGame
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Dialogue")]
    public sealed class Dialogue : ScriptableObject
    {
        public string firstPhrase;
    }
}