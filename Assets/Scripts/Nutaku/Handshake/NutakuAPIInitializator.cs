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
    //[SerializeField] private UIShopController _uIShopController;

    [SerializeField] private GameObject _PanelQA;
    [SerializeField] private Button _buttonQA;

    [SerializeField] private PanelDeactivitedSession _panelDeactivitedSession;

    public string SessionToken => _sessionToken;
    private string _sessionToken = "";
    private WaitForSeconds _secondsControlTime = new WaitForSeconds(10);

    private PuarchaseService _puarchaseService;
    public PuarchaseService PuarchaseService => _puarchaseService;


    //По сути свойства ниже логично убрать в другие скрипты
    public bool IsOpenAllContent {  get; private set; } = false;
    public bool IsOpenGameplayIntoScenarioMenu { get; set; } = false;
    public int IdStageScenario { get; set; } = 0;

    private Coroutine _coroutineControlSessions;


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

        // Debug.Log(Application.persistentDataPath);

        

        NutakuSdkConfig.loginResultToGameCallbackDelegate = LoginResultCallback;


        var GO = Instantiate(new GameObject(), transform);
        GO.name = "PuarchaseService";
        _puarchaseService = GO.AddComponent<PuarchaseService>();
        _puarchaseService.Initialize();


        _panelDeactivitedSession.Initialize(Application.Quit);

        string keyFirstEntered = "the_first_entered_at_device";
        if (PlayerPrefs.HasKey(keyFirstEntered))
        {
            NutakuSdk.Initialize(this);
            _loginPanel.Initialize(); //Кнопка авторизации
        }
        else
        {
            ShowQA();
        }


        //NutakuSdk.Initialize(this);
        //_loginPanel.Initialize(); //Кнопка авторизации

    }


    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            NutakuApi.SendHeartbeat(this);
        }
    }



    private IEnumerator VerifySessionWithServer()
    {
        if (string.IsNullOrEmpty(_sessionToken))
            yield break;

        using (UnityWebRequest request = UnityWebRequest.Get($"{_puarchaseService.ApiBaseUrl}/api/session/check"))
        {
            request.SetRequestHeader("Authorization", $"Bearer {_sessionToken}");
            request.timeout = 5;

            yield return request.SendWebRequest();

            if (request.responseCode == 401)
            {
                //Тут логика отключения
                Debug.Log("Session expired - another device logged in");
                StopCheckSessions();
                _panelDeactivitedSession.Show();
            }
            else if(request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Сессия активна");
            }
            else if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"Session check failed: {request.error}");
            }
        }
    }



    public void StartCheckSessions()
    {
        _coroutineControlSessions = StartCoroutine(ControlSessionsCoroutine());
    }
    public void StopCheckSessions()
    {
        if( _coroutineControlSessions != null ) 
            StopCoroutine(_coroutineControlSessions);
    }

    private IEnumerator ControlSessionsCoroutine()
    {
        while (true)
        {
            yield return _secondsControlTime;


            StartCoroutine(VerifySessionWithServer());

        }

        //string serverSessionToken = "dsfsdfs";   

        //if (_sessionToken != serverSessionToken)
        //{
        //    Debug.Log("сессия не правильная");
        //    //SceneLoader.LoadInitializeScene();
        //    _panelDeactivitedSession.Show();

        //    StopCheckSessions();
        //}
    }

    //Предупреждение о раннем доступе
    private void ShowQA()
    {
        _buttonQA.onClick.AddListener(HideQA);
        _PanelQA.SetActive(true);
    }

    private void HideQA()
    {
        string keyFirstEntered = "the_first_entered_at_device";
        PlayerPrefs.SetInt(keyFirstEntered, 1);

        _PanelQA.SetActive(false);

        NutakuSdk.Initialize(this);
        _loginPanel.Initialize();

        _buttonQA.onClick.RemoveListener(HideQA);
    }


    private IEnumerator LoadGame()
    {
       

        PuarchaseService.LoadShopItems();
        PuarchaseService.LoadInventory();

        yield return new WaitForSeconds(1);

        StartCheckSessions();

        SceneLoader.LoadInitScene();

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
        StartCoroutine(ShowLoginScreenCoroutine());
    }


    private IEnumerator ShowLoginScreenCoroutine()
    {
        yield return new WaitForSeconds(2);

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
                    Debug.Log($"Handshake failed with HTTP {rawResult.body}");
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