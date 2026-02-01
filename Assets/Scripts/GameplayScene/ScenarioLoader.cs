using UnityEngine;
using Newtonsoft.Json;
using MyGame.Bundles;
using Cysharp.Threading.Tasks;

namespace MyGame.Gameplay
{
    public sealed class ScenarioLoader
    {
        private BundlesController _bundlesController;

        public ScenarioLoader()
        {
            _bundlesController = BundlesController.Instance;
        }

        public async UniTask<Scenario> GetScenario()
        {
            await _bundlesController.OnlyGameplayBundle.Load(GameData.CurrentLevel, null);

            Scenario scenario = _bundlesController.OnlyGameplayBundle.Scenario;

            for (int i = 0; i < scenario.scenarioStages.Count; i++)
            {
                ScenarioStage stage = scenario.scenarioStages[i];

                if (stage.typeStage == "Dialogue" || stage.typeStage == "Gifts" || stage.typeStage == "Puzzle")
                {
                    stage.Image = _bundlesController.MainResourcesBundle.Sprites[stage.imageId - 1];
                }

                if (stage.typeStage == "Dialogue")
                {
                    stage.Dialogue = _bundlesController.OnlyGameplayBundle.SimpleDialogues[stage.dialogueId - 1];
                }
                else if (stage.typeStage == "Video")
                {
                    stage.Video = _bundlesController.OnlyGameplayBundle.Videos[stage.videoId - 1];
                }
            }

            return scenario;
        }


        public async UniTask<Scenario> GetScenario(int idLevelGirl)
        {
            await _bundlesController.OnlyGameplayBundle.Load(idLevelGirl, null);

            Scenario scenario = _bundlesController.OnlyGameplayBundle.Scenario;

            for (int i = 0; i < scenario.scenarioStages.Count; i++)
            {
                ScenarioStage stage = scenario.scenarioStages[i];

                if (stage.typeStage == "Dialogue" || stage.typeStage == "Gifts" || stage.typeStage == "Puzzle")
                {
                    stage.Image = _bundlesController.MainResourcesBundle.Sprites[stage.imageId - 1];
                }

                if (stage.typeStage == "Dialogue")
                {
                    stage.Dialogue = _bundlesController.OnlyGameplayBundle.SimpleDialogues[stage.dialogueId - 1];
                }
                else if (stage.typeStage == "Video")
                {
                    stage.Video = _bundlesController.OnlyGameplayBundle.Videos[stage.videoId - 1];
                }
            }

            return scenario;
        }
    }
}