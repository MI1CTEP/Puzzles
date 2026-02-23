#if UNITY_WEBGL
using MyGame;
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
    [SerializeField] private GameObject _PanelQA;
    [SerializeField] private Button _buttonQA;
    [SerializeField] private PanelDeactivitedSession _panelDeactivitedSession;

    public string SessionToken => _sessionToken;
    private string _sessionToken = "";
    private WaitForSeconds _secondsControlTime = new WaitForSeconds(10);

    private PuarchaseService _puarchaseService;
    public PuarchaseService PuarchaseService => _puarchaseService;

    public bool IsOpenAllContent { get; private set; } = false;
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
        var GO = Instantiate(new GameObject(), transform);
        GO.name = "PuarchaseService";
        _puarchaseService = GO.AddComponent<PuarchaseService>();
        _puarchaseService.Initialize();

        _panelDeactivitedSession.Initialize(Application.Quit);

        string keyFirstEntered = "the_first_entered_at_device";
        if (PlayerPrefs.HasKey(keyFirstEntered))
        {
            CheckNutakuReady();
            _loginPanel.Initialize();
        }
        else
        {
            ShowQA();
        }
    }

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

    public void OnNutakuReady()
    {
        Debug.Log("Nutaku JS API ready");
        Application.ExternalCall("NutakuGI.getQuickUserInfo");
    }

    // ========== ”œ–¿¬À≈Õ»≈ —≈——»ﬂÃ» ==========
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
                Debug.Log("Session expired - another device logged in");
                StopCheckSessions();
                _panelDeactivitedSession.Show();
            }
            else if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("—ÂÒÒËˇ ‡ÍÚË‚Ì‡");
            }
            else
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
        if (_coroutineControlSessions != null)
            StopCoroutine(_coroutineControlSessions);
    }

    private IEnumerator ControlSessionsCoroutine()
    {
        while (true)
        {
            yield return _secondsControlTime;
            StartCoroutine(VerifySessionWithServer());
        }
    }

    // ========== ÀŒ√»Õ ==========
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
        CheckNutakuReady();
        _loginPanel.Initialize();

        _buttonQA.onClick.RemoveListener(HideQA);
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

    public void OnUserInfoReceived(string userInfoJson)
    {
        try
        {
            var userInfo = JsonUtility.FromJson<NutakuUserInfo>(userInfoJson);

            if (userInfo != null && !string.IsNullOrEmpty(userInfo.id))
            {
                Debug.Log($"”ÒÔÂ¯Ì˚È ÎÓ„ËÌ ‚ WebGL: {userInfo.id}");
                _panelUserInfo.Initialize(userInfo.id, userInfo.nickname);

                if (_loginPanel != null)
                    _loginPanel.gameObject.SetActive(false);

                PerformGameHandshake(userInfo.id);
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error parsing user info: {ex.Message}");
        }
    }

    // ========== GAME HANDSHAKE ==========
    void PerformGameHandshake(string userId)
    {
        Application.ExternalCall("NutakuGI.startHandshake");
    }

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

                    StartCoroutine(LoadGame());
                }
                else
                {
                    Debug.Log($"Handshake failed: {rawResponse.message}");
                    ShowLoginScreen();
                }
            }
            else
            {
                Debug.Log($"Handshake failed with game_rc={rawResponse.game_rc}: {rawResponse.message}");
                ShowLoginScreen();
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Handshake error: {ex.Message}");
            ShowLoginScreen();
        }
    }

    // ========== «¿√–”« ¿ »√–€ ==========
    private IEnumerator LoadGame()
    {
        PuarchaseService.LoadShopItems();
        PuarchaseService.LoadInventory();

        yield return new WaitForSeconds(1);

        StartCheckSessions();
        SceneLoader.LoadInitScene();
    }

    // ========== –¿«Ã≈– IFRAME ==========
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
#endif