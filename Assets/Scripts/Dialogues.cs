using UnityEngine;

namespace MyGame
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Dialogues")]
    public class Dialogues : ScriptableObject
    {
        public SimpleDialogue[] simpleDialogues;
    }
}