
#if UNITY_ANDROID || UNITY_IOS

using NutakuUnitySdk;

#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] private Button _loginButton;

    public void Initialize()
    {
#if UNITY_ANDROID || UNITY_IOS
        _loginButton.onClick.AddListener(NutakuSdk.OpenLoginPageInBrowser);
#endif
    }
}
