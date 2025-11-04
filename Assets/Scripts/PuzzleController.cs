using UnityEngine;

namespace MyGame
{
    public sealed class PuzzleController : MonoBehaviour
    {
        [SerializeField] private PuzzlePart _partPrefab;
        [SerializeField] private Texture2D _sourceTexture;
        [SerializeField] private int _partsValueX;
        [SerializeField] private int _openLinesY;

        private PuzzlePartsGenerator _partsGenerator;
        private PuzzlePart[,] _parts;
        private PuzzleFrame _puzzleFrame;
        private Vector2Int _partsLength;

        private void Start()
        {
            transform.position = new Vector3(-_sourceTexture.width / 2, -_sourceTexture.height / 2, 0) / 100f;

            _partsGenerator = new();
            _parts = _partsGenerator.GetPuzzleParts(this, _partPrefab, transform, _sourceTexture, _partsValueX);

            _partsLength = new(_parts.GetUpperBound(0) + 1, _parts.GetUpperBound(1) + 1);
            PuzzleMixer.MixParts(_parts, _partsLength, _openLinesY);

            SetPartsPosition();

            _puzzleFrame = new();
            _puzzleFrame.Create(transform, _sourceTexture, _partsLength * _partsGenerator.PartSize);

            UpdateCameraSize();
        }

        public Vector2Int TrySet(PuzzlePart part)
        {
            Vector2Int tempId = Vector2Int.zero;
            if (part.IdPosition.x == 0)
                tempId.x = 1;
            if (part.IdPosition.y == 0)
                tempId.y = 1;
            Vector2Int partId = Vector2Int.zero;
            for (int x = 0; x < _partsLength.x; x++)
            {
                if(Mathf.Abs(part.transform.localPosition.x - _parts[x, tempId.y].transform.localPosition.x) < 0.5f)
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

            if (partId != part.IdPosition)
            {
                _parts[partId.x, partId.y].UpdatePosition(part.IdPosition);
                (_parts[partId.x, partId.y], _parts[part.IdPosition.x, part.IdPosition.y]) = (_parts[part.IdPosition.x, part.IdPosition.y], _parts[partId.x, partId.y]);
            }

            return partId;
        }

        private void SetPartsPosition()
        {
            for (int y = 0; y < _partsLength.y; y++)
                for (int x = 0; x < _partsLength.x; x++)
                    _parts[x, y].UpdatePosition(new Vector2Int(x, y));
        }

        private void UpdateCameraSize()
        {
            Camera camera = Camera.main;
            float aspectRatio = (float)Screen.height / Screen.width;
            camera.orthographicSize = aspectRatio * _sourceTexture.width / 200;
        }
    }
}