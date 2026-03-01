//#if UNITY_WEBGL
using MyGame;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Threading;

public class NutakuAPIInitializator : MonoBehaviour
{
    public static NutakuAPIInitializator instance;

    [SerializeField] private LoginPanel _loginPanel;
    [SerializeField] private PanelUserInfo _panelUserInfo;
    [SerializeField] private GameObject _PanelQA;
    [SerializeField] private Button _buttonQA;
    [SerializeField] private PanelDeactivitedSession _panelDeactivitedSession;

    public string SessionToken => _sessionToken;
    private string _sessionToken = "";
    private float _secondsControlTime = 45f;

    private PuarchaseService _puarchaseService;
    public PuarchaseService PuarchaseService => _puarchaseService;

    public bool IsOpenAllContent { get; private set; } = false;
    public bool IsOpenGameplayIntoScenarioMenu { get; set; } = false;
    public int IdStageScenario { get; set; } = 0;

    private CancellationTokenSource _sessionCheckCts;

    public bool IsEditorWebGL = false;

    private TypePlatform _typePlatform = TypePlatform.Editor;
    public TypePlatform TypePlatform => _typePlatform;

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
#if UNITY_EDITOR
        Debug.Log("Это сообщение появится только в редакторе Unity!");

        _typePlatform = TypePlatform.Editor;
        OnCompleteHandshakeAsync().Forget();

#elif UNITY_WEBGL
       _typePlatform = TypePlatform.WebGL;
       CheckNutakuReady();

#elif UNITY_ANDROID || UNITY_IOS
       _typePlatform = TypePlatform.Phone;

#else



#endif

    }




    private async UniTask GameStart()
    {
        Debug.Log("START");

        var GO = Instantiate(new GameObject());
        DontDestroyOnLoad(GO);
        GO.name = "PuarchaseService";
        _puarchaseService = GO.AddComponent<PuarchaseService>();
        _puarchaseService.Initialize();
        _panelDeactivitedSession.Initialize(Application.Quit);

        await UniTask.Delay(1000);



        //string keyFirstEntered = "the_first_entered_at_device";
        //if (PlayerPrefs.HasKey(keyFirstEntered))
        //{

        //    _loginPanel.Initialize();
        //}
        //else
        //{
        //    ShowQA();
        //}
    }

    //Он должен быть 1
    //Этот метод выполняет проверку готовности Nutaku API в браузере и уведомляет Unity
    private void CheckNutakuReady()
    {
        Application.ExternalEval(@"
            if (typeof NutakuGI !== 'undefined') {
                SendMessage('NutakuAPIInitializator', 'OnNutakuReady');
            } else {
                var checkInterval = setInterval(function() {
                    if (typeof NutakuGI !== 'undefined') {
                        clearInterval(checkInterval);
                        SendMessage('NutakuAPIInitializator', 'OnNutakuReady');
                    }
                }, 100);
            }
        ");
    }

    //Это ответ после CheckNutakuReady
    public void OnNutakuReady()
    {
        Debug.Log("Nutaku JS API ready at Client");
        Application.ExternalCall("NutakuGI.getQuickUserInfo");
    }


    //Ответ от java, что юзер залогинился в WebGL
    public void OnUserInfoReceived(string userInfoJson)
    {
        try
        {
            var userInfo = JsonUtility.FromJson<NutakuUserInfo>(userInfoJson);

            if (userInfo != null && !string.IsNullOrEmpty(userInfo.id))
            {
                Debug.Log("Успешный логин в WebGL");
                _panelUserInfo.Initialize(userInfo.id, userInfo.nickname);

                if (_loginPanel != null)
                    _loginPanel.gameObject.SetActive(false);

                PerformGameHandshake(userInfo.id);
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Error parsing user info");
        }
    }


    // ========== GAME HANDSHAKE ==========
    //Отправка запроса на Handshake
    void PerformGameHandshake(string userId)
    {
        Application.ExternalCall("window.startGameHandshake");
    }

    //Получение ответа от OnHandshakeComplete
    public void OnHandshakeComplete(string responseJson)
    {
        try
        {
            var rawResponse = JsonUtility.FromJson<RawHandshakeResponse>(responseJson);

            if (rawResponse.game_rc != 0 && !string.IsNullOrEmpty(rawResponse.message))
            {
                var handshakeResponse = JsonUtility.FromJson<GameHandshakeResponse>(rawResponse.message);

                if (handshakeResponse != null && handshakeResponse.status == "success")
                {
                    _sessionToken = handshakeResponse.session_token;
                    Debug.Log("Game Handshake successful!");

                    //LoadGameAsync().Forget();
                    OnCompleteHandshakeAsync().Forget();
                }
                else
                {
                    Debug.Log("Handshake failed");
                    //ShowLoginScreen();
                }
            }
            else
            {
                Debug.Log("Handshake failed with game_rc");
                //ShowLoginScreen();
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Handshake error");
            //ShowLoginScreen();
        }
    }

    //Действия при положительном Handshake
    private async UniTask OnCompleteHandshakeAsync()
    {
        Debug.Log("OnCompleteHandshakeAsync");

        await GameStart();

        //await UniTask.Delay(3000);

        await LoadGameAsync();
    }



    // ========== ЗАГРУЗКА ИГРЫ ==========
    private async UniTask LoadGameAsync()
    {
        Debug.Log("LoadGame");

        // Запускаем загрузки параллельно
        var shopTask = PuarchaseService.LoadShopItemsAsync();


        //если в редакторе
        if(TypePlatform == TypePlatform.Editor)
        {
            await UniTask.WhenAll(shopTask);
        }
        else
        {
            var inventoryTask = PuarchaseService.LoadInventoryAsync();
            await UniTask.WhenAll(shopTask, inventoryTask);
        }

        // var inventoryTask = PuarchaseService.LoadInventoryAsync();
        // Ждем обе
       // await UniTask.WhenAll(shopTask, inventoryTask);


        // Небольшая задержка для надежности
        await UniTask.Delay(TimeSpan.FromSeconds(2));

        StartCheckSessions();
        SceneLoader.LoadInitScene();
    }










    // ========== УПРАВЛЕНИЕ СЕССИЯМИ ==========
    private async UniTask VerifySessionWithServer()
    {
        if (string.IsNullOrEmpty(_sessionToken))
            return;

        using (UnityWebRequest request = UnityWebRequest.Get($"{_puarchaseService.ApiBaseUrl}/api/session/check"))
        {
            request.SetRequestHeader("Authorization", $"Bearer {_sessionToken}");
            request.timeout = 5;

            await request.SendWebRequest();

            if (request.responseCode == 401)
            {
                Debug.Log("Session expired - another device logged in");
                StopCheckSessions();
                _panelDeactivitedSession.Show();
            }
            else if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Сессия активна");
            }
            else
            {
                Debug.Log("Session check failed");
            }
        }
    }

    public void StartCheckSessions()
    {
        _sessionCheckCts = new CancellationTokenSource();
        ControlSessionsLoop(_sessionCheckCts.Token).Forget();
    }

    public void StopCheckSessions()
    {
        _sessionCheckCts?.Cancel();
        _sessionCheckCts?.Dispose();
        _sessionCheckCts = null;
    }

    private async UniTaskVoid ControlSessionsLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_secondsControlTime), cancellationToken: token);
            if (!token.IsCancellationRequested)
            {
                await VerifySessionWithServer();
            }
        }
    }


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
        _loginPanel.Initialize();

        _buttonQA.onClick.RemoveListener(HideQA);
    }

    public void ShowLoginScreen()
    {
        ShowLoginScreenAsync().Forget();
    }

    private async UniTaskVoid ShowLoginScreenAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        if (_loginPanel != null)
            _loginPanel.gameObject.SetActive(true);
    }

    

   


    

    // ========== РАЗМЕР IFRAME ==========
    public void SetIframeSize(int height = 0)
    {
        Application.ExternalCall("NutakuGI.setIframeSize", height);
    }

    private void OnDestroy()
    {
        StopCheckSessions();
    }
}

[System.Serializable]
public class NutakuUserInfo
{
    public string id;
    public string nickname;
    public int grade;
    public int test;
    public string language;
}

[System.Serializable]
public class RawHandshakeResponse
{
    public int game_rc;
    public string message;
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
//#endif


public enum TypePlatform
{
    Editor,
    Phone,
    WebGL,
    Desctop
}