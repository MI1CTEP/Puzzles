using MyGame.Bundles;
using NutakuUnitySdk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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


    //Для проверок
    private List<ShopItem> _shopItems = new List<ShopItem>();
    private List<InventoryItem> _inventoryItems = new List<InventoryItem>();
    public List<ShopItem> ShopItems => _shopItems;
    public List<InventoryItem> InventoryItems => _inventoryItems;


    private UnityAction succesCallbackPurchase;

    public void Initialize()
    {
        NutakuSdkConfig.paymentBrowserResultToGameCallbackDelegate = OnPaymentResultFromBrowser;
    }


    //Метод покупки предмета
    public void PurchaseItem(ShopItem item, UnityAction succesCallback = null)
    {

        //Проверка на доступность 
        if (!item.available)
        {
            Debug.Log("Item is not available for purchase");
            return;
        }

        _currentPurchaseItem = item;
        Debug.Log($"Purchasing: {item.name} ({item.priceGold} gold)");


        // Создаем платеж через Nutaku SDK
        var payment = NutakuPayment.PaymentCreationInfo(
            item.sku,
            item.name,
            item.priceGold,
            item.imageUrl,
            item.description
        );

        NutakuApi.CreatePayment(payment, this, (rawResult) =>
        {

            try
            {
                if (rawResult.responseCode >= 200 && rawResult.responseCode < 300)
                {
                    succesCallbackPurchase = succesCallback;

                    var parsedResult = NutakuApi.Parse_CreatePayment(rawResult);

                    Debug.Log($"Payment created: {parsedResult.paymentId}");
                    Debug.Log($"Next step: {parsedResult.next}");

                    // Сохраняем данные для возможного fallback
                    _currentPaymentId = parsedResult.paymentId;
                    _currentTransactionUrl = parsedResult.transactionUrl;

                    if (parsedResult.next == "put")
                    {
                        // Можно завершить через API - показываем подтверждение
                        ConfirmPurchase(parsedResult.paymentId);
                    }
                    else
                    {
                        // Нужно открыть браузер
                        OpenTransactionInBrowser(parsedResult.transactionUrl);
                    }
                }
                else
                {
                    Debug.Log($"Payment creation failed: {rawResult.responseCode}");
                    Debug.Log($"Error: {rawResult.body}");
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Payment error: " + ex.Message);
                Debug.Log($"Exception: {ex}");
            }
        });
    }

    
    void ConfirmPurchase(string paymentId)
    {
        Debug.Log("Confirming purchase...");

        NutakuApi.PutPayment(paymentId, this, (rawResult) =>
        {

            if (rawResult.responseCode == 200)
            {
                Debug.Log("Purchase completed successfully!");
                Debug.Log($"Payment completed: {rawResult.correlationId}");


                succesCallbackPurchase?.Invoke();
                succesCallbackPurchase = null;
                // Обновляем инвентарь после покупки
                StartCoroutine(VerifyAndUpdateInventory());
            }
            else if (rawResult.responseCode == 424)
            {
                Debug.Log("Payment failed: Server error");
                Debug.Log($"GPHS error for payment: {rawResult.correlationId}");

                // Пробуем через браузер как fallback
                if (!string.IsNullOrEmpty(_currentTransactionUrl))
                {
                    Debug.Log("Trying browser fallback...");
                    OpenTransactionInBrowser(_currentTransactionUrl);
                }
            }
            else
            {
                Debug.Log($"Payment failed: {rawResult.responseCode}");
                Debug.Log($"Error: {rawResult.body}");

                // Пробуем через браузер как fallback
                if (!string.IsNullOrEmpty(_currentTransactionUrl))
                {
                    OpenTransactionInBrowser(_currentTransactionUrl);
                }
            }
        });
    }


    void OpenTransactionInBrowser(string transactionUrl)
    {
        if (string.IsNullOrEmpty(transactionUrl))
        {
            Debug.Log("Cannot open browser: transaction URL is empty");
            succesCallbackPurchase = null;
            return;
        }

        try
        {
            NutakuSdk.OpenTransactionUrlInBrowser(transactionUrl);
            Debug.Log("Please complete purchase in browser...");
            Debug.Log($"Opened browser with URL: {transactionUrl}");
        }
        catch (Exception ex)
        {
            succesCallbackPurchase = null;
            Debug.Log("Failed to open browser: " + ex.Message);
            Debug.Log($"Browser open error: {ex}");
        }
    }


    //Калбек покупки
    void OnPaymentResultFromBrowser(string paymentId, string status)
    {
        Debug.Log($"Browser payment result: {status} for {paymentId}");

        if (status == "purchase")
        {
            Debug.Log("Purchase completed in browser!");

            succesCallbackPurchase?.Invoke();
            succesCallbackPurchase = null;

            // Проверяем с сервером что предмет действительно начислен
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
        // Ждем немного чтобы сервер успел обработать платеж
        yield return new WaitForSeconds(1.5f);

        // Загружаем обновленный инвентарь
        LoadInventory();

        //// Показываем сообщение об успехе
        Debug.Log($"Successfully purchased");

    }


    //Делает запрос к БД и скачивает все доступные предметы инвентаря пользователя.
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

                        //Тут добавляем все предметы в список инвентаря вместе с новым купленным
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

    //Получить индексы всех доступных девушек
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
                Debug.Log($"проверка {curIndex}");
                list.Add(curIndex);
            }
        }

        return list;
    }

    //Проверка доступности Девушки, если она куплена или открыта за Respect
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

    //Проверка доступности всех Девушек
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


    //Проверка доступности Дополнительного уровня, idLevel это номер девушки
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

    //Проверка есть ли такая награда
    public bool IsHasAchievements(string keyAchievements)
    {
        string source = "reward";
        foreach (var item in _inventoryItems)
        {
            if (item.source == source)
            {
                if(keyAchievements == item.sku)
                    return true;
            }
        }

        return false;
    }

    //Проверка экстра уровня
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

    //получение количества (quantity) по ключу
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

    //Проверка есть ли такой диалог
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


    //Загрузка доступных предметов из магазина.
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

    //Получить все предметы указанной категории
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

    //Получить предмет категории bonus_stage по индексу
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
   
    //public int GetShopItemBonusStagePrice(int index)
    //{
    //    string category = "bonus_stage";
    //    foreach (var shopItem in _shopItems)
    //    {
    //        if (shopItem.category == category && shopItem.available)
    //        {
    //            int curIndex = int.Parse(shopItem.id.Split('_')[^1]);
    //            if (curIndex == index)
    //                return shopItem.priceGold;
    //        }
    //    }

    //    return 49;
    //}



    //Получить предмет категории secret_album по индексу
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

    //Получить предмет категории show_girl по индексу
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

    //Получить предмет категории show_all_girls все уровни
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



    //Получить предмет категории girl по индексу
    //public ShopItem GetShopItemGirl(int index)
    //{
    //    string category = "girl";
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



    //Загрузка данных профиля. Вдруг понадобится.
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


    //Аутентификация. Проверка сессии пользователя.
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



    // Управление достижениями

    // Добавить предмет (если его нет) или увеличить количество
    //public void AddItemToInventory(string sku, int quantity = 1)
    //{
    //    StartCoroutine(MakeInventoryRequest(sku, quantity));
    //}

    //// Удалить предмет из инвентаря
    //public void RemoveItemFromInventory(string sku)
    //{
    //    // quantity_change = -9999 удалит предмет полностью
    //    StartCoroutine(MakeInventoryRequest(sku, -9999));
    //}

    //// Уменьшить количество предмета (например, использовать предмет)
    //public void UseItem(string sku, int amount = 1)
    //{
    //    StartCoroutine(MakeInventoryRequest(sku, -amount));
    //}

    // Установить точное количество
    //public void SetItemQuantity(string sku, int exactQuantity)
    //{
    //    // Сначала получим текущее количество
    //    StartCoroutine(GetCurrentQuantityAndUpdate(sku, exactQuantity));
    //}

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


    //добавить предмет в инвентарь, изменить его количество
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

    //Обновление количества
    //public IEnumerator GetCurrentQuantityAndUpdate(string sku, int exactQuantity)
    //{
    //    // Сначала получаем инвентарь
    //   // yield return StartCoroutine(LoadInventory());

    //    // Находим текущее количество
    //    int currentQuantity = 0;
    //    foreach (var item in InventoryItems)
    //    {
    //        if (item.sku == sku)
    //        {
    //            currentQuantity = item.quantity;
    //            break;
    //        }
    //    }

    //    // Вычисляем разницу
    //    int quantityChange = exactQuantity - currentQuantity;

    //    if (quantityChange != 0)
    //    {
    //        yield return StartCoroutine(MakeInventoryRequest(sku, quantityChange));
    //    }
    //}

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
