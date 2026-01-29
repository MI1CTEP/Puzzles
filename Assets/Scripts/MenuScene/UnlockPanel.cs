using UnityEngine;
using UnityEngine.Events;

namespace MyGame.Menu
{
    public class UnlockPanel : MenuPanel
    {
        [SerializeField] private MenuButton _closeButton;
        [SerializeField] private MenuButton _openLevelButton;
        [SerializeField] private MenuButton _openAllButton;
        [SerializeField] private GameObject _levelInfo;
        [SerializeField] private GameObject _allInfo;

        private UnityAction _onShowGallaryPanel;

        public void Init(UnityAction onShowGallaryPanel)
        {
            MenuPanelInit();
            _onShowGallaryPanel = onShowGallaryPanel;
            _closeButton.Init(_onShowGallaryPanel);
            _openLevelButton.Init(OpenLevel);
            _openAllButton.Init(OpenAll);
            _levelInfo.SetActive(false);
            _allInfo.SetActive(false);
        }

        protected override void OnEndShow()
        {
            _levelInfo.SetActive(true);
            _allInfo.SetActive(true);
            _closeButton.Show();
            _openLevelButton.Show();
            _openAllButton.Show();
        }

        protected override void OnHide()
        {
            _levelInfo.SetActive(false);
            _allInfo.SetActive(false);
            _closeButton.Hide();
            _openLevelButton.Hide();
            _openAllButton.Hide();
        }

        protected override void OnStartShow() { }

        private void OpenLevel()
        {
            //Debug.Log(GameData.CurrentLevel);
            var itemShop = NutakuAPIInitializator.instance.PuarchaseService.GetShopItemShowGirl(GameData.CurrentLevel);
            NutakuAPIInitializator.instance.PuarchaseService.PurchaseItem(itemShop);

            //GameData.Levels.SetOpened(GameData.CurrentLevel);
            //_onShowGallaryPanel?.Invoke();
        }

        private void OpenAll()
        {
            var itemShop = NutakuAPIInitializator.instance.PuarchaseService.GetShopItemShowGirls();
            NutakuAPIInitializator.instance.PuarchaseService.PurchaseItem(itemShop);

            //GameData.Levels.OpenAll();
            //_onShowGallaryPanel?.Invoke();
        }
    }
}