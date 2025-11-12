using UnityEngine;
using UnityEngine.Events;

namespace MyGame.Gameplay
{
    public sealed class BackgroundController : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        public void Init()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }
    }
}