using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    public Text itemNameText;
    public Text itemQuantityText;
    public Text itemDescriptionText;
    public RawImage itemImage;

    public void Initialize(InventoryItem item)
    {
        if (itemNameText != null)
            itemNameText.text = item.name;

        if (itemQuantityText != null)
            itemQuantityText.text = $"x{item.quantity}";

        if (itemDescriptionText != null)
            itemDescriptionText.text = item.description;

        if (itemImage != null && !string.IsNullOrEmpty(item.imageUrl))
        {
            StartCoroutine(LoadImage(item.imageUrl));
        }
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