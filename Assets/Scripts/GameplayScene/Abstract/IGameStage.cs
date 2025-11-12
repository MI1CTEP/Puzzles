using UnityEngine.Events;

namespace MyGame.Gameplay
{
    public interface IGameStage
    {
        UnityAction OnEnd { get; set; }

        void Play(ScenarioStage scenarioStage);
    }
}