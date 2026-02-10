using UnityEngine;
using MyGame.Bundles;
using Cysharp.Threading.Tasks;

namespace MyGame
{
    public class MainInitializer : MonoBehaviour
    {
        [SerializeField] private BundlesController _bundlesController;

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = -1;

            Input.multiTouchEnabled = false;

            GameData.CurrentLevel = 0;
            _bundlesController.Init(SceneLoader.LoadMenu);
        }
    }
}