using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Gameplay.Dialogue
{
    [System.Serializable]
    public sealed class SimpleDialogue
    {
        [SerializeField] private Phrase _firstPhrase;
        [SerializeField] private List<PhraseVariant> _phraseVariants;

        public Phrase FirstPhrase => _firstPhrase;
        public List<PhraseVariant> PhraseVariants => _phraseVariants;
    }

    [System.Serializable]
    public sealed class PhraseVariant
    {
        [SerializeField] private Phrase _answer;
        [SerializeField] private float _respect;
        [SerializeField] private Phrase _secondPhrase;

        public Phrase Answer => _answer;
        public float Respect => _respect;
        public Phrase SecondPhrase => _secondPhrase;
    }
}