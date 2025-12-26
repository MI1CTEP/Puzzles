using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
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
        [SerializeField] private ContentDownloadingPanel _contentDownloadingPanel;
        [SerializeField] private Sprite _downloadBackground;

        private BundlesController _bundlesController;
        private Sequence _seqContentShow;

        public void Init(UnityAction onShowMainPanel, UnityAction onShowStoryPanel, UnityAction onShowAnbumPanel)
        {
            _bundlesController = BundlesController.Instance;
            _bundlesController.OnStartDownloadLevel += TrySetDownloading;
            _bundlesController.OnEndDownloadLevel += TryActivateLevel;
            MenuPanelInit();
            _upInfoPanel.Init(_bundlesController.LevelsCount);
            _sympathyPanel.Init();
            _achievemetnsPanel.Init();
            gameObject.SetActive(false);
            _buttonPrevious.Init(() => SwitchLevel(-1));
            _buttonNext.Init(() => SwitchLevel(1));
            _buttonClose.Init(onShowMainPanel);
            _buttonPlay.Init(SceneLoader.LoadGameplay);
            _buttonAlbum.Init(onShowAnbumPanel);
            _buttonHistory.Init(onShowStoryPanel);
        }

        protected override void OnStartShow() { }

        protected override void OnEndShow()
        {
            SwitchLevel(0);
            _contentImage.color = Color.clear;
            _seq = DOTween.Sequence();
            _seq.Insert(0, _contentImage.DOColor(Color.white, 0.2f));
            _seq.InsertCallback(0.2f, ()=> 
            {
                _upInfoPanel.SetActive(true);
                _buttonPrevious.Show();
                _buttonNext.Show();
                _buttonClose.Show();
                _buttonPlay.Show();
                _buttonAlbum.Show();
                _buttonHistory.Show();
            });
        }

        protected override void OnHide()
        {
            _upInfoPanel.SetActive(false);
            _buttonPrevious.gameObject.SetActive(false);
            _buttonNext.gameObject.SetActive(false);
            _buttonClose.gameObject.SetActive(false);
            _buttonPlay.Hide();
            _buttonAlbum.Hide();
            _buttonHistory.Hide();
            _sympathyPanel.Hide();
            _achievemetnsPanel.Hide();
            _seq.Insert(0, _contentImage.DOColor(Color.clear, 0.2f));
        }

        private void SwitchLevel(int value)
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
            _upInfoPanel.UpdateText(GameData.CurrentLevel + 1, "В очереди на загрузку");
            _contentImage.sprite = _downloadBackground;
            _contentDownloadingPanel.Show(false);
            _buttonPlay.SetInteractable(false);
            _buttonAlbum.SetInteractable(false);
            _buttonHistory.SetInteractable(false);
            _sympathyPanel.Hide();
            _achievemetnsPanel.Hide();
        }

        private void SetDownloading()
        {
            _upInfoPanel.UpdateText(GameData.CurrentLevel + 1, "Загружается");
            _contentImage.sprite = _downloadBackground;
            _contentDownloadingPanel.Show(true);
            _buttonPlay.SetInteractable(false);
            _buttonAlbum.SetInteractable(false);
            _buttonHistory.SetInteractable(false);
            _sympathyPanel.Hide();
            _achievemetnsPanel.Hide();
        }

        private void SetDownloaded()
        {
            _contentDownloadingPanel.Hide();
            _buttonPlay.SetInteractable(true);
            _buttonAlbum.SetInteractable(true);
            _buttonHistory.SetInteractable(true);
            _sympathyPanel.UpdateValue(GameData.Sympathy.Load(GameData.CurrentLevel));
            _sympathyPanel.Show();
            _achievemetnsPanel.Show();
            _bundlesController.MainResourcesBundle.Load(GameData.CurrentLevel, EndLoad);
        }

        private void EndLoad()
        {
            _upInfoPanel.UpdateText(GameData.CurrentLevel + 1, _bundlesController.MainResourcesBundle.GetName);
            _contentImage.color = new Color(1, 1, 1, 0);
            _contentImage.sprite = _bundlesController.MainResourcesBundle.Sprites[0];
            TryStopAnim();
            _seqContentShow = DOTween.Sequence();
            _seqContentShow.Insert(0, _contentImage.DOFade(1, 0.2f));
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