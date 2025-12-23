using UnityEngine;
using UnityEngine.UI;
using MyGame.Bundles;

namespace MyGame.MenuTemp
{
    public sealed class MenuTempController : MonoBehaviour
    {
        [SerializeField] private LevelController _levelController;
        [SerializeField] private DownloadInfoPanel _downloadInfoPanel;

        private void Start()
        {
            _levelController.Init();
            _downloadInfoPanel.Init();
        }
    }
}