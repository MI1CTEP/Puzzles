using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Gameplay
{
    public sealed class GiftRespect : MonoBehaviour
    {
        [SerializeField] private Text _text;

        public void SetRespect(int value)
        {
            _text.text = $"+{value}";
        }
    }
}