using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace MyGame.Gameplay
{
    public sealed class PaidContentOpenedPanel : MonoBehaviour, IGameStage
    {
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _missButton;
        [SerializeField] private TextMeshProUGUI _priceText;

        public UnityAction OnEnd { get; set; }

        public void Init()
        {
            _buyButton.onClick.AddListener(Buy);
            _missButton.onClick.AddListener(Close);
        }

        public void Play(ScenarioStage scenarioStage)
        {
            Debug.Log("Play");
            bool isAvaiable = NutakuAPIInitializator.instance.PuarchaseService.IsAvaliableBonusStage(GameData.CurrentLevel);
            if (isAvaiable)
            {
                OnEnd?.Invoke();
            }
            else
            {
                string text = NutakuAPIInitializator.instance.PuarchaseService.GetShopItemBonusStage(GameData.CurrentLevel).priceGold.ToString();
                Debug.Log(text);
                _priceText.text = text;
                gameObject.SetActive(true);
            }


            //if (GameData.PaidContent.IsUnlock(GameData.CurrentLevel))
            //    OnEnd?.Invoke();
            //else
            //    gameObject.SetActive(true);
        }



        //Покупка платного уровня. Подруги.
        private void Buy()
        {
            Debug.Log("Buy");
            //GameData.PaidContent.Save(GameData.CurrentLevel);

            var itemShop = NutakuAPIInitializator.instance.PuarchaseService.GetShopItemBonusStage(GameData.CurrentLevel);
            Debug.Log(itemShop);
            int price = itemShop.priceGold;
            Debug.Log(price);
            NutakuAPIInitializator.instance.PuarchaseService.PurchaseItem(itemShop, ActionSuccessPurchase);




           // Close();
        }

        private void Close()
        {
            gameObject.SetActive(false);
            OnEnd?.Invoke();
        }


        public void ActionSuccessPurchase()
        {
            Close();
            GameData.StageGirlLevel.UnlockStage(GameData.CurrentLevel, GameData.CurrentPuzzleStep);
        }
    }
}