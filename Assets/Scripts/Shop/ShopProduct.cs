using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MyGame.Gifts;

namespace MyGame.Shop
{
    public sealed class ShopProduct : MonoBehaviour
    {
        [SerializeField] private Button _buyButton;
        [SerializeField] private int _idGroup;

        private GiftsSettings _giftsSettings;

        public UnityAction OnBuy { get; set; }

        public void Init(GiftsSettings giftsSettings)
        {
            _giftsSettings = giftsSettings;
            _buyButton.onClick.AddListener(Buy);
        }

        public void Buy()
        {
            //Вызвать метод покупки
            //Продать 10 подарков

            var itemShop = NutakuAPIInitializator.instance.PuarchaseService.GetShopItemLootbox(_idGroup);
            NutakuAPIInitializator.instance.PuarchaseService.PurchaseItem(itemShop, ActionSuccessPurchase);

            //Вот это добавить callback после покупки
            //for (int i = 0; i < _giftsSettings.GiftsGroups[_idGroup].sprites.Length; i++)
            //{
            //    GameData.Gifts.AddValue(_idGroup, i, 1);
            //}

            //Возможно лучше перенести в ActionSuccessPurchase
            //OnBuy?.Invoke();
        }


        public void ActionSuccessPurchase()
        {
            for (int i = 0; i < _giftsSettings.GiftsGroups[_idGroup].sprites.Length; i++)
            {
                GameData.Gifts.AddValue(_idGroup, i, 1);
            }
        }
    }
}