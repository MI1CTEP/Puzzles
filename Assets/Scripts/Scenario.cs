using UnityEngine;

namespace MyGame
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Scenario")]
    public sealed class Scenario : ScriptableObject
    {
        public ScenarioStage[] scenarioStages;

        public ScenarioStage TryGetScenarioStage(int id)
        {
            if (id < scenarioStages.Length)
                return scenarioStages[id];

            return null;
        }
    }

    [System.Serializable]
    public sealed class ScenarioStage
    {
        public TypeStage typeStage;
        public int imageId;
        public int id;
    }

    public enum TypeStage
    {
        Dialogue, Puzzle, Video, PaidContent, Gifts
    }
}