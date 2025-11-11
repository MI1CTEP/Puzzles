using UnityEngine;
using UnityEngine.Events;

namespace MyGame.Gameplay
{
    public sealed class BackgroundController : MonoBehaviour, IGameStage
    {
        private SpriteRenderer _spriteRenderer;

        public UnityAction OnEnd { get; set; }

        public void Init()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Play(ScenarioStage scenarioStage)
        {
            _spriteRenderer.sprite = scenarioStage.Sprite;
            OnEnd?.Invoke();
        }
    }
}