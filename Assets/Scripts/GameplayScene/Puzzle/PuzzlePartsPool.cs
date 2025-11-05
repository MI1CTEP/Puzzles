using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Gameplay.Puzzle
{
    public sealed class PuzzlePartsPool
    {
        private PuzzlePart _partPrefab;
        private PuzzleBoard _board;
        private Stack<PuzzlePart> _parts = new();

        public PuzzlePartsPool(PuzzlePart partPrefab, PuzzleBoard board)
        {
            _partPrefab = partPrefab;
            _board = board;
        }

        public PuzzlePart Get()
        {
            PuzzlePart puzzlePart;

            if(_parts.Count == 0)
            {
                puzzlePart = GameObject.Instantiate(_partPrefab);
                puzzlePart.Initialize(_board);
            }
            else
            {
                puzzlePart = _parts.Pop();
                puzzlePart.gameObject.SetActive(true);
            }
            return puzzlePart;
        }

        public void Return(PuzzlePart puzzlePart)
        {
            puzzlePart.gameObject.SetActive(false);
            _parts.Push(puzzlePart);
        }
    }
}