using UnityEngine;
using Cysharp.Threading.Tasks;

namespace MyGame
{
    public class MainInitializer : MonoBehaviour
    {
        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = -1;

            Input.multiTouchEnabled = false;

            GameData.CurrentLevel = 0;
            SceneLoader.LoadMenu();
        }
    }
}