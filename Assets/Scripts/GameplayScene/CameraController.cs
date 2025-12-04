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

        public void UpdateSize(float contentHeight,  float contentWidth)
        {
            float contentRatio = contentHeight / contentWidth;
            float aspectRatio = (float)Screen.height / Screen.width;
            if(aspectRatio < contentRatio)
            {
                _camera.orthographicSize = aspectRatio * contentWidth / 200;
            }
            else
            {
                _camera.orthographicSize = contentRatio * contentWidth / 200;
            }
        }
    }
}
