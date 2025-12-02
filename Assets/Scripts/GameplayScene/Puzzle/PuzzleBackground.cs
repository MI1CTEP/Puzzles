using UnityEngine;

namespace MyGame.Gameplay.Puzzle
{
    public sealed class PuzzleBackground
    {
        private SpriteRenderer _spriteRenderer;
        private Transform _puzzleParent;

        public PuzzleBackground(Transform puzzleParent)
        {
            _puzzleParent = puzzleParent;

            GameObject mask = new("Mask");
            _spriteRenderer = mask.AddComponent<SpriteRenderer>();
            mask.transform.SetParent(_puzzleParent);
        }

        public void SetMask(Vector2Int partsSize)
        {
            Texture2D maskTexture = new Texture2D(partsSize.x, partsSize.y, TextureFormat.RGBA32, false);
            Sprite maskSprite = Sprite.Create(maskTexture, new Rect(0, 0, maskTexture.width, maskTexture.height), new Vector2(0.5f, 0.5f));
            _spriteRenderer.sprite = maskSprite;
            _spriteRenderer.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);
            _spriteRenderer.transform.position = Vector3.zero;
            if (partsSize.x % 2 == 1)
                _spriteRenderer.transform.position = -Vector3.right * 0.005f;
            if (partsSize.y % 2 == 1)
                _spriteRenderer.transform.position = -Vector3.up * 0.005f;
            SetActiveMask(true);
        }

        public void SetActiveMask(bool value)
        {
            _spriteRenderer.gameObject.SetActive(value);
        }
    }
}