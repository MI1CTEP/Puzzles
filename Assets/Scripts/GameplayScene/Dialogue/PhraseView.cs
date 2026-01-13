using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Gameplay.Dialogue
{
    public class PhraseView : MonoBehaviour
    {
        [SerializeField] protected Text _text;

        protected RectTransform _rectTransform;

        protected void Init(Languages phrase)
        {
            _rectTransform = GetComponent<RectTransform>();
            //!!!����� ���������� ������ ����, �� ��� ��������!!!
            float preferredSize = _text.cachedTextGenerator.GetPreferredHeight(_text.text, _text.GetGenerationSettings(_text.rectTransform.rect.size));
            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, preferredSize * 1000 / Screen.width + 20);

            string currentLang = I2.Loc.LocalizationManager.CurrentLanguage;
                string text = currentLang switch
                {
                    "Russian" => phrase.ru,
                    "English" => phrase.en,
                    "German" => phrase.de,
                    "Chinese" => phrase.zh,
                    "French" => phrase.fr,
                    "Hindi" => phrase.hi,
                    "Italian" => phrase.it,
                    "Japanese" => phrase.ja,
                    "Portuguese" => phrase.pt,
                    "Spanish" => phrase.es,
                    _ => phrase.en
                };
            _text.text = text;
        }
    }
}