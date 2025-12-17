using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Text itemNameText;
    public Text itemPriceText;
    public Text itemDescriptionText;
    public RawImage itemImage;
    public Button buyButton;

    private ShopItem _item;
    private System.Action _onBuyClicked;

    public void Initialize(ShopItem item, System.Action onBuyClicked)
    {
        _item = item;
        _onBuyClicked = onBuyClicked;

        if (itemNameText != null)
            itemNameText.text = item.name;

        if (itemPriceText != null)
            itemPriceText.text = $"{item.priceGold} gold";

        if (itemDescriptionText != null)
            itemDescriptionText.text = item.description;

        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyButtonClick);
            buyButton.interactable = item.available;
        }

        // Загрузка изображения (опционально)
        if (itemImage != null && !string.IsNullOrEmpty(item.imageUrl))
        {
            StartCoroutine(LoadImage(item.imageUrl));
        }
    }

    void OnBuyButtonClick()
    {
        _onBuyClicked?.Invoke();
    }

    System.Collections.IEnumerator LoadImage(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success && itemImage != null)
            {
                itemImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            }
        }
    }
}