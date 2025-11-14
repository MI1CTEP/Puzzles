using UnityEngine;
using DG.Tweening;

namespace MyGame.Gameplay
{
    public sealed class BackgroundController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _frontSprite;
        [SerializeField] private SpriteRenderer _backSprite;

        private Sprite _newSprite;
        private Sequence _seq;
        private Color _clearColor = new(1, 1, 1, 0);
        private float _timeAnim = 0.3f;

        public void SetSprite(Sprite sprite, bool isAnim)
        {
            if (isAnim)
            {
                _frontSprite.sprite = _backSprite.sprite;
                _frontSprite.color = Color.white;

                _backSprite.sprite = sprite;

                _seq.Kill();
                _seq = DOTween.Sequence();
                _seq.Insert(0, _frontSprite.DOFade(0, _timeAnim));
            }
            else
            {
                _backSprite.sprite = sprite;
                _frontSprite.color = _clearColor;
            }
        }

        private void OnDestroy()
        {
            _seq.Kill();
        }
    }
}