using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Gameplay
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Scenario")]
    public sealed class Scenario : ScriptableObject
    {
        [SerializeField] private List<ScenarioStage> _scenarioStages;

        public ScenarioStage TryGetScenarioStage(int id)
        {
            if (id < _scenarioStages.Count)
                return _scenarioStages[id];

            return null;
        }
    }
}