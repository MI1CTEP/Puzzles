using MyGame;
using MyGame.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuScenarioItem : MonoBehaviour
{
    [SerializeField] private Image _imageStage;
    [SerializeField] private Text _nameTypeStage;
    [SerializeField] private Image _imageBlure;
    [SerializeField] private Button _buttonPlay;

    private ScenarioStage _scenarioStage;

    private bool _isAvailable = false;

    private UnityAction <ScenarioStage> _actionClick;

    private int _idStage = 0;

   // public void Init(Sprite Image, string TypeStage, bool isAvailable, UnityAction actionClick)
    public void Init(ScenarioStage scenarioStage, bool isAvailable, UnityAction<ScenarioStage> actionClick, int idStage)
    {
        _scenarioStage = scenarioStage;
        _imageStage.sprite = _scenarioStage.Image;
        _nameTypeStage.text = _scenarioStage.typeStage;
        _isAvailable = isAvailable;
        _actionClick = actionClick;
        _idStage = idStage;

        if (_isAvailable)
        {
            _buttonPlay.onClick.AddListener(OnClickButtonItem);
            _buttonPlay.gameObject.SetActive(true);

            _imageBlure.gameObject.SetActive(false);
        }
        else
        {

            _buttonPlay.gameObject.SetActive(false);

            _imageBlure.gameObject.SetActive(true);
        }

    }


    private void OnClickButtonItem()
    {
        NutakuAPIInitializator.instance.IdStageScenario = _idStage;

        _actionClick?.Invoke(_scenarioStage);
        //SceneLoader.LoadGameplay();
    }


    private void OnDestroy()
    {
        _buttonPlay.onClick.RemoveListener(OnClickButtonItem);
        _actionClick = null;
    }
}
