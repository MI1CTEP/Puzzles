using UnityEngine;
using DG.Tweening;

namespace MyGame.Gameplay.Dialogue
{
    public sealed class DialogueAnim
    {
        private RectTransform _messageHistory;
        private Sequence _seq;
        private float _timeAnim = 0.3f;

        public DialogueAnim(RectTransform messageHistory)
        {
            _messageHistory = messageHistory;
        }

        public void MoveMessageHistory(float anchorPositionY)
        {
            TryStopAnim();
            _seq = DOTween.Sequence();
            _seq.Append(_messageHistory.DOAnchorPosY(anchorPositionY, _timeAnim));
        }

        public void TryStopAnim()
        {
            if (_seq != null)
                _seq.Kill();
        }
    }
}