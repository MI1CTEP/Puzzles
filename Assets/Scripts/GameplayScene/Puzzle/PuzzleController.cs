using UnityEngine;
using UnityEngine.Events;
using MyGame.Gameplay.ExtraLevel;

namespace MyGame.Gameplay.Puzzle
{
    public sealed class PuzzleController : MonoBehaviour, IGameStage
    {
        [SerializeField] private PuzzlePart _partPrefab;
        [SerializeField] private PuzzleChoices _puzzleChoices;

        private ProgressPanel _progressPanel;
        private ExtraLevelUnlocker _extraLevelUnlocker;
        private PuzzleBoard _board;
        private PuzzlePartsPool _partsPool;
        private PuzzlePartsGenerator _partsGenerator;
        private PuzzleMixer _mixer;
        private PuzzleBackground _background;
        private PuzzlePart[,] _parts;
        private ScenarioStage _scenarioStage;
        private Vector2Int _partsLength;
        private float _chanceGetDetail;

        public TypeDifficulty TypeDifficulty { get; set; } = TypeDifficulty.None;
        public UnityAction OnEnd { get; set; }

        public void Init(ProgressPanel progressPanel, ExtraLevelUnlocker extraLevelUnlocker)
        {
            _progressPanel = progressPanel;
            _extraLevelUnlocker = extraLevelUnlocker;
            _board = new();
            _board.OnComplete += End;
            _partsPool = new(_partPrefab, _board);
            _partsGenerator = new(_partsPool, transform);
            _mixer = new();
            _background = new(transform);
            _puzzleChoices.Init(this);
        }

        public void Play(ScenarioStage scenarioStage)
        {
            _scenarioStage = scenarioStage;
            gameObject.SetActive(true);

            for (int y = 0; y < _partsLength.y; y++)
                for (int x = 0; x < _partsLength.x; x++)
                    _partsPool.Return(_parts[x, y]);

            _puzzleChoices.Open(scenarioStage);
        }

        public void StartGameplay(int puzzleValueX, float chanceGetDetail)
        {
            _chanceGetDetail = chanceGetDetail;
            _parts = _partsGenerator.GetPuzzleParts(_scenarioStage.Image.texture, puzzleValueX);
            _partsLength = new(_parts.GetUpperBound(0) + 1, _parts.GetUpperBound(1) + 1);
            _board.ResetProgress();
            _board.SetParts(_parts, _partsLength);
            _mixer.MixParts(_parts, _partsLength);
            _board.SetPartsPosition();
            _board.OnAddProgress += UpdateProgressView;
            _background.SetMask(_partsLength * _partsGenerator.PartSize);
            _progressPanel.SetPuzzle(_board.Progress, _board.NecessaryProgress);
            _progressPanel.Show(true);
        }

        private void UpdateProgressView()
        {
            _progressPanel.ChangeValue(_board.Progress);
        }

        private void End()
        {
            gameObject.SetActive(false);
            _board.OnAddProgress -= UpdateProgressView;
            _background.SetActiveMask(false);
            float random = Random.Range(0, 1f);
            if (random < _chanceGetDetail)
            {
                _progressPanel.Hide(true);
                _extraLevelUnlocker.Show(OnEnd);
            }
            else
                OnEnd?.Invoke();
        }

        private void OnDestroy()
        {
            if(gameObject.activeSelf)
                _board.OnAddProgress -= UpdateProgressView;
            _board.OnComplete -= End;
        }
    }
}