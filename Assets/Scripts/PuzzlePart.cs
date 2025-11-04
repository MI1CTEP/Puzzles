using UnityEngine;
using UnityEngine.EventSystems;

namespace MyGame
{
    public sealed class PuzzlePart : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private BoxCollider2D _collider;

        private PuzzleController _controller;
        private Camera _camera;
        private Vector3 _startPosition;
        private Vector3 _clickPosition;
        private Vector2Int _id;
        private Vector2Int _idPosition;
        private Vector2Int _offset;
        private float _size;
        private bool _isMove;

        public bool IsTruePosition { get; set; }
        public Vector2Int IdPosition => _idPosition;


        public void Init(PuzzleController controller, Vector2Int id, Vector2Int offset, Sprite sprite, float size)
        {
            _controller = controller;
            _camera = Camera.main;
            _id = id;
            _offset = offset;
            _spriteRenderer.sprite = sprite;
            _collider.size = Vector2.one * size / 100f;
            _size = size;
        }

        public void UpdatePosition(Vector2Int idPosition)
        {
            _idPosition = idPosition;
            transform.localPosition = new Vector3(_offset.x + (idPosition.x + 0.5f) * _size, _offset.y + (idPosition.y + 0.5f) * _size, 0) / 100f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isMove = true;
            _spriteRenderer.sortingOrder = 1;
            _startPosition = transform.position;
            _clickPosition = _startPosition - _camera.ScreenToWorldPoint(Input.mousePosition);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isMove = false;
            _spriteRenderer.sortingOrder = 0;
            Vector2Int newPosition = _controller.TrySet(this);
            UpdatePosition(newPosition);
        }

        private void Update()
        {
            if (!_isMove) return;

            transform.position = _camera.ScreenToWorldPoint(Input.mousePosition) + _clickPosition;
        }
    }
}
