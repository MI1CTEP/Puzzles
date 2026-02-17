using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;
using MyGame.Bundles;

namespace MyGame.Menu
{
    public sealed class GallaryPanel : MenuPanel
    {
        [SerializeField] private UpInfoPanel _upInfoPanel;
        [SerializeField] private Image _contentImage;
        [SerializeField] private SympathyPanel _sympathyPanel;
        [SerializeField] private AchievemetnsPanel _achievemetnsPanel;
        [SerializeField] private MenuButton _buttonPrevious;
        [SerializeField] private MenuButton _buttonNext;
        [SerializeField] private MenuButton _buttonClose;
        [SerializeField] private MenuButton _buttonPlay;
        [SerializeField] private MenuButton _buttonAlbum;
        [SerializeField] private MenuButton _buttonHistory;
        [SerializeField] private MenuButton _buttonOpenLevel;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private GameObject _extraLevelInfo;
        [SerializeField] private GameObject _lock;
        [SerializeField] private Image _priceLogo;
        [SerializeField] private ContentDownloadingPanel _contentDownloadingPanel;
        [SerializeField] private Sprite _downloadBackground;
        [SerializeField] private Sprite _respectIcon;
        [SerializeField] private Sprite _detailIcon;

        private RespectPanel _respectPanel;
        private BundlesController _bundlesController;
        private Sequence _seqContentShow;
        private UnityAction _onShowUnlockPanel;

        public void Init(RespectPanel respectPanel, UnityAction onShowMainPanel, UnityAction onShowStoryPanel, UnityAction onShowAnbumPanel, UnityAction onShowUnlockPanel)
        {
            _respectPanel = respectPanel;
            _bundlesController = BundlesController.Instance;
            _bundlesController.OnStartDownloadLevel += TrySetDownloading;
            _bundlesController.OnEndDownloadLevel += TryActivateLevel;
            MenuPanelInit();
            _upInfoPanel.Init(_buttonPlay, _contentImage, _bundlesController.LevelsCount);
            _sympathyPanel.Init();
            _achievemetnsPanel.Init();
            gameObject.SetActive(false);
            _buttonPrevious.Init(() => SwitchLevel(-1));
            _buttonNext.Init(() => SwitchLevel(1));
            _buttonClose.Init(onShowMainPanel);

            _buttonPlay.Init(SceneLoader.LoadGameplay);
            //_buttonPlay.Init(SceneLoader.LoadMenuScenarioScene);

            _buttonAlbum.Init(onShowAnbumPanel);
            _buttonHistory.Init(onShowStoryPanel);
            _buttonOpenLevel.Init(OpenLevel);
            _onShowUnlockPanel = onShowUnlockPanel;
        }

        protected override void OnStartShow() 
        {
            SetStartColor();
        }

        protected override void OnEndShow()
        {
            _seq = DOTween.Sequence();
            _seq.InsertCallback(0.2f, ()=> 
            {
                _upInfoPanel.SetActive(true);
                _buttonPrevious.Show();
                _buttonNext.Show();
                _buttonClose.Show();
                SwitchLevel(0);
            });
        }

        protected override void OnHide()
        {
            _upInfoPanel.SetActive(false);
            _buttonPrevious.Hide();
            _buttonNext.Hide();
            _buttonClose.Hide();
            _buttonPlay.Hide();
            _buttonAlbum.Hide();
            _buttonHistory.Hide();
            _buttonOpenLevel.Hide();
            _sympathyPanel.Hide();
            _achievemetnsPanel.Hide();
            _lock.SetActive(false);
            _contentImage.color = new Color(1, 1, 1, 0);
        }

        public void SwitchLevel(int value)
        {
            GameData.CurrentLevel += value;
            if (GameData.CurrentLevel >= _bundlesController.LevelsCount)
                GameData.CurrentLevel = 0;
            else if (GameData.CurrentLevel < 0)
                GameData.CurrentLevel = _bundlesController.LevelsCount - 1;

            if (_bundlesController.TypeLevelStatus(GameData.CurrentLevel) == TypeLevelStatus.NotDownloaded)
                SetNotDownloaded();
            else if (_bundlesController.TypeLevelStatus(GameData.CurrentLevel) == TypeLevelStatus.Downloading)
                SetDownloading();
            else
                SetDownloaded();
        }


        public void SetLevel(int value)
        {
            GameData.CurrentLevel = 0;

            if (_bundlesController.TypeLevelStatus(GameData.CurrentLevel) == TypeLevelStatus.NotDownloaded)
                SetNotDownloaded();
            else if (_bundlesController.TypeLevelStatus(GameData.CurrentLevel) == TypeLevelStatus.Downloading)
                SetDownloading();
            else
                SetDownloaded();

        }


        private void TrySetDownloading(int id)
        {
            if (GameData.CurrentLevel == id)
                SetDownloading();
        }

        private void TryActivateLevel(int id)
        {
            if (GameData.CurrentLevel == id)
                SetDownloaded();
        }

        private void SetNotDownloaded()
        {
            _upInfoPanel.UpdateAll(GameData.CurrentLevel + 1, "not downloaded");
            _contentImage.sprite = _downloadBackground;
            _contentDownloadingPanel.Show(false);
            _buttonPlay.Hide();
            _buttonAlbum.Hide();
            _buttonHistory.Hide();
            _buttonOpenLevel.Hide();
            _sympathyPanel.Hide();
            _achievemetnsPanel.Hide();
            _priceText.gameObject.SetActive(false);
            _extraLevelInfo.SetActive(false);
            _lock.SetActive(false);
        }

        private void SetDownloading()
        {
            _upInfoPanel.UpdateAll(GameData.CurrentLevel + 1, "downloading");
            _contentImage.sprite = _downloadBackground;
            _contentDownloadingPanel.Show(true);
            _buttonPlay.Hide();
            _buttonAlbum.Hide();
            _buttonHistory.Hide();
            _buttonOpenLevel.Hide();
            _sympathyPanel.Hide();
            _achievemetnsPanel.Hide();
            _priceText.gameObject.SetActive(false);
            _extraLevelInfo.SetActive(false);
            _lock.SetActive(false);
        }

        private void SetDownloaded()
        {
            _contentDownloadingPanel.Hide();
            _upInfoPanel.OnStartLoad();
            if (IsOpenedLevel())
            {
                SetOpened();
                _bundlesController.MainResourcesBundle.Load(GameData.CurrentLevel, () => EndLoad(true));
            }
            else
            {
                SetClosed();
                _bundlesController.MainResourcesBundle.Load(GameData.CurrentLevel, () => EndLoad(false));
            }
        }

        private bool IsOpenedLevel()
        {
            Level level = _bundlesController.LevelsInfo.Level(GameData.CurrentLevel);
            if ((level.price == 0 || GameData.Levels.IsOpenedAll()) && level.type != "Extra" || GameData.Levels.IsOpened(GameData.CurrentLevel))
                return true;

            return false;
        }

        private void SetOpened()
        {
            _buttonPlay.Show();
            _buttonAlbum.Show();
            _buttonHistory.Show();
            _sympathyPanel.UpdateValue(GameData.Sympathy.Load(GameData.CurrentLevel));
            _sympathyPanel.Show();
            _achievemetnsPanel.Show();
            _buttonOpenLevel.Hide();
            _priceText.gameObject.SetActive(false);
            _extraLevelInfo.SetActive(false);
            _lock.SetActive(false);
        }

        private void SetClosed()
        {
            _buttonPlay.Hide();
            _buttonAlbum.Hide();
            _buttonHistory.Hide();
            _sympathyPanel.Hide();
            _achievemetnsPanel.Hide();
            _priceText.gameObject.SetActive(true);
            _lock.SetActive(true);
            if (_bundlesController.LevelsInfo.Level(GameData.CurrentLevel).type == "Extra")
            {
                _priceLogo.sprite = _detailIcon;
                _priceText.text = $"{GameData.ExtraLevel.GetOpenedPartsValue()}/{GameData.ExtraLevel.PartSize.x * GameData.ExtraLevel.PartSize.y}";
                _buttonOpenLevel.Hide();
                _extraLevelInfo.SetActive(true);
            }
            else
            {
                _priceLogo.sprite = _respectIcon;
                _priceText.text = _bundlesController.LevelsInfo.Level(GameData.CurrentLevel).price.ToString();
                _buttonOpenLevel.Show();
                _extraLevelInfo.SetActive(false);
            }
        }

        private void OpenLevel()
        {
            int price = _bundlesController.LevelsInfo.Level(GameData.CurrentLevel).price;
            if (GameData.Respect.Load() >= price)
            {
                GameData.Respect.Add(-price);
                GameData.Levels.SetOpened(GameData.CurrentLevel);
                _respectPanel.UpdateView();
            }
            else _onShowUnlockPanel?.Invoke();
        }

        private void EndLoad(bool isOpened)
        {
            _upInfoPanel.OnEndLoad();
            _upInfoPanel.UpdateAll(GameData.CurrentLevel + 1, _bundlesController.MainResourcesBundle.GetName);
            SetStartColor();
            _contentImage.sprite = _bundlesController.MainResourcesBundle.Sprites[0];
            TryStopAnim();
            _seqContentShow = DOTween.Sequence();
            _seqContentShow.Insert(0, _contentImage.DOFade(1, 0.2f));
        }

        private void SetStartColor()
        {
            if (IsOpenedLevel())
                _contentImage.color = new Color(1, 1, 1, 0);
            else
                _contentImage.color = new Color(0.1f, 0.1f, 0.25f, 0);
        }

        private void TryStopAnim()
        {
            if (_seqContentShow != null)
                _seqContentShow.Kill();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _bundlesController.OnStartDownloadLevel -= TrySetDownloading;
            _bundlesController.OnEndDownloadLevel -= TryActivateLevel;
            TryStopAnim();
        }
    }
}