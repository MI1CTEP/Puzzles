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
        private float _startAnchorY;
        private int _value;

        public void Init()
        {
            _startAnchorY = _rectTransform.anchoredPosition.y;
            _rectTransform.anchoredPosition = new Vector2(0, -_startAnchorY);
        }

        public void Add(int value)
        {
            _value = GameData.Score.Load();
            GameData.Score.Add(value);
            _respectValueText.text = _value.ToString();
            _respectAddedValueText.text = $"+{value}";
            TryStopAnim();
            _seq = DOTween.Sequence();
            _seq.Insert(0, _rectTransform.DOAnchorPosY(_startAnchorY, _timeAnim / 8));
            _seq.Insert(_timeAnim / 4, _respectValueText.transform.DOScale(Vector3.one * 1.25f, _timeAnim / 8));
            _seq.InsertCallback(_timeAnim * 3 / 8, () => _respectValueText.text = (_value + value).ToString());
            _seq.Insert(_timeAnim / 2, _respectValueText.transform.DOScale(Vector3.one, _timeAnim / 8));
            _seq.Insert(_timeAnim * 7 / 8, _rectTransform.DOAnchorPosY(-_startAnchorY, _timeAnim / 8));
        }

        public void ShowEnd(Transform parent, float endPositionY, float canvasSizeY, int addedRespect, float timeAnim, ref float waitTimeAnim)
        {
            TryStopAnim();

            _respectValueText.text = GameData.Score.Load().ToString();
            _respectAddedValueText.text = $"+{addedRespect}";

            transform.SetParent(parent);
            _rectTransform.anchoredPosition = new Vector2(0, -canvasSizeY + _startAnchorY);

            _seq = DOTween.Sequence();
            _seq.Insert(waitTimeAnim, _rectTransform.DOLocalMoveY(0, timeAnim / 2).SetEase(Ease.OutExpo));
            _seq.Insert(waitTimeAnim + timeAnim / 4, _respectValueText.transform.DOScale(Vector3.one * 1.25f, timeAnim / 8));
            _seq.InsertCallback(waitTimeAnim + timeAnim / 2, () => _respectValueText.text = GameData.Score.Load().ToString());
            _seq.Insert(waitTimeAnim + timeAnim / 2, _respectValueText.transform.DOScale(Vector3.one, timeAnim / 8));
            _seq.Insert(waitTimeAnim + timeAnim / 2, _rectTransform.DOAnchorPosY(endPositionY + _startAnchorY, timeAnim / 2).SetEase(Ease.InExpo));
            waitTimeAnim += timeAnim;
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