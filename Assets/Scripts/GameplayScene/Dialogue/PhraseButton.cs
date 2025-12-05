using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MyGame.Gameplay.Dialogue
{
    public sealed class PhraseButton : PhraseView
    {
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _valueObject;
        [SerializeField] private Text _valueText;

        public void Init(Phrase phrase, ref float anchorPositionY, UnityAction onClick)
        {
            Init(phrase);

            _rectTransform.anchoredPosition = new Vector2(0, anchorPositionY);
            anchorPositionY += _rectTransform.sizeDelta.y;

            _button.onClick.AddListener(onClick);
        }

        public void TryShowValue(int dialogueId, int positionId, int value)
        {
            if(GameData.Dialogue.IsUnlock(GameData.CurrentLevel, dialogueId, positionId))
            {
                _valueObject.SetActive(true);
                _valueText.text = $"+{value}";
            }
            else
            {
                _valueObject.SetActive(false);
            }
        }
    }
}