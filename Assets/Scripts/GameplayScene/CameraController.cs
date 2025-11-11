using UnityEngine;

namespace MyGame.Gameplay
{
    public sealed class CameraController : MonoBehaviour
    {
        private Camera _camera;

        public void Init()
        {
            _camera = Camera.main;
        }

        public void UpdateSize(float contentWidth)
        {
            float aspectRatio = (float)Screen.height / Screen.width;
            _camera.orthographicSize = aspectRatio * contentWidth / 200;
        }
    }
}
