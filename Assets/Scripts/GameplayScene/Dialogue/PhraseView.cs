using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Gameplay.Dialogue
{
    public class PhraseView : MonoBehaviour
    {
        [SerializeField] protected Text _text;

        protected RectTransform _rectTransform;

        protected void Init(Phrase phrase)
        {
            _rectTransform = GetComponent<RectTransform>();
            _text.text = phrase.ru;

            float preferredSize = _text.cachedTextGenerator.GetPreferredHeight(_text.text, _text.GetGenerationSettings(_text.rectTransform.rect.size));
            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, preferredSize);
        }
    }
}