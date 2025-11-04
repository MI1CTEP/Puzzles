using UnityEngine;

namespace MyGame
{
    public sealed class PuzzlePartsGenerator
    {
        private PuzzlePart _partPrefab;
        private Transform _gameParent;
        private Texture2D _sourceTexture;
        private PuzzlePart[,] _parts;
        private Vector2Int _soursceTextureSize;
        private Vector2Int _partsValue;
        private Vector2Int _offset;
        private int _partsSize;

        public int PartSize => _partsSize;

        public PuzzlePart[,] GetPuzzleParts(PuzzleController controller, PuzzlePart partPrefab, Transform gameParent, Texture2D sourceTexture, int partsValueX)
        {
            _partPrefab = partPrefab;
            _gameParent = gameParent;
            _sourceTexture = sourceTexture;

            _soursceTextureSize = new(sourceTexture.width, sourceTexture.height);

            _partsSize = _soursceTextureSize.x / (partsValueX + 1);

            _partsValue.x = partsValueX;
            _partsValue.y = _soursceTextureSize.y / _partsSize - 1;

            _offset.x = (_soursceTextureSize.x - _partsValue.x * _partsSize) / 2;
            _offset.y = (_soursceTextureSize.y - _partsValue.y * _partsSize) / 2;

            CreateParts(controller);

            return _parts;
        }

        private void CreateParts(PuzzleController controller)
        {
            _parts = new PuzzlePart[_partsValue.x, _partsValue.y];

            for (int y = 0; y < _partsValue.y; y++)
            {
                for (int x = 0; x < _partsValue.x; x++)
                {
                    _parts[x, y] = GameObject.Instantiate(_partPrefab, _gameParent);
                    _parts[x, y].Init(controller, new Vector2Int(x, y), _offset, GetPartSprite(x, y), _partsSize);
                }
            }
        }

        private Sprite GetPartSprite(int x, int y)
        {
            Rect rect = new Rect(x * _partsSize + _offset.x, y * _partsSize + _offset.y, _partsSize, _partsSize);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            return Sprite.Create(_sourceTexture, rect, pivot);
        }
    }
}