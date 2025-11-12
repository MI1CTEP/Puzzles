using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MyGame.Gameplay.Dialogue
{
    public sealed class PhraseAnim : MonoBehaviour
    {
        private Coroutine _cor;
        private WaitForSeconds _wait = new(0.07f);

        public UnityAction OnEndAnim { get; set; }

        public void Play(Text text, string value)
        {
            text.text = "";
            _cor = StartCoroutine(ShowText(text, value));
        }

        private IEnumerator ShowText(Text text, string value)
        {
            foreach (char c in value)
            {
                text.text += c;
                yield return _wait;
            }
            OnEndAnim?.Invoke();
        }
    }
}