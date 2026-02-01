#if UNITY_ANDROID || UNITY_IOS
using MyGame;
using NutakuUnitySdk;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NutakuAPIInitializator : MonoBehaviour
{
    public static NutakuAPIInitializator instance;

    [SerializeField] private LoginPanel _loginPanel;
    [SerializeField] private PanelUserInfo _panelUserInfo;
    [SerializeField] private UIShopController _uIShopController;


    public string SessionToken => _sessionToken;
    private string _sessionToken = "";

    private PuarchaseService _puarchaseService;
    public PuarchaseService PuarchaseService => _puarchaseService;


    //По сути свойства ниже логично убрать в другие скрипты
    public bool IsOpenAllContent {  get; private set; } = true;
    public bool IsOpenGameplayIntoScenarioMenu { get; set; } = false;
    public int IdStageScenario { get; set; } = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        NutakuSdkConfig.loginResultToGameCallbackDelegate = LoginResultCallback;


        var GO = Instantiate(new GameObject(), transform);
        GO.name = "PuarchaseService";
        _puarchaseService = GO.AddComponent<PuarchaseService>();
        _puarchaseService.Initialize();

        NutakuSdk.Initialize(this);

        _loginPanel.Initialize();


        _uIShopController.gameObject.SetActive(false);



       // StartCoroutine(Test());

    }


    private IEnumerator LoadGame()
    {
        PuarchaseService.LoadShopItems();

       // yield return StartCoroutine(PuarchaseService.MakeInventoryRequest("sku_testing", 5));
       // yield return StartCoroutine(PuarchaseService.GetCurrentQuantityAndUpdate("sku_testing", 2));

        PuarchaseService.LoadInventory();

        yield return new WaitForSeconds(1);
        //SceneManager.LoadScene("InitScene");
        SceneLoader.LoadInitScene();

    }

    private IEnumerator Test()
    {

        yield return new WaitForSeconds(4);
        //PuarchaseService.LoadShopItems();
        //yield return new WaitForSeconds(2);
        //_uIShopController.CreateItems(PuarchaseService.GetShopItemsFromCategory("show_girl"), PuarchaseService.PurchaseItem);
        //yield return new WaitForSeconds(2);
        //_uIShopController.gameObject.SetActive(true);

        PuarchaseService.LoadInventory();

        yield return new WaitForSeconds(2);

        PuarchaseService.GetAllAvailableShowGirls();

        var res2 = PuarchaseService.IsAvaliableShowGirl(2);
        var res3 = PuarchaseService.IsAvaliableShowGirl(3);
        Debug.Log($" 2 -  {res2}");
        Debug.Log($" 3 -  {res3}");
    }



    // ========== NUTAKU SDK CALLBACKS ==========
    public static void LoginResultCallback(bool wasSuccess)
    {
        if (wasSuccess)        
            instance.OnLoginSuccess();        
        else        
            instance.ShowLoginScreen();        
    }

    private void OnLoginSuccess()
    {
        Debug.Log("Успешный логинг");

        // Получаем данные пользователя из SDK
        string userId = NutakuCurrentUser.GetUserId().ToString();
        string nickname = NutakuCurrentUser.GetUserNickname();

        _panelUserInfo.Initialize(userId, nickname);


        // Скрываем панель логина
        if (_loginPanel != null)
            _loginPanel.gameObject.SetActive(false);

        // Выполняем Game Handshake
        PerformGameHandshake(userId);
    }

    public void ShowLoginScreen()
    {       
        if (_loginPanel != null)
            _loginPanel.gameObject.SetActive(true);
    }

    // ========== GAME HANDSHAKE ==========
    void PerformGameHandshake(string userId)
    {

        NutakuApi.GameHandshake(this, (rawResult) =>
        {
            try
            {
                if (rawResult.responseCode >= 200 && rawResult.responseCode < 300)
                {
                    var parsedResult = NutakuApi.Parse_GameHandshake(rawResult);

                    //неполадки с сервером
                    if (parsedResult.game_rc == 0)
                    {
                        Debug.Log($"Handshake failed: {parsedResult.message}");
                        ShowLoginScreen();
                    }
                    else
                    {
                        // Парсим JSON ответ от нашего сервера
                        try
                        {
                            var handshakeResponse = JsonUtility.FromJson<GameHandshakeResponse>(parsedResult.message);

                            //сессия создана и сверена можно пускать в игру
                            if (handshakeResponse != null && handshakeResponse.status == "success")
                            {
                                _sessionToken = handshakeResponse.session_token;
                                Debug.Log("Game Handshake successful!");

                                // Загружаем сцену с игрой или даем досмтуп к игре


                                StartCoroutine(LoadGame());
                            }
                            else //Ошибка в сверке сессии
                            {
                                Debug.Log($"Server response: {parsedResult.message}");
                                ShowLoginScreen();
                            }
                        }
                        catch (Exception jsonEx) //Ошибка
                        {
                            Debug.Log($"JSON Parse Error: {jsonEx.Message}");
                            ShowLoginScreen();
                        }
                    }
                }
                else //Ошибка
                {
                    Debug.Log($"Handshake failed with HTTP {rawResult.responseCode}");
                    ShowLoginScreen();
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Handshake error: " + ex.Message);
                ShowLoginScreen();
            }
        });
    }

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

#endif