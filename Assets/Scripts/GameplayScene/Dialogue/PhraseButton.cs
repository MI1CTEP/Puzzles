using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MyGame.Gameplay.Dialogue
{
    public sealed class PhraseButton : PhraseView
    {
        [SerializeField] private Button _button;

        public void Init(Phrase phrase, ref float anchorPositionY, UnityAction onClick)
        {
            Init(phrase);

            _rectTransform.anchoredPosition = new Vector2(0, anchorPositionY);
            anchorPositionY += _rectTransform.sizeDelta.y;

            _button.onClick.AddListener(onClick);
        }
    }
}