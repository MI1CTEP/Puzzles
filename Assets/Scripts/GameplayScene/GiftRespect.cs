using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MyGame.Gameplay
{
    public sealed class GiftRespect : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void SetRespect(int value)
        {
            _text.text = $"+{value}";
        }
    }
}