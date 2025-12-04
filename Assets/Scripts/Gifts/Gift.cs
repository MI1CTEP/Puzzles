using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace MyGame.Gifts
{
    public sealed class Gift : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _outline;
        [SerializeField] private Button _button;
        [SerializeField] private Image _valueImage;
        [SerializeField] private TextMeshProUGUI _valueText;

        private int _groupId;
        private int _id;
        private int _value;

        public int GroupId => _groupId;
        public int Id => _id;
        public int Value => _value;

        public void Init(GiftsSettings giftsSettings, int groupId, int id)
        {
            _groupId = groupId;
            _id = id;
            _value = GameData.Gifts.LoadValue(_groupId, _id);
            _icon.sprite = giftsSettings.GiftsGroups[groupId].sprites[id];
            _outline.color = giftsSettings.GiftsGroups[groupId].outlineColor;
            _valueImage.color = giftsSettings.GiftsGroups[groupId].outlineColor;
            _button.onClick.RemoveAllListeners();
            ShowValue(int.MaxValue);
        }

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
        }

        public void SetClickLogic(UnityAction<Gift> onClick)
        {
            _button.onClick.AddListener(()=> onClick?.Invoke(this));
        }

        public void ShowValue(int minQuantityForShowing)
        {
            _value = GameData.Gifts.LoadValue(_groupId, _id);
            _valueImage.gameObject.SetActive(_value >= minQuantityForShowing);
            _valueText.gameObject.SetActive(_value >= minQuantityForShowing);
            _valueText.text = _value.ToString();
        }
    }
}