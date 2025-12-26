using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Bundles
{
    public sealed class ExtraImagesBundle : Bundle
    {
        public List<Sprite> Sprites { get; set; } = new();

        public override string GetFileName(int id) => $"scenario_{id + 1}.extra_images";

        protected override void LoadResources()
        {
            Sprites.Clear();
            bool isHaveResource = true;
            int id = 1;
            while (isHaveResource)
            {
                Sprite sprite = _bundle.LoadAsset<Sprite>($"extra_image_{id}");
                if (sprite != null)
                {
                    Sprites.Add(sprite);
                    id++;
                }
                else isHaveResource = false;
            }
        }
    }
}