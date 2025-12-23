using NutakuUnitySdk;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] private Button _loginButton;

    public void Initialize()
    {
        _loginButton.onClick.AddListener(NutakuSdk.OpenLoginPageInBrowser);
    }
}
