using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MyGame.Gameplay.Dialogue
{
    public sealed class PhraseAnim : MonoBehaviour
    {
        private Coroutine _cor;
        private readonly WaitForSeconds _wait = new(0.05f);
        private Text _text;
        private string _fullText;
        private bool _isPlaingAnim;

        public UnityAction OnEndAnim { get; set; }

        public void Play(Text text)
        {
            _isPlaingAnim = true;
            _text = text;
            _cor = StartCoroutine(ShowText());
        }

        public void TrySkipAnim()
        {
            if (!_isPlaingAnim) return;

            if (_cor != null)
                StopCoroutine(_cor);
            _text.text = _fullText;
            _isPlaingAnim = false;
            OnEndAnim?.Invoke();
        }

        private IEnumerator ShowText()
        {
            _fullText = _text.text;
            string clearText = _fullText;
            string showText = "";
            string startShowColor = "<color=#ffffffff>";
            string startClearColor = "<color=#00000000>";
            string endColor = "</color>";

            while (clearText.Length > 0)
            {
                showText += clearText[0];
                clearText = clearText.Remove(0, 1);
                _text.text = startShowColor + showText + endColor + startClearColor + clearText + endColor;
                yield return _wait;
            }
            _isPlaingAnim = false;
            OnEndAnim?.Invoke();
        }

        private void OnDestroy()
        {
            if (_cor != null)
                StopCoroutine(_cor);
        }
    }
}