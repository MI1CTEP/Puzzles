using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using MyGame.Gameplay.Dialogue;
using Newtonsoft.Json;
using MyGame.Gameplay;

namespace MyGame.Bundles
{
    public sealed class OnlyGameplayBundle : Bundle
    {
        public Scenario Scenario { get; set; }
        public List<SimpleDialogue> SimpleDialogues { get; set; } = new();
        public List<VideoClip> Videos { get; set; } = new();

        public override string GetFileName(int id) => $"scenario_{id + 1}.only_gameplay";

        protected override void LoadResources()
        {
            TextAsset scenarioTextAsset = _bundle.LoadAsset<TextAsset>($"scenario");
            Scenario = JsonConvert.DeserializeObject<Scenario>(scenarioTextAsset.text);

            SimpleDialogues.Clear();
            bool isHaveResource = true;
            int id = 1;
            while (isHaveResource)
            {
                TextAsset infoTextAsset = _bundle.LoadAsset<TextAsset>($"dialogue_{id}");
                if (infoTextAsset != null)
                {
                    SimpleDialogue simpleDialogue = JsonConvert.DeserializeObject<SimpleDialogue>(infoTextAsset.text);
                    SimpleDialogues.Add(simpleDialogue);
                    id++;
                }
                else isHaveResource = false;
            }

            Videos.Clear();
            isHaveResource = true;
            id = 1;
            while (isHaveResource)
            {
                VideoClip video = _bundle.LoadAsset<VideoClip>($"video_{id}");
                if (video != null)
                {
                    Videos.Add(video);
                    id++;
                }
                else isHaveResource = false;
            }
        }
    }
}