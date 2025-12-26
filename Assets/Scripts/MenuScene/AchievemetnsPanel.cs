using UnityEngine;
using TMPro;

namespace MyGame.Menu
{
    public sealed class AchievemetnsPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void Init()
        {
            Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            int value = 0;
            for (int i = 0; i < 10; i++)
            {
                if (GameData.Achievements.IsUnlock(GameData.CurrentLevel, i))
                    value++;
            }
            _text.text = $"{value}/10";
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}