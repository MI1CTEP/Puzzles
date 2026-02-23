#if UNITY_WEBGL
using MyGame.Bundles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class PuarchaseService : MonoBehaviour
{
    private string _apiBaseUrl = "https://api.tetragon-games.org";
    public string ApiBaseUrl => _apiBaseUrl;

    // œÓÎˇ ‰Îˇ Ó·‡·ÓÚÍË ÔÓÍÛÔÓÍ
    private string _currentTransactionUrl = "";
    private string _currentPaymentId = "";
    private ShopItem _currentPurchaseItem;

    // ƒÎˇ ÔÓ‚ÂÓÍ
    private List<ShopItem> _shopItems = new List<ShopItem>();
    private List<InventoryItem> _inventoryItems = new List<InventoryItem>();
    public List<ShopItem> ShopItems => _shopItems;
    public List<InventoryItem> InventoryItems => _inventoryItems;

    private UnityAction succesCallbackPurchase;

    public void Initialize()
    {
        // ¬ WebGL ÌÂÚ ‰ÂÎÂ„‡Ú‡, ÔÓÒÚÓ ÎÓ„ËÛÂÏ
        Debug.Log("PuarchaseService initialized for WebGL");
    }

    // ========== œŒ ”œ » ==========
    public void PurchaseItem(ShopItem item, UnityAction succesCallback = null)
    {
        if (!item.available)
        {
            Debug.Log("Item is not available for purchase");
            return;
        }

        _currentPurchaseItem = item;
        Debug.Log($"Purchasing: {item.name} ({item.priceGold} gold)");

        var paymentData = new
        {
            price = item.priceGold,
            name = item.name,
            skuId = item.sku,
            imgUrl = item.imageUrl,
            description = item.description,
            message = ""
        };

        string json = JsonUtility.ToJson(paymentData);
        _currentPaymentId = ""; // Œ˜Ë˘‡ÂÏ ÔÂÂ‰ ÌÓ‚ÓÈ ÔÓÍÛÔÍÓÈ
        _currentTransactionUrl = "";
        succesCallbackPurchase = succesCallback;

        // ¬˚Á˚‚‡ÂÏ JS ÙÛÌÍˆË˛ NutakuGI.createPayment
        Application.ExternalCall("NutakuGI.createPayment", json);
    }

    // ¬˚Á˚‚‡ÂÚÒˇ ËÁ NutakuWebGLInitializator.OnPaymentResult
    public void OnPaymentResultFromBrowser(string paymentId, string status)
    {
        Debug.Log($"Browser payment result: {status} for {paymentId}");

        if (status == "purchase")
        {
            Debug.Log("Purchase completed in browser!");

            succesCallbackPurchase?.Invoke();
            succesCallbackPurchase = null;

            StartCoroutine(VerifyAndUpdateInventory());
        }
        else if (status == "cancel")
        {
            succesCallbackPurchase = null;
            Debug.Log("Purchase cancelled in browser");
        }
        else if (status == "errorFromGPHS")
        {
            succesCallbackPurchase = null;
            Debug.Log("Payment failed: Server error (GPHS)");
        }
        else
        {
            succesCallbackPurchase = null;
            Debug.Log($"Payment error in browser: {status}");
        }
    }

    private IEnumerator VerifyAndUpdateInventory()
    {
        yield return new WaitForSeconds(1.5f);
        LoadInventory();
        Debug.Log("Successfully purchased");
    }

    // ========== »Õ¬≈Õ“¿–‹ ==========
    public void LoadInventory()
    {
        StartCoroutine(MakeAuthenticatedRequest($"{_apiBaseUrl}/api/inventory", "GET", null, (success, json) =>
        {
            if (success)
            {
                try
                {
                    var inventoryResponse = JsonUtility.FromJson<InventoryResponse>(json);
                    _inventoryItems.Clear();

                    Debug.Log(json);
                    foreach (var itemData in inventoryResponse.inventory)
                    {
                        var inventoryItem = new InventoryItem
                        {
                            sku = itemData.item_sku,
                            name = itemData.name,
                            description = itemData.description,
                            quantity = itemData.quantity,
                            imageUrl = itemData.image_url,
                            category = itemData.category,
                            source = itemData.source,
                        };

                        _inventoryItems.Add(inventoryItem);
                    }

                    Debug.Log($"Loaded {inventoryResponse.total_items} inventory items");
                }
                catch (Exception ex)
                {
                    Debug.Log($"Error parsing inventory: {ex.Message}");
                }
            }
            else
            {
                Debug.Log("Failed to load inventory");
            }
        }));
    }

    // ========== œ–Œ¬≈– » »Õ¬≈Õ“¿–ﬂ ==========
    public List<int> GetAllAvailableShowGirls()
    {
        Debug.Log($"GetAllAvailableShowGirls {_inventoryItems.Count}");
        var list = new List<int>();

        string category = "show_girl";
        foreach (var item in _inventoryItems)
        {
            Debug.Log(item.category);
            if (item.category == category)
            {
                int curIndex = int.Parse(item.sku.Split('_')[^1]);
                Debug.Log($"ÔÓ‚ÂÍ‡ {curIndex}");
                list.Add(curIndex);
            }
        }

        return list;
    }

    public bool IsAvaliableShowGirl(int idLevel)
    {
        string category = "show_girl";
        string source = "reward";

        foreach (var item in _inventoryItems)
        {
            if (item.category == category)
            {
                int curIndex = int.Parse(item.sku.Split('_')[^1]);
                if (curIndex == idLevel)
                    return true;
            }

            if (source == item.source)
            {
                string key = $"keyLevelOnRespect_{idLevel}";
                if (item.sku == key)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsAvaliableAllShowGirls()
    {
        string category = "show_all_girls";
        foreach (var item in _inventoryItems)
        {
            if (item.category == category)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsAvaliableBonusStage(int idLevel)
    {
        string category = "bonus_stage";
        foreach (var item in _inventoryItems)
        {
            if (item.category == category)
            {
                int curIndex = int.Parse(item.sku.Split('_')[^1]);
                if (curIndex == idLevel)
                    return true;
            }
        }

        return false;
    }

    public bool IsHasAchievements(string keyAchievements)
    {
        string source = "reward";
        foreach (var item in _inventoryItems)
        {
            if (item.source == source)
            {
                if (keyAchievements == item.sku)
                    return true;
            }
        }

        return false;
    }

    public bool IsUnlockPartExtraLevel(string keyPart)
    {
        string source = "reward";
        foreach (var item in _inventoryItems)
        {
            if (item.source == source)
            {
                if (keyPart == item.sku)
                    return true;
            }
        }

        return false;
    }

    public int GetOpenedPartsValue(string key)
    {
        string source = "reward";
        foreach (var item in _inventoryItems)
        {
            if (item.source == source)
            {
                if (key == item.sku)
                    return item.quantity;
            }
        }

        return 0;
    }

    public bool IsHasDialogues(string keyDialogues)
    {
        string source = "reward";
        foreach (var item in _inventoryItems)
        {
            if (item.source == source)
            {
                if (keyDialogues == item.sku)
                    return true;
            }
        }

        return false;
    }

    // ========== Ã¿√¿«»Õ ==========
    public void LoadShopItems()
    {
        StartCoroutine(MakeGetRequest($"{_apiBaseUrl}/api/shop/items", (success, json) =>
        {
            if (success)
            {
                try
                {
                    var shopResponse = JsonUtility.FromJson<ShopResponse>(json);
                    _shopItems.Clear();
                    foreach (var itemData in shopResponse.items)
                    {
                        var shopItem = new ShopItem
                        {
                            id = itemData.id,
                            sku = itemData.sku,
                            name = itemData.name,
                            description = itemData.description,
                            priceGold = itemData.price_gold,
                            imageUrl = itemData.image_url,
                            available = itemData.available,
                            category = itemData.category
                        };
                        _shopItems.Add(shopItem);
                    }

                    Debug.Log($"Loaded {shopResponse.items.Count} shop items");
                }
                catch (Exception ex)
                {
                    Debug.Log($"Error parsing shop items: {ex.Message}");
                }
            }
            else
            {
                Debug.Log("Failed to load shop items");
            }
        }));
    }

    public List<ShopItem> GetShopItemsFromCategory(string category)
    {
        List<ShopItem> shopItems = new List<ShopItem>();
        foreach (var shopItem in _shopItems)
        {
            if (shopItem.category == category && shopItem.available)
                shopItems.Add(shopItem);
        }
        return shopItems;
    }

    public ShopItem GetShopItemBonusStage(int index)
    {
        string category = "bonus_stage";
        foreach (var shopItem in _shopItems)
        {
            if (shopItem.category == category && shopItem.available)
            {
                int curIndex = int.Parse(shopItem.id.Split('_')[^1]);
                if (curIndex == index)
                    return shopItem;
            }
        }

        return null;
    }

    public ShopItem GetShopItemSecretAlbum(int index)
    {
        string category = "secret_album";
        foreach (var shopItem in _shopItems)
        {
            if (shopItem.category == category && shopItem.available)
            {
                int curIndex = int.Parse(shopItem.id.Split('_')[^1]);
                if (curIndex == index)
                    return shopItem;
            }
        }

        return null;
    }

    public ShopItem GetShopItemShowGirl(int index)
    {
        string category = "show_girl";
        foreach (var shopItem in _shopItems)
        {
            if (shopItem.category == category && shopItem.available)
            {
                int curIndex = int.Parse(shopItem.id.Split('_')[^1]);
                if (curIndex == index)
                    return shopItem;
            }
        }

        return null;
    }

    public ShopItem GetShopItemShowGirls()
    {
        string category = "show_all_girls";
        foreach (var shopItem in _shopItems)
        {
            if (shopItem.category == category && shopItem.available)
            {
                return shopItem;
            }
        }

        return null;
    }

    public ShopItem GetShopItemLootbox(int index)
    {
        string category = "lootbox";
        foreach (var shopItem in _shopItems)
        {
            if (shopItem.category == category && shopItem.available)
            {
                Debug.Log(shopItem.sku);
                int curIndex = int.Parse(shopItem.sku.Split('_')[^1]);
                if (curIndex == index)
                    return shopItem;
            }
        }

        return null;
    }

    // ========== œ–Œ‘»À‹ ==========
    public void LoadProfile()
    {
        StartCoroutine(MakeAuthenticatedRequest($"{_apiBaseUrl}/api/profile", "GET", null, (success, json) =>
        {
            if (success)
            {
                try
                {
                    var profileResponse = JsonUtility.FromJson<ProfileResponse>(json);
                    Debug.Log($"Profile loaded: {profileResponse.nickname}, Spent gold: {profileResponse.total_spent_gold}");
                }
                catch (Exception ex)
                {
                    Debug.Log($"Error parsing profile: {ex.Message}");
                }
            }
        }));
    }

    // ========== HTTP «¿œ–Œ—€ ==========
    private IEnumerator MakeAuthenticatedRequest(string url, string method, string body, Action<bool, string> callback)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, method))
        {
            if (!string.IsNullOrEmpty(body))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {NutakuAPIInitializator.instance.SessionToken}");
            request.timeout = 10;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(true, request.downloadHandler.text);
            }
            else
            {
                Debug.Log($"Auth request error {request.responseCode}: {request.error}");

                if (request.responseCode == 401)
                {
                    Debug.Log("Session expired or invalid. Please log in again.");
                    NutakuAPIInitializator.instance.ShowLoginScreen();
                }

                callback?.Invoke(false, request.error);
            }
        }
    }

    private IEnumerator MakeGetRequest(string url, Action<bool, string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.timeout = 10;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(true, request.downloadHandler.text);
            }
            else
            {
                Debug.Log($"HTTP GET error {request.responseCode}: {request.error}");

                try
                {
                    var errorResponse = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);
                    if (!string.IsNullOrEmpty(errorResponse?.detail))
                    {
                        Debug.Log($"Server error: {errorResponse.detail}");
                    }
                }
                catch { }

                callback?.Invoke(false, request.error);
            }
        }
    }

    // ========== ”œ–¿¬À≈Õ»≈ »Õ¬≈Õ“¿–≈Ã ==========
    public IEnumerator MakeInventoryRequest(string sku, int quantityChange, UnityAction succesCallback = null, UnityAction failCallback = null)
    {
        var requestData = new UpdateInventoryRequest
        {
            item_sku = sku,
            quantity_change = quantityChange
        };

        string json = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = new UnityWebRequest($"{_apiBaseUrl}/api/inventory/update", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {NutakuAPIInitializator.instance.SessionToken}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Inventory updated: {sku} changed by {quantityChange}");
                LoadInventory();
                succesCallback?.Invoke();
            }
            else
            {
                Debug.LogError($"Failed to update inventory: {request.error}");
                failCallback?.Invoke();
            }
        }
    }

    public void StartMakeInventoryRequest(string sku, int quantityChange, UnityAction succesCallback = null, UnityAction failCallback = null)
    {
        StartCoroutine(MakeInventoryRequest(sku, quantityChange, succesCallback, failCallback));
    }
}
#endif



[System.Serializable]
public class InventoryItem
{
    public string sku;
    public string name;
    public string description;
    public int quantity;
    public string imageUrl;
    public string category;
    public string source;
}


[System.Serializable]
public class InventoryItemData
{
    public string item_sku;
    public int quantity;
    public string name;
    public string description;
    public string image_url;
    public string category;
    public string source;
}


[System.Serializable]
public class InventoryResponse
{
    public string user_id;
    public List<InventoryItemData> inventory;
    public int total_items;
}


[System.Serializable]
public class ShopItem
{
    public string id;
    public string sku;
    public string name;
    public string description;
    public int priceGold;
    public string imageUrl;
    public bool available;
    public string category;
}

[System.Serializable]
public class ShopItemData
{
    public string id;
    public string sku;
    public string name;
    public string description;
    public int price_gold;
    public string image_url;
    public bool available;
    public string category;
}

[System.Serializable]
public class ShopResponse
{
    public int count;
    public List<ShopItemData> items;
    public string version;
}


[System.Serializable]
public class ProfileResponse
{
    public string nutaku_user_id;
    public int game_user_id;
    public string nickname;
    public int grade;
    public string language;
    public int total_spent_gold;
    public string created_at;
    public string last_login;
}

[System.Serializable]
public class ErrorResponse
{
    public string detail;
}


[System.Serializable]
public class UpdateInventoryRequest
{
    public string item_sku;
    public int quantity_change;
    public string source = "reward";
}
