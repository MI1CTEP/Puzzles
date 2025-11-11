using System;
using UnityEngine;
using UnityEngine.Events;

namespace MyGame.Gameplay.Puzzle
{
    public sealed class PuzzleController : MonoBehaviour, IGameStage
    {
        [SerializeField] private PuzzlePart _partPrefab;

        private PuzzleBoard _board;
        private PuzzlePartsPool _partsPool;
        private PuzzlePartsGenerator _partsGenerator;
        private PuzzleMixer _mixer;
        private PuzzleBackground _background;
        private PuzzlePart[,] _parts;
        private Vector2Int _partsLength;

        public UnityAction OnEnd { get; set; }

        public void Init()
        {
            _board = new();
            _partsPool = new(_partPrefab, _board);
            _partsGenerator = new(_partsPool, transform);
            _mixer = new();
            _background = new(transform);
        }

        public void Play(ScenarioStage scenarioStage)
        {
            gameObject.SetActive(true);

            for (int y = 0; y < _partsLength.y; y++)
                for (int x = 0; x < _partsLength.x; x++)
                    _partsPool.Return(_parts[x, y]);

            _parts = _partsGenerator.GetPuzzleParts(scenarioStage.Sprite.texture, scenarioStage.PuzzleValueX);
            _partsLength = new(_parts.GetUpperBound(0) + 1, _parts.GetUpperBound(1) + 1);
            _board.ResetProgress();
            _board.SetParts(_parts, _partsLength);
            _mixer.MixParts(_parts, _partsLength);
            _board.SetPartsPosition();
            _board.OnComplete += End;
            _background.SetMask(_partsLength * _partsGenerator.PartSize);
            _background.SetBackground(scenarioStage.Sprite.texture);
        }

        private void End()
        {
            gameObject.SetActive(false);
            OnEnd?.Invoke();
        }

        private void OnDestroy()
        {
            _board.OnComplete -= End;
        }
    }
}