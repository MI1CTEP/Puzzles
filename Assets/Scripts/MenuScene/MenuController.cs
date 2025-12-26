using UnityEngine;

namespace MyGame.Menu
{
    public sealed class MenuController : MonoBehaviour
    {
        [SerializeField] private MainPanel _mainPanel;
        [SerializeField] private GallaryPanel _gallaryPanel;
        [SerializeField] private StoryPanel _storyPanel;
        [SerializeField] private AlbumPanel _albumPanel;
        [SerializeField] private SettingsPanel _settingsPanel;

        private void Start()
        {
            _mainPanel.Init(ShowGallaryPanel, ShowSettingsPanel);
            MenuPanel.CurrentMenuPanel = _mainPanel;
            _gallaryPanel.Init(ShowMainPanel, ShowStoryPanel, ShowAlbumPanel);
            _storyPanel.Init(ShowGallaryPanel);
            _albumPanel.Init(ShowGallaryPanel);
            _settingsPanel.Init(ShowGallaryPanel);
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
    }
}