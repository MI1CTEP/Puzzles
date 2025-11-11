using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    [System.Serializable]
    public sealed class Dialogue
    {
        [SerializeField] private Phrase _firstPhrase;
        [SerializeField] private List<PhraseVariant> _phraseVariants;
    }

    [System.Serializable]
    public sealed class PhraseVariant
    {
        [SerializeField] private Phrase _answer;
        [SerializeField] private float _respect;
        [SerializeField] private Phrase _secondPhrase;
    }
}