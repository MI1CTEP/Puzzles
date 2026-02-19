using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PanelDeactivitedSession : MonoBehaviour
{
    [SerializeField] private Button _buttonOk;
    [SerializeField] private Text _textInfo;

    private UnityAction _actionShow;
    private WaitForSeconds _secondsControlTime = new WaitForSeconds(5);

    public void Initialize(UnityAction actionOnClick)
    {

        _actionShow = actionOnClick;
        _buttonOk.onClick.AddListener(_actionShow);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        StartCoroutine(ShowPanelDeactivitedCoroutine());
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateTextInfo()
    {

    }


    private IEnumerator ShowPanelDeactivitedCoroutine()
    {
        yield return _secondsControlTime;

        _actionShow?.Invoke();
    }
}
