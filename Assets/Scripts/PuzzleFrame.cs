using UnityEngine;

namespace MyGame
{
    public sealed class PuzzleFrame
    {
        public void Create(Transform gameParent, Texture2D sourceTexture, Vector2Int partsSize)
        {
            GameObject mask = new("Mask");
            SpriteMask spriteMask = mask.AddComponent<SpriteMask>();
            Texture2D maskTexture = new Texture2D(partsSize.x, partsSize.y, TextureFormat.RGBA32, false);
            Sprite maskSprite = Sprite.Create(maskTexture, new Rect(0, 0, maskTexture.width, maskTexture.height), new Vector2(0.5f, 0.5f));
            spriteMask.sprite = maskSprite;
            if (partsSize.x % 2 == 1)
                mask.transform.position -= Vector3.right * 0.005f;
            if (partsSize.y % 2 == 1)
                mask.transform.position -= Vector3.up * 0.005f;
            mask.transform.SetParent(gameParent.transform);

            GameObject frame = new("Frame");
            SpriteRenderer spriteRendererFrame = frame.AddComponent<SpriteRenderer>();
            spriteRendererFrame.sprite = Sprite.Create(sourceTexture, new Rect(0, 0, sourceTexture.width, sourceTexture.height), new Vector2(0.5f, 0.5f));
            spriteRendererFrame.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            spriteRendererFrame.transform.SetParent(gameParent.transform);
        }
    }
}