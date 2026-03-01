


#if UNITY_WEBGL
using Cysharp.Threading.Tasks;
using I2.Loc.SimpleJSON;
using MyGame.Bundles;
//using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class PuarchaseService : MonoBehaviour
{
    private string _apiBaseUrl = "https://api.tetragon-games.org";
    public string ApiBaseUrl => _apiBaseUrl;

    // Поля для обработки покупок
    private string _currentTransactionUrl = "";
    private string _currentPaymentId = "";
    private ShopItem _currentPurchaseItem;

    // Для проверок
    private List<ShopItem> _shopItems = new List<ShopItem>();
    private List<InventoryItem> _inventoryItems = new List<InventoryItem>();
    public List<ShopItem> ShopItems => _shopItems;
    public List<InventoryItem> InventoryItems => _inventoryItems;

    private UnityAction succesCallbackPurchase;

    public void Initialize()
    {
        Debug.Log("PuarchaseService initialized for WebGL");
    }





    public void PurchaseItem(ShopItem item, UnityAction succesCallback = null)
    {
        if (!item.available)
        {
            Debug.Log("Item is not available for purchase");
            return;
        }

        _currentPurchaseItem = item;
        Debug.Log("Purchasing");

        Debug.Log(item.priceGold);
        Debug.Log(item.name);
        Debug.Log(item.sku);
        Debug.Log(item.imageUrl);
        Debug.Log(item.description);

        var paymentData = new PaymentData
        {
            price = item.priceGold.ToString(),
            name = item.name,
            skuId = item.sku,
            imgUrl = "https://drive.google.com/uc?export=download&id=1gPxkmuh4sd27pSda1nQiBc-H7Wff17ri",
            description = "description",
            message = ""
        };

        string json = JsonUtility.ToJson(paymentData);
        Debug.Log(json);


        _currentPaymentId = "";
        _currentTransactionUrl = "";
        succesCallbackPurchase = succesCallback;

        //Application.ExternalCall("NutakuGI.createPayment", json);

        Application.ExternalCall("window.createPayment", json);
    }



    public void OnPaymentResultFromBrowser(string jsonResult)
    {
        Debug.Log("Browser payment result");

        Debug.Log(jsonResult);

        var json = JSON.Parse(jsonResult);
        string status = json["status"].Value;
        string paymentId = json["paymentId"].Value;
        // Парсим JSON
        //var result = JsonUtility.FromJson<PaymentResultDataPuzzles>(jsonResult);

     

        //string status = result.status;
        //string paymentId = result.paymentId;

            if (status == "success")
            {
                Debug.Log("Purchase completed in browser!");

                succesCallbackPurchase?.Invoke();
                succesCallbackPurchase = null;

                VerifyAndUpdateInventoryAsync().Forget();
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
                Debug.Log("Payment error in browser");
            }




    }


    //public void OnPaymentResultFromBrowser(string paymentId, string status)
    //{
    //    Debug.Log("Browser payment result");

    //    if (status == "purchase")
    //    {
    //        Debug.Log("Purchase completed in browser!");

    //        succesCallbackPurchase?.Invoke();
    //        succesCallbackPurchase = null;

    //        VerifyAndUpdateInventoryAsync().Forget();
    //    }
    //    else if (status == "cancel")
    //    {
    //        succesCallbackPurchase = null;
    //        Debug.Log("Purchase cancelled in browser");
    //    }
    //    else if (status == "errorFromGPHS")
    //    {
    //        succesCallbackPurchase = null;
    //        Debug.Log("Payment failed: Server error (GPHS)");
    //    }
    //    else
    //    {
    //        succesCallbackPurchase = null;
    //        Debug.Log("Payment error in browser");
    //    }
    //}

    private async UniTaskVoid VerifyAndUpdateInventoryAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        await LoadInventoryAsync();
        Debug.Log("Successfully purchased");
    }


    // ========== ИНВЕНТАРЬ ==========
    public async UniTask LoadInventoryAsync()
    {
        Debug.Log("Start LoadInventoryAsync");

        string tempUrl = "https://api.tetragon-games.org";
        Debug.Log("Inventory URL");

        var (success, json) = await MakeAuthenticatedRequestAsync($"{tempUrl}/api/inventory", "GET", null);

        if (success)
        {
            try
            {
                Debug.Log("Raw inventory JSON length}");

                if (string.IsNullOrEmpty(json))
                {
                    Debug.LogError("Empty inventory response from server");
                    return;
                }

                // Проверяем, что ответ начинается с { (объект)
                if (!json.TrimStart().StartsWith("{"))
                {
                    Debug.LogError("Invalid JSON format - doesn't start with ");
                    return;
                }

                // ИСПОЛЬЗУЕМ NEWTONSOFT ВМЕСТО JsonUtility
                var response = JObject.Parse(json);
                _inventoryItems.Clear();

                var inventoryArray = response["inventory"] as JArray;
                if (inventoryArray == null)
                {
                    Debug.LogError("inventory array is null");
                    return;
                }

                foreach (var itemToken in inventoryArray)
                {
                    if (itemToken == null) continue;

                    var inventoryItem = new InventoryItem
                    {
                        sku = itemToken["item_sku"]?.ToString() ?? "",
                        name = itemToken["name"]?.ToString() ?? "",
                        description = itemToken["description"]?.ToString() ?? "",
                        quantity = itemToken["quantity"]?.Value<int>() ?? 0,
                        imageUrl = itemToken["image_url"]?.ToString() ?? "",
                        category = itemToken["category"]?.ToString() ?? "",
                        source = itemToken["source"]?.ToString() ?? "",
                    };

                    _inventoryItems.Add(inventoryItem);
                }

                int totalItems = response["total_items"]?.Value<int>() ?? _inventoryItems.Count;
                Debug.Log("Loaded inventory items");
            }
            catch (Exception ex)
            {
                Debug.LogError("Error parsing inventory:");
                Debug.LogError("Stack trace: ");
                Debug.LogError("JSON that caused error:");
            }
        }
        else
        {
            Debug.LogError("Failed to load inventory");
        }
    }

    public void LoadInventory()
    {
        LoadInventoryAsync().Forget();
    }

    // ========== ПРОВЕРКИ ИНВЕНТАРЯ ==========


    public List<int> GetAllAvailableShowGirls()
    {
        var list = new List<int>();
        string category = "show_girl";

        foreach (var item in _inventoryItems)
        {
            if (item.category == category && !string.IsNullOrEmpty(item.sku))
            {
                string[] skuParts = item.sku.Split('_');
                if (skuParts.Length > 0 && int.TryParse(skuParts[skuParts.Length - 1], out int curIndex))
                {
                    list.Add(curIndex);
                }
            }
        }
        return list;
    }


    //public List<int> GetAllAvailableShowGirls()
    //{
    //    var list = new List<int>();

    //    string category = "show_girl";
    //    foreach (var item in _inventoryItems)
    //    {

    //        if (item.category == category)
    //        {
    //            int curIndex = int.Parse(item.sku.Split('_')[^1]);

    //            list.Add(curIndex);
    //        }
    //    }

    //    return list;
    //}



    public bool IsAvaliableShowGirl(int idLevel)
    {
        string category = "show_girl";
        string source = "reward";

        foreach (var item in _inventoryItems)
        {
            if (item.category == category && !string.IsNullOrEmpty(item.sku))
            {
                string[] skuParts = item.sku.Split('_');
                if (skuParts.Length > 0 && int.TryParse(skuParts[skuParts.Length - 1], out int curIndex) && curIndex == idLevel)
                    return true;
            }

            if (source == item.source && item.sku == $"keyLevelOnRespect_{idLevel}")
            {
                return true;
            }
        }
        return false;
    }


    //public bool IsAvaliableShowGirl(int idLevel)
    //{
    //    string category = "show_girl";
    //    string source = "reward";

    //    foreach (var item in _inventoryItems)
    //    {
    //        if (item.category == category)
    //        {

    //            int curIndex = int.Parse(item.sku.Split('_')[^1]);
    //            if (curIndex == idLevel)
    //                return true;
    //        }

    //        if (source == item.source)
    //        {
    //            string key = $"keyLevelOnRespect_{idLevel}";
    //            if (item.sku == key)
    //            {
    //                return true;
    //            }
    //        }
    //    }

    //    return false;
    //}




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
        Debug.Log("IsAvaliableBonusStage");
        string category = "bonus_stage";
        foreach (var item in _inventoryItems)
        {
            if (item.category == category && !string.IsNullOrEmpty(item.sku))
            {
                string[] skuParts = item.sku.Split('_');
                if (skuParts.Length > 0 && int.TryParse(skuParts[skuParts.Length - 1], out int curIndex) && curIndex == idLevel)
                    return true;
            }
        }
        return false;
    }


    //public bool IsAvaliableBonusStage(int idLevel)
    //{
    //    string category = "bonus_stage";
    //    foreach (var item in _inventoryItems)
    //    {
    //        if (item.category == category)
    //        {
    //            int curIndex = int.Parse(item.sku.Split('_')[^1]);
    //            if (curIndex == idLevel)
    //                return true;
    //        }
    //    }

    //    return false;
    //}

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

    // ========== МАГАЗИН ==========
    // ========== МАГАЗИН ==========
    public async UniTask LoadShopItemsAsync()
    {
       
        string tempUrl = "https://api.tetragon-games.org";




        var (success, json) = await MakeGetRequestAsync($"{tempUrl}/api/shop/items");

        if (success)
        {
            try
            {
                Debug.Log("Raw shop JSON response length:");

                if (string.IsNullOrEmpty(json))
                {
                    Debug.LogError("Empty shop response from server");
                    return;
                }

                // Проверяем, что ответ начинается с { (объект)
                if (!json.TrimStart().StartsWith("{"))
                {
                    Debug.LogError("Invalid JSON format - doesn't start with ");
                    return;
                }

                // ИСПОЛЬЗУЕМ NEWTONSOFT ВМЕСТО JsonUtility

                Debug.Log("Raw shop JSON response length: 0");
                Debug.Log(_shopItems);
                var response = JObject.Parse(json);
                //Debug.Log(response);


                Debug.Log(gameObject);

                _shopItems.Clear();
                Debug.Log(_shopItems);
                var itemsArray = response["items"] as JArray;
                if (itemsArray == null)
                {
                    Debug.LogError("items array is null");
                    return;
                }


                Debug.Log("Raw shop JSON response length: 1");

                //Дошла вот до сюда _shopItems == null
                //Поэтому не может добавит в _shopItems
                //Возможно вобще весь объект пустой и не создан

                foreach (var itemToken in itemsArray)
                {
                    if (itemToken == null) continue;

                    var shopItem = new ShopItem
                    {
                        id = itemToken["id"]?.ToString() ?? "",
                        sku = itemToken["sku"]?.ToString() ?? "",
                        name = itemToken["name"]?.ToString() ?? "",
                        description = itemToken["description"]?.ToString() ?? "",
                        priceGold = itemToken["price_gold"]?.Value<int>() ?? 0,
                        imageUrl = itemToken["image_url"]?.ToString() ?? "",
                        available = itemToken["available"]?.Value<bool>() ?? false,
                        category = itemToken["category"]?.ToString() ?? ""
                    };
                    _shopItems.Add(shopItem);
                }


                Debug.Log("Raw shop JSON response length: 2");

            }
            catch (Exception ex)
            {
                Debug.LogError("Error parsing shop items");
                Debug.LogError("Stack trace");
                Debug.LogError("JSON that caused error");
            }
        }
        else
        {
            Debug.LogError("Failed to load shop items");
        }
    }

    public void LoadShopItems()
    {
        LoadShopItemsAsync().Forget();
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
        Debug.Log("GetShopItemBonusStage");

        string category = "bonus_stage";
        foreach (var shopItem in _shopItems)
        {
            if (shopItem.category == category && shopItem.available && !string.IsNullOrEmpty(shopItem.id))
            {
                string[] idParts = shopItem.id.Split('_');
                if (idParts.Length > 0 && int.TryParse(idParts[idParts.Length - 1], out int curIndex) && curIndex == index)
                    return shopItem;
            }
        }
        return null;
    }


    //public ShopItem GetShopItemBonusStage(int index)
    //{
    //    string category = "bonus_stage";
    //    foreach (var shopItem in _shopItems)
    //    {
    //        if (shopItem.category == category && shopItem.available)
    //        {
    //            int curIndex = int.Parse(shopItem.id.Split('_')[^1]);
    //            if (curIndex == index)
    //                return shopItem;
    //        }
    //    }

    //    return null;
    //}




    public ShopItem GetShopItemSecretAlbum(int index)
    {
        string category = "secret_album";
        foreach (var shopItem in _shopItems)
        {
            if (shopItem.category == category && shopItem.available && !string.IsNullOrEmpty(shopItem.id))
            {
                string[] idParts = shopItem.id.Split('_');
                if (idParts.Length > 0 && int.TryParse(idParts[idParts.Length - 1], out int curIndex) && curIndex == index)
                    return shopItem;
            }
        }
        return null;
    }


    //public ShopItem GetShopItemSecretAlbum(int index)
    //{
    //    string category = "secret_album";
    //    foreach (var shopItem in _shopItems)
    //    {
    //        if (shopItem.category == category && shopItem.available)
    //        {
    //            int curIndex = int.Parse(shopItem.id.Split('_')[^1]);
    //            if (curIndex == index)
    //                return shopItem;
    //        }
    //    }

    //    return null;
    //}



    public ShopItem GetShopItemShowGirl(int index)
    {
        string category = "show_girl";
        foreach (var shopItem in _shopItems)
        {
            if (shopItem.category == category && shopItem.available && !string.IsNullOrEmpty(shopItem.id))
            {
                string[] idParts = shopItem.id.Split('_');
                if (idParts.Length > 0 && int.TryParse(idParts[idParts.Length - 1], out int curIndex) && curIndex == index)
                    return shopItem;
            }
        }
        return null;
    }




    //public ShopItem GetShopItemShowGirl(int index)
    //{
    //    string category = "show_girl";
    //    foreach (var shopItem in _shopItems)
    //    {
    //        if (shopItem.category == category && shopItem.available)
    //        {
    //            int curIndex = int.Parse(shopItem.id.Split('_')[^1]);
    //            if (curIndex == index)
    //                return shopItem;
    //        }
    //    }

    //    return null;
    //}

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
        Debug.Log("GetShopItemLootbox");

        string category = "lootbox";
        foreach (var shopItem in _shopItems)
        {
            if (shopItem.category == category && shopItem.available && !string.IsNullOrEmpty(shopItem.sku))
            {
                string[] skuParts = shopItem.sku.Split('_');
                if (skuParts.Length > 0 && int.TryParse(skuParts[skuParts.Length - 1], out int curIndex) && curIndex == index)
                    return shopItem;
            }
        }
        return null;
    }



    //public ShopItem GetShopItemLootbox(int index)
    //{
    //    string category = "lootbox";
    //    foreach (var shopItem in _shopItems)
    //    {
    //        if (shopItem.category == category && shopItem.available)
    //        {
    //           // Debug.Log(shopItem.sku);
    //            int curIndex = int.Parse(shopItem.sku.Split('_')[^1]);
    //            if (curIndex == index)
    //                return shopItem;
    //        }
    //    }

    //    return null;
    //}

    // ========== ПРОФИЛЬ ==========
    public async UniTask LoadProfileAsync()
    {
        var (success, json) = await MakeAuthenticatedRequestAsync($"{ApiBaseUrl}/api/profile", "GET", null);

        if (success)
        {
            try
            {
                var profileResponse = JsonUtility.FromJson<ProfileResponse>(json);
                Debug.Log("Profile loaded: ");
            }
            catch (Exception ex)
            {
                Debug.Log("Error parsing profile");
            }
        }
    }

    public void LoadProfile()
    {
        LoadProfileAsync().Forget();
    }

    // ========== HTTP ЗАПРОСЫ (UniTask версии) ==========
    private async UniTask<(bool success, string response)> MakeAuthenticatedRequestAsync(string url, string method, string body)
    {
        Debug.Log("MakeAuthenticatedRequestAsync");
        Debug.Log("SessionToken exists");

        using (UnityWebRequest request = new UnityWebRequest(url, method))
        {
            if (!string.IsNullOrEmpty(body))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                //Debug.Log("Request body");
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(NutakuAPIInitializator.instance?.SessionToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {NutakuAPIInitializator.instance.SessionToken}");
            }
            else
            {
                Debug.LogError("No session token available for authenticated request!");
            }

            request.timeout = 10;

            await request.SendWebRequest();

            Debug.Log("Response code");

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler?.text ?? "";
                Debug.Log("MakeAuthenticatedRequestAsync Success, response length");

                if (responseText.Length > 0)
                {
                    Debug.Log("Response preview");
                }

                return (true, responseText);
            }
            else
            {
                string errorText = request.error ?? "Unknown error";
                Debug.LogError("Auth request error ");

                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    string errorResponse = request.downloadHandler.text;
                    Debug.LogError("Error response body");

                    try
                    {
                        var errorResponseObj = JsonUtility.FromJson<ErrorResponse>(errorResponse);
                        if (!string.IsNullOrEmpty(errorResponseObj?.detail))
                        {
                            Debug.LogError("Server error detail");
                        }
                    }
                    catch { }
                }

                if (request.responseCode == 401)
                {
                    Debug.LogError("Session expired or invalid. Please log in again.");
                    //NutakuAPIInitializator.instance?.ShowLoginScreen();
                }
                else if (request.responseCode == 0)
                {
                    Debug.LogError("Network error - possible CORS or connection issue");
                }

                return (false, errorText);
            }
        }
    }

    private async UniTask<(bool success, string response)> MakeGetRequestAsync(string url)
    {
        Debug.Log("MakeGetRequestAsync");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.timeout = 10;

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler?.text ?? "";
                Debug.Log("MakeGetRequestAsync Success, response length");

                if (responseText.Length > 0)
                {
                    Debug.Log("Response preview");
                }

                return (true, responseText);
            }
            else
            {
                string errorText = request.error ?? "Unknown error";
                Debug.LogError("HTTP GET error ");

                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    string errorResponse = request.downloadHandler.text;
                    Debug.LogError("Error response body");

                    try
                    {
                        var errorResponseObj = JsonUtility.FromJson<ErrorResponse>(errorResponse);
                        if (!string.IsNullOrEmpty(errorResponseObj?.detail))
                        {
                            Debug.LogError("Server error detail");
                        }
                    }
                    catch { }
                }

                if (request.responseCode == 0)
                {
                    Debug.LogError("Network error - possible CORS or connection issue");
                }

                return (false, errorText);
            }
        }
    }

    // ========== УПРАВЛЕНИЕ ИНВЕНТАРЕМ ==========
    public async UniTask MakeInventoryRequestAsync(string sku, int quantityChange, UnityAction succesCallback = null, UnityAction failCallback = null)
    {
        Debug.Log("MakeInventoryRequestAsync");

        var requestData = new UpdateInventoryRequest
        {
            item_sku = sku,
            quantity_change = quantityChange
        };

        string json = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = new UnityWebRequest($"{ApiBaseUrl}/api/inventory/update", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {NutakuAPIInitializator.instance.SessionToken}");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("MakeInventoryRequestAsync Success");
              
                await LoadInventoryAsync();
                succesCallback?.Invoke();
            }
            else
            {
                Debug.LogError("Failed to update inventory");
                failCallback?.Invoke();
            }
        }
    }

    public void StartMakeInventoryRequest(string sku, int quantityChange, UnityAction succesCallback = null, UnityAction failCallback = null)
    {
        if (NutakuAPIInitializator.instance.TypePlatform == TypePlatform.Editor)
            return;

        MakeInventoryRequestAsync(sku, quantityChange, succesCallback, failCallback).Forget();
    }
}
#endif

[System.Serializable]
public class PaymentData
{
    public string price;
    public string name;
    public string skuId;
    public string imgUrl;
    public string description;
    public string message;
}


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


//[System.Serializable]
//public class PaymentResultDataPuzzles
//{
//    public string paymentId;
//    public string status;
//    public string error;
//}