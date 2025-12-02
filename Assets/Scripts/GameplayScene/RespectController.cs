using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace MyGame.Gameplay
{
    public sealed class RespectController : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private TextMeshProUGUI _respectValueText;
        [SerializeField] private TextMeshProUGUI _respectAddedValueText;

        private Sequence _seq;
        private readonly float _timeAnim = 3f; 
        private int _value;

        public void Add(int value)
        {
            _value = GameData.Score.Load();
            GameData.Score.Add(value);
            _respectValueText.text = _value.ToString();
            _respectAddedValueText.text = $"+{value}";
            TryStopAnim();
            _seq = DOTween.Sequence();
            _seq.Insert(0, _rectTransform.DOAnchorPosY(-50, _timeAnim / 8));
            _seq.Insert(_timeAnim / 4, _respectValueText.transform.DOScale(Vector3.one * 1.25f, _timeAnim / 8));
            _seq.InsertCallback(_timeAnim * 3 / 8, () => _respectValueText.text = (_value + value).ToString());
            _seq.Insert(_timeAnim / 2, _respectValueText.transform.DOScale(Vector3.one, _timeAnim / 8));
            _seq.Insert(_timeAnim * 7 / 8, _rectTransform.DOAnchorPosY(50, _timeAnim / 8));
        }

        private void TryStopAnim()
        {
            if (_seq != null)
                _seq.Kill();
        }

        private void OnDestroy()
        {
            TryStopAnim();
        }
    }
}