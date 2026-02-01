using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;

namespace MyGame.Gameplay.Puzzle
{
    public sealed class PuzzlePart : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _collider;
        private PuzzleBoard _board;
        private Camera _camera;
        private Vector3 _clickPosition;
        private Vector2Int _id;
        private Vector2Int _idPosition;
        private float _size;
        private bool _isMove;
        private ParticleSystem _correctPositionParticles;

        public bool IsTruePosition { get; set; }
        public Vector2Int IdPosition => _idPosition;

        public void Initialize(PuzzleBoard board)
        {
            _camera = Camera.main;
            _board = board;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
            _correctPositionParticles = GetComponentInChildren<ParticleSystem>();

        }

        public void SetParams(Sprite sprite, Vector2Int id, float size)
        {
            _spriteRenderer.sprite = sprite;
            _id = id;
            _collider.size = Vector2.one * size / 100f;
            _size = size;
            IsTruePosition = false;
        }

        public void UpdatePosition(Vector2Int idPosition)
        {
            _idPosition = idPosition;
            transform.localPosition = new Vector3((idPosition.x + 0.5f) * _size, (idPosition.y + 0.5f) * _size, 0) / 100f;
            if (_id == _idPosition)
            {
                IsTruePosition = true;
                _board.AddProgress();
                SoundManager.Instance.PlayRandomGemido();
                PlayParticles();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (IsTruePosition)
                return;
            _isMove = true;
            _spriteRenderer.sortingOrder = 2;
            _clickPosition = transform.position - _camera.ScreenToWorldPoint(Input.mousePosition);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (IsTruePosition)
                return;
            _isMove = false;
            _spriteRenderer.sortingOrder = 1;
            Vector2Int newPosition = _board.TryChangePosition(this);
            UpdatePosition(newPosition);
        }

        private void Update()
        {
            if (!_isMove) return;

            transform.position = _camera.ScreenToWorldPoint(Input.mousePosition) + _clickPosition;
        }

        private void PlayParticles()
        {
            if (_correctPositionParticles != null) _correctPositionParticles.Play();
        }
    }
}
