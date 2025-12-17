#if UNITY_ANDROID || UNITY_IOS
using NutakuUnitySdk;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


    public class NutakuService : MonoBehaviour
    {
        [Header("UI Elements")]
        public Text userIdText;
        public Text nicknameText;
        public Text statusText;
        public Text logText;
        public ScrollRect logScrollRect;
        public GameObject loginPanel;
        public Button loginButton;

        [Header("Shop UI")]
        public Transform shopItemsContainer;
        public GameObject shopItemPrefab;

        [Header("Inventory UI")]
        public Transform inventoryContainer;
        public GameObject inventoryItemPrefab;

        [Header("Purchase UI")]
        public GameObject purchaseConfirmPanel;
        public Text purchaseItemNameText;
        public Text purchasePriceText;
        public RawImage purchaseItemImage;
        public Button purchaseConfirmButton;
        public Button purchaseCancelButton;

        [Header("Loading")]
        public GameObject loadingPanel;

        // Данные
        private string _sessionToken = "";
        private List<ShopItem> _shopItems = new List<ShopItem>();
        private List<InventoryItem> _inventoryItems = new List<InventoryItem>();
        private string _apiBaseUrl = "https://api.tetragon-games.org";

        // Поля для обработки покупок
        private string _currentTransactionUrl = "";
        private string _currentPaymentId = "";
        private ShopItem _currentPurchaseItem;



        void Start()
        {
            NutakuSdkConfig.loginResultToGameCallbackDelegate = LoginResultCallback;
            NutakuSdkConfig.paymentBrowserResultToGameCallbackDelegate = Callback_PaymentResultFromBrowser;

            // Инициализируем SDK
            NutakuSdk.Initialize(this);

            // Настраиваем UI
            if (loadingPanel != null)
                loadingPanel.SetActive(false);

            if (purchaseConfirmPanel != null)
                purchaseConfirmPanel.SetActive(false);

            // Настраиваем кнопки
            if (loginButton != null)
                loginButton.onClick.AddListener(OnLoginButtonClick);

            if (purchaseConfirmButton != null)
                purchaseConfirmButton.onClick.AddListener(OnPurchaseConfirm);

            if (purchaseCancelButton != null)
                purchaseCancelButton.onClick.AddListener(OnPurchaseCancel);

            // Проверяем автологин
            CheckAutoLogin();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                // Отправляем heartbeat при возвращении в игру
                NutakuApi.SendHeartbeat(this);
            }
        }

        void CheckAutoLogin()
        {
            UpdateStatus("Checking for saved session...");
        }

        // ========== NUTAKU SDK CALLBACKS ==========
        public static void LoginResultCallback(bool wasSuccess)
        {
            Debug.Log("Успешный логинг LoginResultCallback");
            var controller = FindObjectOfType<NutakuService>();
            if (controller == null) return;

            if (wasSuccess)
            {
                controller.OnLoginSuccess();
            }
            else
            {
                controller.ShowLoginScreen();
            }
        }

        void OnLoginSuccess()
        {
            Debug.Log("Успешный логинг");


            // Получаем данные пользователя из SDK
            string userId = NutakuCurrentUser.GetUserId().ToString();
            string nickname = NutakuCurrentUser.GetUserNickname();

            userIdText.text = "User ID: " + userId;
            nicknameText.text = "Nickname: " + nickname;

            UpdateStatus("Login successful! Performing Game Handshake...");

            // Скрываем панель логина
            if (loginPanel != null)
                loginPanel.SetActive(false);

            // Выполняем Game Handshake
            PerformGameHandshake(userId);
        }

        void ShowLoginScreen()
        {
            UpdateStatus("Please log in with your Nutaku account");
            if (loginPanel != null)
                loginPanel.SetActive(true);
        }

        public void OnLoginButtonClick()
        {
            Debug.Log("Открываем браузер для подтверждения");
            NutakuSdk.OpenLoginPageInBrowser();
        }

        // ========== GAME HANDSHAKE ==========
        void PerformGameHandshake(string userId)
        {
            UpdateStatus("Performing Game Handshake...");

            ShowLoading(true);

            NutakuApi.GameHandshake(this, (rawResult) =>
            {
                ShowLoading(false);

                try
                {
                    if (rawResult.responseCode >= 200 && rawResult.responseCode < 300)
                    {
                        var parsedResult = NutakuApi.Parse_GameHandshake(rawResult);

                        if (parsedResult.game_rc == 0)
                        {
                            UpdateStatus($"Handshake failed: {parsedResult.message}");
                            LogMessage($"Error: Nutaku couldn't reach our server. Details: {parsedResult.message}");
                            ShowLoginScreen();
                        }
                        else
                        {
                            // Парсим JSON ответ от нашего сервера
                            try
                            {
                                var handshakeResponse = JsonUtility.FromJson<GameHandshakeResponse>(parsedResult.message);

                                if (handshakeResponse != null && handshakeResponse.status == "success")
                                {
                                    _sessionToken = handshakeResponse.session_token;
                                    UpdateStatus("Game Handshake successful!");
                                    LogMessage($"Session token received: {_sessionToken.Substring(0, Math.Min(16, _sessionToken.Length))}...");

                                    // Загружаем магазин и инвентарь
                                    LoadShopItems();
                                    LoadInventory();
                                    LoadProfile();
                                }
                                else
                                {
                                    UpdateStatus("Handshake failed: Invalid server response");
                                    LogMessage($"Server response: {parsedResult.message}");
                                    ShowLoginScreen();
                                }
                            }
                            catch (Exception jsonEx)
                            {
                                UpdateStatus("Handshake failed: Invalid JSON");
                                LogMessage($"JSON Parse Error: {jsonEx.Message}");
                                ShowLoginScreen();
                            }
                        }
                    }
                    else
                    {
                        UpdateStatus($"Handshake failed with HTTP {rawResult.responseCode}");
                        LogMessage($"Error: {rawResult.body}");
                        ShowLoginScreen();
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatus("Handshake error: " + ex.Message);
                    LogMessage($"Exception: {ex}");
                    ShowLoginScreen();
                }
            });
        }

        // ========== API REQUESTS ==========
        void LoadShopItems()
        {
            ShowLoading(true);

            StartCoroutine(MakeGetRequest($"{_apiBaseUrl}/api/shop/items", (success, json) =>
            {
                ShowLoading(false);

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

                        UpdateShopUI();
                        LogMessage($"Loaded {shopResponse.items.Count} shop items");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Error parsing shop items: {ex.Message}");
                    }
                }
                else
                {
                    LogMessage("Failed to load shop items");
                }
            }));
        }

        void LoadInventory()
        {
            if (string.IsNullOrEmpty(_sessionToken))
            {
                LogMessage("No session token, skipping inventory load");
                return;
            }

            ShowLoading(true);

            StartCoroutine(MakeAuthenticatedRequest($"{_apiBaseUrl}/api/inventory", "GET", null, (success, json) =>
            {
                ShowLoading(false);

                if (success)
                {
                    try
                    {
                        var inventoryResponse = JsonUtility.FromJson<InventoryResponse>(json);
                        _inventoryItems.Clear();

                        foreach (var itemData in inventoryResponse.inventory)
                        {
                            var inventoryItem = new InventoryItem
                            {
                                sku = itemData.item_sku,
                                name = itemData.name,
                                description = itemData.description,
                                quantity = itemData.quantity,
                                imageUrl = itemData.image_url
                            };
                            _inventoryItems.Add(inventoryItem);
                        }

                        UpdateInventoryUI();
                        LogMessage($"Loaded {inventoryResponse.total_items} inventory items");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Error parsing inventory: {ex.Message}");
                    }
                }
                else
                {
                    LogMessage("Failed to load inventory");
                }
            }));
        }

        void LoadProfile()
        {
            if (string.IsNullOrEmpty(_sessionToken))
                return;

            StartCoroutine(MakeAuthenticatedRequest($"{_apiBaseUrl}/api/profile", "GET", null, (success, json) =>
            {
                if (success)
                {
                    try
                    {
                        var profileResponse = JsonUtility.FromJson<ProfileResponse>(json);
                        LogMessage($"Profile loaded: {profileResponse.nickname}, Spent gold: {profileResponse.total_spent_gold}");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Error parsing profile: {ex.Message}");
                    }
                }
            }));
        }

        // ========== PURCHASE SYSTEM ==========
        public void PurchaseItem(ShopItem item)
        {
            if (item == null)
            {
                LogMessage("Cannot purchase: Item is null");
                return;
            }

            if (!item.available)
            {
                UpdateStatus("Item is not available for purchase");
                return;
            }

            _currentPurchaseItem = item;
            UpdateStatus($"Purchasing: {item.name} ({item.priceGold} gold)");

            ShowLoading(true);

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
                ShowLoading(false);

                try
                {
                    if (rawResult.responseCode >= 200 && rawResult.responseCode < 300)
                    {
                        var parsedResult = NutakuApi.Parse_CreatePayment(rawResult);

                        LogMessage($"Payment created: {parsedResult.paymentId}");
                        LogMessage($"Next step: {parsedResult.next}");

                        // Сохраняем данные для возможного fallback
                        _currentPaymentId = parsedResult.paymentId;
                        _currentTransactionUrl = parsedResult.transactionUrl;

                        if (parsedResult.next == "put")
                        {
                            // Можно завершить через API - показываем подтверждение
                            ShowPurchaseConfirmationDialog(item, parsedResult.paymentId);
                        }
                        else
                        {
                            // Нужно открыть браузер
                            OpenTransactionInBrowser(parsedResult.transactionUrl);
                        }
                    }
                    else
                    {
                        UpdateStatus($"Payment creation failed: {rawResult.responseCode}");
                        LogMessage($"Error: {rawResult.body}");
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatus("Payment error: " + ex.Message);
                    LogMessage($"Exception: {ex}");
                }
            });
        }

        void ShowPurchaseConfirmationDialog(ShopItem item, string paymentId)
        {
            if (purchaseConfirmPanel == null)
            {
                // Если нет UI для подтверждения, сразу подтверждаем
                ConfirmPurchase(paymentId);
                return;
            }

            _currentPaymentId = paymentId;

            // Настраиваем UI диалога
            if (purchaseItemNameText != null)
                purchaseItemNameText.text = item.name;

            if (purchasePriceText != null)
                purchasePriceText.text = $"{item.priceGold} gold";

            // Загружаем изображение
            if (purchaseItemImage != null && !string.IsNullOrEmpty(item.imageUrl))
            {
                StartCoroutine(LoadPurchaseImage(item.imageUrl));
            }

            // Показываем диалог
            purchaseConfirmPanel.SetActive(true);
            UpdateStatus($"Confirm purchase: {item.name} for {item.priceGold} gold?");
        }

        System.Collections.IEnumerator LoadPurchaseImage(string url)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success && purchaseItemImage != null)
                {
                    purchaseItemImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                }
            }
        }

        void OnPurchaseConfirm()
        {
            if (purchaseConfirmPanel != null)
                purchaseConfirmPanel.SetActive(false);

            if (!string.IsNullOrEmpty(_currentPaymentId))
            {
                ConfirmPurchase(_currentPaymentId);
            }
        }

        void OnPurchaseCancel()
        {
            if (purchaseConfirmPanel != null)
                purchaseConfirmPanel.SetActive(false);

            UpdateStatus("Purchase cancelled");
            _currentPaymentId = "";
            _currentTransactionUrl = "";
        }

        void ConfirmPurchase(string paymentId)
        {
            UpdateStatus("Confirming purchase...");
            ShowLoading(true);

            NutakuApi.PutPayment(paymentId, this, (rawResult) =>
            {
                ShowLoading(false);

                if (rawResult.responseCode == 200)
                {
                    UpdateStatus("Purchase completed successfully!");
                    LogMessage($"Payment completed: {rawResult.correlationId}");

                    // Обновляем инвентарь после покупки
                    StartCoroutine(VerifyAndUpdateInventory());
                }
                else if (rawResult.responseCode == 424)
                {
                    UpdateStatus("Payment failed: Server error");
                    LogMessage($"GPHS error for payment: {rawResult.correlationId}");

                    // Пробуем через браузер как fallback
                    if (!string.IsNullOrEmpty(_currentTransactionUrl))
                    {
                        LogMessage("Trying browser fallback...");
                        OpenTransactionInBrowser(_currentTransactionUrl);
                    }
                }
                else
                {
                    UpdateStatus($"Payment failed: {rawResult.responseCode}");
                    LogMessage($"Error: {rawResult.body}");

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
                LogMessage("Cannot open browser: transaction URL is empty");
                return;
            }

            try
            {
                NutakuSdk.OpenTransactionUrlInBrowser(transactionUrl);
                UpdateStatus("Please complete purchase in browser...");
                LogMessage($"Opened browser with URL: {transactionUrl}");
            }
            catch (Exception ex)
            {
                UpdateStatus("Failed to open browser: " + ex.Message);
                LogMessage($"Browser open error: {ex}");
            }
        }

        System.Collections.IEnumerator VerifyAndUpdateInventory()
        {
            // Ждем немного чтобы сервер успел обработать платеж
            yield return new WaitForSeconds(1.5f);

            // Загружаем обновленный инвентарь
            LoadInventory();

            // Показываем сообщение об успехе
            if (_currentPurchaseItem != null)
            {
                UpdateStatus($"Successfully purchased: {_currentPurchaseItem.name}");
            }
        }

        // ========== PAYMENT CALLBACK FROM BROWSER ==========
        public static void Callback_PaymentResultFromBrowser(string paymentId, string statusFromBrowser)
        {
            var controller = FindObjectOfType<NutakuService>();
            if (controller == null) return;

            controller.OnPaymentResultFromBrowser(paymentId, statusFromBrowser);
        }

        void OnPaymentResultFromBrowser(string paymentId, string status)
        {
            LogMessage($"Browser payment result: {status} for {paymentId}");

            if (status == "purchase")
            {
                UpdateStatus("Purchase completed in browser!");
                // Проверяем с сервером что предмет действительно начислен
                StartCoroutine(VerifyAndUpdateInventory());
            }
            else if (status == "cancel")
            {
                UpdateStatus("Purchase cancelled in browser");
            }
            else if (status == "errorFromGPHS")
            {
                UpdateStatus("Payment failed: Server error (GPHS)");
                LogMessage("GPHS error - check server logs");
            }
            else
            {
                UpdateStatus($"Payment error in browser: {status}");
            }
        }

        // ========== UI UPDATES ==========
        void UpdateShopUI()
        {
            if (shopItemsContainer == null || shopItemPrefab == null)
            {
                LogMessage("Shop UI elements not assigned");
                return;
            }

            // Очищаем контейнер
            foreach (Transform child in shopItemsContainer)
            {
                Destroy(child.gameObject);
            }

            // Создаем элементы магазина
            foreach (var item in _shopItems)
            {
                var shopItemObj = Instantiate(shopItemPrefab, shopItemsContainer);
                var shopItemUI = shopItemObj.GetComponent<ShopItemUI>();

                if (shopItemUI != null)
                {
                    shopItemUI.Initialize(item, () =>
                    {
                        // Callback при нажатии на кнопку покупки
                        PurchaseItem(item);
                    });
                }
                else
                {
                    LogMessage("ShopItemUI component not found on prefab");
                }
            }
        }

        void UpdateInventoryUI()
        {
            if (inventoryContainer == null || inventoryItemPrefab == null)
            {
                LogMessage("Inventory UI elements not assigned");
                return;
            }

            foreach (Transform child in inventoryContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var item in _inventoryItems)
            {
                var inventoryItemObj = Instantiate(inventoryItemPrefab, inventoryContainer);
                var inventoryItemUI = inventoryItemObj.GetComponent<InventoryItemUI>();

                if (inventoryItemUI != null)
                {
                    inventoryItemUI.Initialize(item);
                }
                else
                {
                    LogMessage("InventoryItemUI component not found on prefab");
                }
            }
        }

        void UpdateStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;

            LogMessage($"Status: {message}");
        }

        void LogMessage(string message)
        {
            if (logText != null)
            {
                logText.text += "\n" + DateTime.Now.ToString("HH:mm:ss") + " - " + message;

                // Автоскролл
                if (logScrollRect != null)
                {
                    Canvas.ForceUpdateCanvases();
                    logScrollRect.verticalNormalizedPosition = 0f;
                }
            }

            Debug.Log(message);
        }

        // ========== HTTP REQUEST HELPERS ==========
        System.Collections.IEnumerator MakeGetRequest(string url, Action<bool, string> callback)
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
                    LogMessage($"HTTP GET error {request.responseCode}: {request.error}");

                    try
                    {
                        var errorResponse = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);
                        if (!string.IsNullOrEmpty(errorResponse?.detail))
                        {
                            LogMessage($"Server error: {errorResponse.detail}");
                        }
                    }
                    catch { }

                    callback?.Invoke(false, request.error);
                }
            }
        }

        System.Collections.IEnumerator MakeAuthenticatedRequest(string url, string method, string body, Action<bool, string> callback)
        {
            if (string.IsNullOrEmpty(_sessionToken))
            {
                LogMessage("Cannot make authenticated request: no session token");
                callback?.Invoke(false, "No session token");
                yield break;
            }

            using (UnityWebRequest request = new UnityWebRequest(url, method))
            {
                if (!string.IsNullOrEmpty(body))
                {
                    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                }

                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {_sessionToken}");
                request.timeout = 10;

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    callback?.Invoke(true, request.downloadHandler.text);
                }
                else
                {
                    LogMessage($"Auth request error {request.responseCode}: {request.error}");

                    if (request.responseCode == 401)
                    {
                        LogMessage("Session expired or invalid. Please log in again.");
                        ShowLoginScreen();
                    }

                    callback?.Invoke(false, request.error);
                }
            }
        }

        void ShowLoading(bool show)
        {
            if (loadingPanel != null)
                loadingPanel.SetActive(show);
        }

        // ========== UI BUTTONS ==========
        public void OnRefreshShopButtonClick()
        {
            LoadShopItems();
        }

        public void OnRefreshInventoryButtonClick()
        {
            LoadInventory();
        }

        public void OnLogoutButtonClick()
        {
            _sessionToken = "";
            _shopItems.Clear();
            _inventoryItems.Clear();

            NutakuSdk.LogoutAndExit();
            ShowLoginScreen();

            UpdateStatus("Logged out");
        }

        public void OnClearLogButtonClick()
        {
            if (logText != null)
                logText.text = "";
        }



        // Модели данных
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
        public class InventoryItemData
        {
            public string item_sku;
            public int quantity;
            public string name;
            public string description;
            public string image_url;
        }

        [System.Serializable]
        public class InventoryResponse
        {
            public string user_id;
            public List<InventoryItemData> inventory;
            public int total_items;
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
        public class GameHandshakeResponse
        {
            public string session_token;
            public string user_id;
            public int game_user_id;
            public int server_time;
            public string status;
            public string message;
        }

        [System.Serializable]
        public class ErrorResponse
        {
            public string detail;
        }
    }






// Вспомогательные классы для UI
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
public class InventoryItem
{
    public string sku;
    public string name;
    public string description;
    public int quantity;
    public string imageUrl;
}



#endif