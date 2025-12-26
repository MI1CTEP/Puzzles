using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyGame.Bundles;

namespace MyGame.MenuTemp
{
    public sealed class LevelController : MonoBehaviour
    {
        [SerializeField] private Image _mainImage;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Button _previousLevelButton;
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private GameObject _downloadingImage;
        [SerializeField] private LevelDownloadProgress _levelDownloadProgress;
        [SerializeField] private Button _startGameplayButton;
        [SerializeField] private Button _previousImageButton;
        [SerializeField] private Button _nextImageButton;

        private BundlesController _bundlesController;
        private int _currentImageId;

        public void Init()
        {
            _bundlesController = BundlesController.Instance;
            _bundlesController.OnStartDownloadLevel += TrySetDownloading;
            _bundlesController.OnEndDownloadLevel += TryActivateLevel;
            _previousLevelButton.onClick.AddListener(() => SwitchLevel(-1));
            _nextLevelButton.onClick.AddListener(() => SwitchLevel(1));
            _startGameplayButton.onClick.AddListener(SceneLoader.LoadGameplay);
            _previousImageButton.onClick.AddListener(() => SetMainImage(_currentImageId - 1));
            _nextImageButton.onClick.AddListener(() => SetMainImage(_currentImageId + 1));
            SwitchLevel(0);
        }

        private void SwitchLevel(int value)
        {
            GameData.CurrentLevel += value;
            if (GameData.CurrentLevel >= _bundlesController.LevelsCount)
                GameData.CurrentLevel = 0;
            else if (GameData.CurrentLevel < 0)
                GameData.CurrentLevel = _bundlesController.LevelsCount - 1;
            _levelText.text = $"Уровень {GameData.CurrentLevel + 1}/{_bundlesController.LevelsCount}";

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
            _downloadingImage.SetActive(true);
            _levelDownloadProgress.SetActive(false);
            SetEnableNavigation(false);
        }

        private void SetDownloading()
        {
            _downloadingImage.SetActive(false);
            _levelDownloadProgress.SetActive(true);
            SetEnableNavigation(false);
        }

        private void SetDownloaded()
        {
            _downloadingImage.SetActive(false);
            _levelDownloadProgress.SetActive(false);
            _bundlesController.MainResourcesBundle.Load(GameData.CurrentLevel, EndLoad);
            SetEnableNavigation(true);
        }

        private void EndLoad()
        {
            _nameText.text = _bundlesController.MainResourcesBundle.GetName;
            SetMainImage(0);
        }

        private void SetMainImage(int id)
        {
            _currentImageId = id;
            if (_currentImageId < 0)
                _currentImageId = _bundlesController.MainResourcesBundle.Sprites.Count - 1;
            if (_currentImageId >= _bundlesController.MainResourcesBundle.Sprites.Count)
                _currentImageId = 0;
            _mainImage.sprite = _bundlesController.MainResourcesBundle.Sprites[_currentImageId];
        }

        private void SetEnableNavigation(bool value)
        {
            _startGameplayButton.interactable = value;
            _previousImageButton.interactable = value;
            _nextImageButton.interactable = value;
        }

        private void OnDestroy()
        {
            _bundlesController.OnStartDownloadLevel -= TrySetDownloading;
            _bundlesController.OnEndDownloadLevel -= TryActivateLevel;
            _levelDownloadProgress.SetActive(false);
        }
    }
}