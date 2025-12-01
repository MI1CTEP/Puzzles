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

        public UnityAction OnEndAnim { get; set; }

        public void Play(Text text)
        {
            _cor = StartCoroutine(ShowText(text));
        }

        private IEnumerator ShowText(Text text)
        {
            string clearText = text.text;
            string showText = "";
            string startShowColor = "<color=#323232>";
            string startClearColor = "<color=#00000000>";
            string endColor = "</color>";

            while (clearText.Length > 0)
            {
                text.text = startShowColor + showText + endColor + startClearColor + clearText + endColor;
                showText += clearText[0];
                clearText = clearText.Remove(0, 1);
                yield return _wait;
            }
            OnEndAnim?.Invoke();
        }

        private void OnDestroy()
        {
            if (_cor != null)
                StopCoroutine(_cor);
        }
    }
}