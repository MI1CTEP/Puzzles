using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIShopController : MonoBehaviour
{
    [SerializeField] private ShopItemUI _prefabShopItemUI;
    [SerializeField] private Transform _parentItemsShop;

    private UnityAction<ShopItem> _onShopItemClicked;

    public void CreateItems(List<ShopItem> shopItems, UnityAction<ShopItem> actionClick)
    {
        foreach (var shopItem in shopItems)
        {
            ShopItemUI item = Instantiate(_prefabShopItemUI, _parentItemsShop);
            item.Initialize(shopItem, actionClick);
        }

    }


}
