using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _priceText;
    public Image itemImage;
    public Button buyButton;

    private ShopItem _item;
    private UnityAction<ShopItem> _onBuyClicked;

    public void Initialize(ShopItem item, UnityAction<ShopItem> onBuyClicked)
    {
        _item = item;
        _onBuyClicked = onBuyClicked;

        _nameText.text = item.name;
        _priceText.text = item.priceGold.ToString();

      

        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyButtonClick);
            buyButton.interactable = item.available;
        }

        //// Загрузка изображения (опционально)
        //if (itemImage != null && !string.IsNullOrEmpty(item.imageUrl))
        //{
        //    StartCoroutine(LoadImage(item.imageUrl));
        //}
    }

    void OnBuyButtonClick()
    {
        _onBuyClicked?.Invoke(_item);
    }

    //System.Collections.IEnumerator LoadImage(string url)
    //{
    //    using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
    //    {
    //        yield return request.SendWebRequest();

    //        if (request.result == UnityWebRequest.Result.Success && itemImage != null)
    //        {
    //            itemImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    //        }
    //    }
    //}
}