using UnityEngine;
using TMPro;

namespace MyGame.Menu
{
    public sealed class UpInfoPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        private int _levelsCount;

        public void Init(int levelsCount)
        {
            _levelsCount = levelsCount;
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void UpdateText(int currentLevel, string mainInfo)
        {
            _text.text = $"{currentLevel}/{_levelsCount} {mainInfo}";
        }
    }
}