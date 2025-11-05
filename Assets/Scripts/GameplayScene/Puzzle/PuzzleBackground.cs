using UnityEngine;
using Zenject;

namespace MyGame.Gameplay.Puzzle
{
    public sealed class PuzzleBackground : IInitializable
    {
        private SpriteMask _spriteMask;
        private SpriteRenderer _spriteRenderer;
        private Transform _puzzleParent;

        public PuzzleBackground(Transform puzzleParent)
        {
            _puzzleParent = puzzleParent;
        }

        public void Initialize()
        {
            GameObject mask = new("Mask");
            _spriteMask = mask.AddComponent<SpriteMask>();
            _spriteMask.transform.SetParent(_puzzleParent);

            GameObject frame = new("Background");
            _spriteRenderer = frame.AddComponent<SpriteRenderer>();
            _spriteRenderer.transform.SetParent(_puzzleParent);
        }

        public void SetMask(Vector2Int partsSize)
        {
            Texture2D maskTexture = new Texture2D(partsSize.x, partsSize.y, TextureFormat.RGBA32, false);
            Sprite maskSprite = Sprite.Create(maskTexture, new Rect(0, 0, maskTexture.width, maskTexture.height), new Vector2(0.5f, 0.5f));
            _spriteMask.sprite = maskSprite;
            _spriteMask.transform.position = Vector3.zero;
            if (partsSize.x % 2 == 1)
                _spriteMask.transform.position = -Vector3.right * 0.005f;
            if (partsSize.y % 2 == 1)
                _spriteMask.transform.position = -Vector3.up * 0.005f;
            SetActiveMask(true);
        }

        public void SetBackground(Texture2D sourceTexture)
        {
            _spriteRenderer.sprite = Sprite.Create(sourceTexture, new Rect(0, 0, sourceTexture.width, sourceTexture.height), new Vector2(0.5f, 0.5f));
            _spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }

        public void SetActiveMask(bool value)
        {
            _spriteMask.gameObject.SetActive(value);
        }
    }
}