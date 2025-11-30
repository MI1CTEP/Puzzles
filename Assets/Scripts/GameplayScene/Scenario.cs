using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using MyGame.Gameplay.Dialogue;

namespace MyGame.Gameplay
{
    public sealed class Scenario
    {
        public List<ScenarioStage> scenarioStages;

        public ScenarioStage TryGetScenarioStage(int id)
        {
            if (id < scenarioStages.Count)
                return scenarioStages[id];

            return null;
        }
    }

    [System.Serializable]
    public sealed class ScenarioStage
    {
        public string typeStage;
        public int dialogueId;
        public int videoId;
        public int imageId;
        public bool isAnimImage;
        public int easyValueX;
        public int mediumValueX;
        public int hardValueX;

        public SimpleDialogue Dialogue { get; set; }
        public Sprite Image { get; set; }
        public VideoClip Video { get; set; }
    }
}