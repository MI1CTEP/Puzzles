using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Bundles
{
    public sealed class MainResourcesBundle : Bundle
    {
        private Info _info;

        public List<Sprite> Sprites { get; set; } = new();
        public string GetName => _info.name;
        public Languages GetInfoLanguages => _info.languages;

        public override string GetFileName(int id) => $"scenario_{id + 1}.main_resources";

        protected override void LoadResources()
        {
            Sprites.Clear();
            bool isHaveResource = true;
            int id = 1;
            while (isHaveResource)
            {
                Sprite sprite = _bundle.LoadAsset<Sprite>($"image_{id}");
                if (sprite != null)
                {
                    Sprites.Add(sprite);
                    id++;
                }
                else isHaveResource = false;
            }

            TextAsset infoTextAsset = _bundle.LoadAsset<TextAsset>($"info");
            _info = JsonConvert.DeserializeObject<Info>(infoTextAsset.text);
        }

        private class Info
        {
            public string name;
            public Languages languages;
        }
    }
}