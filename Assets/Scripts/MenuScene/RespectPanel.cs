using UnityEngine;
using TMPro;

namespace MyGame.Menu
{
    public sealed class RespectPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _valueText;

        public void Init()
        {
            UpdateView();
        }

        public void UpdateView()
        {
            _valueText.text = GameData.Respect.Load().ToString();
        }
    }
}