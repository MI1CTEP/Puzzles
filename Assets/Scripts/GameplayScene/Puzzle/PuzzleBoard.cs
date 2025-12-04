using UnityEngine;
using UnityEngine.Events;
using UniRx;

namespace MyGame.Gameplay.Puzzle
{
    public sealed class PuzzleBoard
    {
        private PuzzlePart[,] _parts;
        private Vector2Int _partsLength;
        private int _necessaryProgress;
        private int _progress;

        public UnityAction OnAddProgress { get; set; }
        public UnityAction OnComplete { get; set; }
        public int NecessaryProgress => _necessaryProgress;
        public int Progress => _progress;

        public void SetParts(PuzzlePart[,] parts, Vector2Int partsLength)
        {
            _parts = parts;
            _partsLength = partsLength;
            _necessaryProgress = partsLength.x * partsLength.y;
        }

        public void AddProgress()
        {
            _progress++;
            OnAddProgress?.Invoke();
            if (_progress >= _necessaryProgress)
                OnComplete?.Invoke();
        }

        public void ResetProgress()
        {
            _progress = 0;
        }

        public Vector2Int TryChangePosition(PuzzlePart part)
        {
            Vector2Int tempId = Vector2Int.zero;
            if (part.IdPosition.x == 0)
                tempId.x = 1;
            if (part.IdPosition.y == 0)
                tempId.y = 1;
            Vector2Int partId = Vector2Int.zero;
            for (int x = 0; x < _partsLength.x; x++)
            {
                if (Mathf.Abs(part.transform.localPosition.x - _parts[x, tempId.y].transform.localPosition.x) < 0.5f)
                {
                    partId.x = x;
                    break;
                }
                if (x == _partsLength.x - 1)
                    return part.IdPosition;
            }

            for (int y = 0; y < _partsLength.y; y++)
            {
                if (Mathf.Abs(part.transform.localPosition.y - _parts[tempId.x, y].transform.localPosition.y) < 0.5f)
                {
                    partId.y = y;
                    break;
                }
                if (y == _partsLength.y - 1)
                    return part.IdPosition;
            }

            if (_parts[partId.x, partId.y].IsTruePosition)
                return part.IdPosition;

            if (partId != part.IdPosition)
            {
                _parts[partId.x, partId.y].UpdatePosition(part.IdPosition);
                (_parts[partId.x, partId.y], _parts[part.IdPosition.x, part.IdPosition.y]) = (_parts[part.IdPosition.x, part.IdPosition.y], _parts[partId.x, partId.y]);
            }

            return partId;
        }

        public void SetPartsPosition()
        {
            for (int y = 0; y < _partsLength.y; y++)
                for (int x = 0; x < _partsLength.x; x++)
                    _parts[x, y].UpdatePosition(new Vector2Int(x, y));
        }
    }
}