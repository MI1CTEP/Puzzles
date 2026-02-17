using UnityEngine;

namespace MyGame.Menu
{
    public sealed class MenuController : MonoBehaviour
    {
        [SerializeField] private RespectPanel _respectPanel;
        [SerializeField] private MainPanel _mainPanel;
        [SerializeField] private GallaryPanel _gallaryPanel;
        [SerializeField] private StoryPanel _storyPanel;
        [SerializeField] private AlbumPanel _albumPanel;
        [SerializeField] private SettingsPanel _settingsPanel;
        [SerializeField] private UnlockPanel _unlockPanel;

        private void Start()
        {
            _respectPanel.Init();
            _mainPanel.Init(ShowGallaryPanel, ShowSettingsPanel);
            MenuPanel.CurrentMenuPanel = _mainPanel;
            _gallaryPanel.Init(_respectPanel, ShowMainPanel, ShowStoryPanel, ShowAlbumPanel, ShowUnlockPanel);
            _storyPanel.Init(ShowGallaryPanel);
            _albumPanel.Init(ShowGallaryPanel);
            _settingsPanel.Init(ShowMainPanel);
            _unlockPanel.Init(ShowGallaryPanel, _gallaryPanel.SetLevel);
        }

        private void ShowMainPanel()
        {
            _mainPanel.Show();
        }

        private void ShowGallaryPanel()
        {
            _gallaryPanel.Show();
        }

        private void ShowStoryPanel()
        {
            _storyPanel.Show();
        }

        private void ShowAlbumPanel()
        {
            _albumPanel.Show();
        }

        private void ShowSettingsPanel()
        {
            _settingsPanel.Show();
        }

        private void ShowUnlockPanel()
        {
            _unlockPanel.Show();
        }
    }
}