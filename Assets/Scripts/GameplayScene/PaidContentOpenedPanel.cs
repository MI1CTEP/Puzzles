using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MyGame.Gameplay
{
    public sealed class PaidContentOpenedPanel : MonoBehaviour, IGameStage
    {
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _missButton;

        public UnityAction OnEnd { get; set; }

        public void Init()
        {
            _buyButton.onClick.AddListener(Buy);
            _missButton.onClick.AddListener(Close);
        }

        public void Play(ScenarioStage scenarioStage)
        {
            if(NutakuAPIInitializator.instance.PuarchaseService.IsAvaliableBonusStage(GameData.CurrentLevel))
                OnEnd?.Invoke();
            else
                gameObject.SetActive(true);


            //if (GameData.PaidContent.IsUnlock(GameData.CurrentLevel))
            //    OnEnd?.Invoke();
            //else
            //    gameObject.SetActive(true);
        }

        private void Buy()
        {
            
            GameData.PaidContent.Save(GameData.CurrentLevel);
            Close();
        }

        private void Close()
        {
            gameObject.SetActive(false);
            OnEnd?.Invoke();
        }
    }
}