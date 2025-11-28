using UnityEngine;

namespace MyGame.Gameplay.Puzzle
{
    public sealed class PuzzleBackground
    {
        private SpriteMask _spriteMask;
        private Transform _puzzleParent;

        public PuzzleBackground(Transform puzzleParent)
        {
            _puzzleParent = puzzleParent;

            GameObject mask = new("Mask");
            _spriteMask = mask.AddComponent<SpriteMask>();
            _spriteMask.transform.SetParent(_puzzleParent);
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

        public void SetActiveMask(bool value)
        {
            _spriteMask.gameObject.SetActive(value);
        }
    }
}