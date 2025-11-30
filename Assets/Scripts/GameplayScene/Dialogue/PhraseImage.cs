using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MyGame.Gameplay.Dialogue
{
    public sealed class PhraseImage : PhraseView
    {
        [SerializeField] private Image _background;
        [SerializeField] private Color _colorForPlayer;
        [SerializeField] private Color _colorForCharacter;

        private UnityAction _onSetPhrase;
        private PhraseAnim _anim;

        public void Init(Phrase phrase, bool isPlayerPhrase, UnityAction onSetPhrase)
        {
            Init(phrase);

            _onSetPhrase = onSetPhrase;


            if (isPlayerPhrase)
            {
                _background.transform.localPosition += Vector3.right * 100;
                _background.color = _colorForPlayer;
                SetPhrase();
            }
            else
            {
                _background.transform.localPosition += Vector3.left * 100;
                _background.color = _colorForCharacter;
                _anim = gameObject.AddComponent<PhraseAnim>();
                _anim.Play(_text, phrase.ru);
                _anim.OnEndAnim += SetPhrase;
            }
        }

        private void SetPhrase()
        {
            _onSetPhrase?.Invoke();
        }

        private void OnDestroy()
        {
            if (_anim != null)
                _anim.OnEndAnim += SetPhrase;
        }
    }
}