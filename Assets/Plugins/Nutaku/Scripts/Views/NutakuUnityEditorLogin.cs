#if (UNITY_ANDROID || UNITY_IOS) && UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UI;

namespace NutakuUnitySdk
{
    /// <summary>
    /// A login screen used by the SDK at runtime in Unity Editor.
    /// Outside Unity Editor, the device browser is used instead.
    /// </summary>
    public class NutakuUnityEditorLogin : MonoBehaviour
    {
        static NutakuUnityEditorLogin instance;
        
        public GameObject content;
        public GameObject touchBlocker;

        public Button loginButton;
        public Text messageText;
        public InputField emailInputField;
        public InputField passwordInputField;


        void Awake()
        {
            loginButton.onClick.AddListener(LoginButtonClick);
        }


        private static void InitInstance()
        {
            instance = Instantiate(Resources.Load<NutakuUnityEditorLogin>("Nutaku/Prefabs/Editor/NutakuUnityEditorLogin")) as NutakuUnityEditorLogin;
            instance.transform.SetAsLastSibling();
        }


        internal static void Show()
        {
            if (instance == null)
                InitInstance();

            instance.content.SetActive(true);
            instance.touchBlocker.SetActive(false);
        }


        private void LoginButtonClick()
        {
            try
            {
                if (string.IsNullOrEmpty(emailInputField.text) || string.IsNullOrEmpty(passwordInputField.text))
                    messageText.text = "Please enter a valid sandbox Username and a Password.";
                else
                {
                    touchBlocker.SetActive(true);
                    NutakuApi.ManualLoginForUnityEditor(emailInputField.text, passwordInputField.text, this, EditorManualLoginCallback);
                }
            }
            catch (Exception ex)
            {
                messageText.text = ex.Message;
                touchBlocker.SetActive(false);
            }
        }


        void EditorManualLoginCallback(NutakuApiRawResult rawResult)
        {
            try
            {
                if ((rawResult.responseCode > 199) && (rawResult.responseCode < 300))
                {
                    var result = NutakuApi.ParseApiResponse<NutakuUserProfile>(rawResult);
                    NutakuCurrentUser.SaveUserInfo(result.nugt, result.id, result.nickname, result.test, result.grade, result.language);
                    NutakuSdk.Log("EditorManualLoginCallback() Manual Login successful");
                    NutakuSdkConfig.loginResultToGameCallbackDelegate.Invoke(true);

                    if (instance != null)
                        Destroy(instance.gameObject);
                }
                else
                {
                    NutakuSdk.Log("EditorManualLoginCallback() Manual Login failed");
                    messageText.text = rawResult.body;
                    touchBlocker.SetActive(false);
                }
            }
            catch (Exception ex)
            {
                NutakuSdk.Log("EditorManualLoginCallback() Manual Login exception: " + ex.Message);
                messageText.text = ex.Message;
                touchBlocker.SetActive(false);
            }
        }
    }
}
#endif
