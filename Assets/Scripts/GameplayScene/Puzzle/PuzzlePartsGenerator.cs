using UnityEngine;

namespace MyGame.Gameplay.Puzzle
{
    public sealed class PuzzlePartsGenerator
    {
        private PuzzlePartsPool _partsPool;
        private Transform _parent;
        private Vector2Int _soursceTextureSize;
        private Vector2Int _partsValue;
        private Vector2Int _offset;
        private int _partsSize;

        public int PartSize => _partsSize;

        public PuzzlePartsGenerator(PuzzlePartsPool puzzlePartsPool, Transform puzzleParent)
        {
            _partsPool = puzzlePartsPool;
            _parent = new GameObject("PuzzleParts").transform;
            _parent.SetParent(puzzleParent);
        }

        public PuzzlePart[,] GetPuzzleParts(Texture2D sourceTexture, int partsValueX)
        {
            _soursceTextureSize = new(sourceTexture.width, sourceTexture.height);

            _partsSize = _soursceTextureSize.x / (partsValueX + 1);

            _partsValue.x = partsValueX;
            _partsValue.y = _soursceTextureSize.y / _partsSize - 1;

            _offset.x = (_soursceTextureSize.x - _partsValue.x * _partsSize) / 2;
            _offset.y = (_soursceTextureSize.y - _partsValue.y * _partsSize) / 2;

            _parent.position = new Vector3(_partsValue.x, _partsValue.y, 0) * _partsSize / -200f;

            return CreateParts(sourceTexture);
        }

        private PuzzlePart[,] CreateParts(Texture2D sourceTexture)
        {
            PuzzlePart[,] parts = new PuzzlePart[_partsValue.x, _partsValue.y];

            for (int y = 0; y < _partsValue.y; y++)
            {
                for (int x = 0; x < _partsValue.x; x++)
                {
                    parts[x, y] = _partsPool.Get();
                    parts[x, y].transform.SetParent(_parent);
                    parts[x, y].SetParams(GetPartSprite(sourceTexture, x, y), new Vector2Int(x, y), _partsSize);
                }
            }
            return parts;
        }

        private Sprite GetPartSprite(Texture2D sourceTexture, int x, int y)
        {
            Rect rect = new Rect(x * _partsSize + _offset.x, y * _partsSize + _offset.y, _partsSize, _partsSize);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            return Sprite.Create(sourceTexture, rect, pivot);
        }
    }
}