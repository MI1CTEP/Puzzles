using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using MyGame.Gameplay.Dialogue;
using Newtonsoft.Json;

namespace MyGame.Gameplay
{
    public sealed class ScenarioLoader
    {
        [SerializeField] private readonly List<SimpleDialogue> _simpleDialogues;
        [SerializeField] private readonly List<Sprite> _images;
        [SerializeField] private readonly List<VideoClip> _videos;

        public ScenarioLoader()
        {
            _simpleDialogues = new();
            _images = new();
            _videos = new();
        }

        public Scenario GetScenario(int levelId)
        {
            TextAsset jsonAssetScenario = Resources.Load<TextAsset>($"Scenarios/Scenario_{levelId}/scenario_{levelId}");
            Scenario scenario = JsonConvert.DeserializeObject<Scenario>(jsonAssetScenario.text);

            for (int i = 0; i < scenario.scenarioStages.Count; i++)
            {
                ScenarioStage stage = scenario.scenarioStages[i];

                if (stage.typeStage == "Dialogue" || stage.typeStage == "Gifts" || stage.typeStage == "Puzzle")
                {
                    if (_images.Count < stage.imageId)
                        for (int j = _images.Count; j <= stage.imageId; j++)
                            _images.Add(null);
                    if (_images[stage.imageId - 1] == null)
                    {
                        Sprite sprite = Resources.Load<Sprite>($"Scenarios/Scenario_{levelId}/image_{levelId}_{stage.imageId}");
                        _images[stage.imageId - 1] = sprite;
                    }
                    stage.Image = _images[stage.imageId - 1];
                }

                if (stage.typeStage == "Dialogue")
                {
                    if (_simpleDialogues.Count < stage.dialogueId)
                        for (int j = _simpleDialogues.Count; j <= stage.dialogueId; j++)
                            _simpleDialogues.Add(null);
                    if (_simpleDialogues[stage.dialogueId - 1] == null)
                    {
                        TextAsset jsonAssetDialogue = Resources.Load<TextAsset>($"Scenarios/Scenario_{levelId}/dialogue_{levelId}_{stage.dialogueId}");
                        SimpleDialogue simpleDialogue = JsonConvert.DeserializeObject<SimpleDialogue>(jsonAssetDialogue.text);
                        _simpleDialogues[stage.dialogueId - 1] = simpleDialogue;
                    }
                    stage.Dialogue = _simpleDialogues[stage.dialogueId - 1];
                }
                else if (stage.typeStage == "Video")
                {
                    if (_videos.Count < stage.videoId)
                        for (int j = _videos.Count; j <= stage.videoId; j++)
                            _videos.Add(null);
                    if (_videos[stage.videoId - 1] == null)
                    {
                        VideoClip video = Resources.Load<VideoClip>($"Scenarios/Scenario_{levelId}/video_{levelId}_{stage.imageId}");
                        _videos[stage.videoId - 1] = video;
                    }
                    stage.Video = _videos[stage.videoId - 1];
                }
            }

            return scenario;
        }
    }
}