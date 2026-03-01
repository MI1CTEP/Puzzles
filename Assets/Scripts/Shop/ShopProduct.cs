using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MyGame.Gifts;
using TMPro;

namespace MyGame.Shop
{
    public sealed class ShopProduct : MonoBehaviour
    {
        [SerializeField] private Button _buyButton;
        [SerializeField] private int _idGroup;
        [SerializeField] private TextMeshProUGUI _priceText;

        private GiftsSettings _giftsSettings;

        public UnityAction OnBuy { get; set; }

        private UnityAction _onClosePanelShopGifts { get; set; }

        public void Init(GiftsSettings giftsSettings, UnityAction onClosePanelShopGifts)
        {
            _giftsSettings = giftsSettings;
            _buyButton.onClick.AddListener(Buy);
            _onClosePanelShopGifts = onClosePanelShopGifts;
        }

        public void ShowPrice()
        {
            Debug.Log("ShowPrice");
            string priceText = NutakuAPIInitializator.instance.PuarchaseService.GetShopItemLootbox(_idGroup).priceGold.ToString();
            Debug.Log(priceText);
            _priceText.text = priceText;
        }

        public void Buy()
        {
            //Вызвать метод покупки
            //Продать 10 подарков
            Debug.Log("Buy");
            var itemShop = NutakuAPIInitializator.instance.PuarchaseService.GetShopItemLootbox(_idGroup);
            Debug.Log(itemShop);
            int price = itemShop.priceGold;
            Debug.Log(price);
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


            OnBuy?.Invoke();

            _onClosePanelShopGifts?.Invoke();
        }
    }
}