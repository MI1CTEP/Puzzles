using System.Collections.Generic;

namespace MyGame.Gameplay.Dialogue
{
    public sealed class SimpleDialogue
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
        public Languages secondPhrase;
    }
}