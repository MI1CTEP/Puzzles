using UnityEngine;

namespace MyGame
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Phrase")]
    public sealed class Phrase : ScriptableObject
    {
        [SerializeField, TextArea(1, 10)] private string _rus;
        [SerializeField, TextArea(1, 10)] private string _eng;
    }
}