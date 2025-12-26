using UnityEngine;
using MyGame.Bundles;

namespace MyGame
{
    public class MainInitializer : MonoBehaviour
    {
        [SerializeField] private BundlesController _bundlesController;

        private void Start()
        {
            GameData.CurrentLevel = 0;
            _bundlesController.Init(SceneLoader.LoadMenu);
        }
    }
}