using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    [CreateAssetMenu(menuName = "ScriptableObjects/SimpleDialogue")]
    public sealed class SimpleDialogue : ScriptableObject
    {
        public Languages firstPhrase;
        public List<PhraseVariant> phraseVariants;
        public int id;
    }

    [System.Serializable]
    public sealed class PhraseVariant
    {
        public Languages answer;
        public int respect;
    }
}