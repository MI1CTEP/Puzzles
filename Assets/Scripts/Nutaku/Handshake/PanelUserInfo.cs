using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelUserInfo : MonoBehaviour
{
    [SerializeField] private Text _userNameText;
    [SerializeField] private Text _userIdText;
    [SerializeField] private Text _Info;

    public void Initialize(string userName, string userId)
    {
        _userNameText.text = userName;
        _userIdText.text = userId;
    }


    public void UpdateInfo(string info)
    {
        _Info.text = info;
    }

}
