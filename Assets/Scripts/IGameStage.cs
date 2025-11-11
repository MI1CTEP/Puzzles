using UnityEngine.Events;

namespace MyGame
{
    public interface IGameStage
    {
        UnityAction OnEnd { get; set; }

        void Play(ScenarioStage scenarioStage);
    }
}