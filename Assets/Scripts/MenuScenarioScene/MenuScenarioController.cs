using Cysharp.Threading.Tasks;
using MyGame;
using MyGame.Gameplay;
using MyGame.Gameplay.Dialogue;
using MyGame.Gameplay.ExtraLevel;
using MyGame.Gameplay.Puzzle;
using MyGame.Gifts;
using MyGame.Menu;
using MyGame.Shop;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static MyGame.GameData;

public class MenuScenarioController : MonoBehaviour
{
    //[SerializeField] private MenuScenarioItem _menuScenarioItemPrefab;
    //[SerializeField] private Transform _parentForItems;
    //[SerializeField] private PuzzleChoices _puzzleChoices;

    //private ScenarioLoader _scenarioLoader;
    //private Scenario _scenario;
    //private ScenarioStage _currentScenarioStage;

    //private List<string> _availableTypeStage = new() { "Puzzle" }; //, "Dialogue", "Gifts" };

    ////Проверить статус доступности
    ////Передача id
    ////Кликабельность и запуск уровня

    //private async void Start()
    //{
    //    await Init();
    //}

    //private async UniTask Init()
    //{
    //    _scenarioLoader = new();

    //    //Получения сценария девки по id
    //    _scenario = await _scenarioLoader.GetScenario(GameData.CurrentLevel);


    //    CreateItems();


    //    //Получение стадии сценария по id
    //    //_currentScenarioStage = _scenario.TryGetScenarioStage(0);



        

    //}

    //private void CreateItems()
    //{
    //    int countStages = _scenario.scenarioStages.Count;

    //    for (int i = 0; i < countStages; i++)
    //    {
    //        //Получение стадии сценария по id
    //        _currentScenarioStage = _scenario.TryGetScenarioStage(i);

    //        if(_availableTypeStage.Contains(_currentScenarioStage.typeStage))
    //        {
    //            Createitem(i);
    //        }    
    //    }
    //}

    //private void Createitem(int idStage)
    //{
    //    int dialogueId = 0;
    //    int positionId = 0; // это где меняется?

    //    bool available = GameData.Dialogues.IsUnlocked(GameData.CurrentLevel, dialogueId, positionId);


    //    MenuScenarioItem item = Instantiate(_menuScenarioItemPrefab, _parentForItems);
    //    item.Init(_currentScenarioStage, available, OnClickItem, idStage);
    //}

    //private void OnClickItem(ScenarioStage scenarioStage)
    //{
    //    //_puzzleChoices.Open(scenarioStage, true);


    //    NutakuAPIInitializator.instance.IsOpenGameplayIntoScenarioMenu = true;
    //    SceneLoader.LoadGameplay();
    //}

    

}
