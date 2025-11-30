using System.Collections.Generic;

namespace MyGame.Gameplay.Dialogue
{
    public sealed class SimpleDialogue
    {
        public Phrase firstPhrase;
        public List<PhraseVariant> phraseVariants;
    }

    [System.Serializable]
    public sealed class PhraseVariant
    {
        public Phrase answer;
        public float respect;
        public Phrase secondPhrase;
    }

    public sealed class Phrase
    {
        public string ru;
        public string en;
        public string de;
        public string zh;
        public string fr;
        public string hi;
        public string it;
        public string ja;
        public string pt;
    }
}