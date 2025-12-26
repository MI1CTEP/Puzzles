using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

namespace MyGame.Gameplay.ExtraLevel
{
    public sealed class ExtraLevelDetail : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private Image _outline;
        [SerializeField] private Transform _inside;

        private RectTransform _rectTransform;
        private UnityAction _onClick;
        private Button _button;
        private Sprite _startBackgroundSprite;
        private Sequence _seq;
        private Vector2 _startSize;

        public void Init(UnityAction onClick)
        {
            _onClick = onClick;
            _rectTransform = GetComponent<RectTransform>();
            _startSize = _rectTransform.sizeDelta;
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
            gameObject.SetActive(false);
            _startBackgroundSprite = _background.sprite;
        }

        public void Show(Sequence seq, float timeAnim)
        {
            _rectTransform.sizeDelta = _startSize;
            _button.interactable = false;
            gameObject.SetActive(true);
            transform.localScale = Vector3.zero;
            transform.localPosition = Vector3.zero;
            _background.sprite = _startBackgroundSprite;
            _outline.gameObject.SetActive(true);
            _inside.gameObject.SetActive(true);
            _inside.eulerAngles = Vector3.zero;

            seq.Insert(0, transform.DOScale(Vector3.one * 1.2f, timeAnim));
        }

        public void FixateShowingAnim(Sequence seq, float timeAnim)
        {
            _button.interactable = true;
            seq.Append(_inside.DORotate(new Vector3(0, 0, -5), timeAnim / 12));
            seq.Append(_inside.DORotate(new Vector3(0, 0, 5), timeAnim / 6));
            seq.Append(_inside.DORotate(new Vector3(0, 0, 0), timeAnim / 12));
            seq.Append(transform.DOScale(Vector3.one, timeAnim / 3f));
            seq.Append(transform.DOScale(Vector3.one * 1.2f, timeAnim / 3f));
            seq.SetLoops(-1);
        }

        public void Open(Sprite sprite, Vector3 detailPosition, float scale, Sequence seq, float timeAnim)
        {
            seq.Append(transform.DOScaleX(0, timeAnim / 4));
            seq.AppendCallback(() =>
            {
                _background.sprite = sprite;
                _outline.gameObject.SetActive(false);
                _inside.gameObject.SetActive(false);
            });
            seq.Append(transform.DOScaleX(transform.localScale.y, timeAnim / 4));
            seq.Append(transform.DOLocalMove(detailPosition, timeAnim / 2));
            seq.Insert(timeAnim / 2, transform.DOScale(Vector3.one, timeAnim / 2));
            seq.Insert(timeAnim / 2, _rectTransform.DOSizeDelta(Vector2.one * scale, timeAnim / 2));
        }

        private void OnClick()
        {
            _button.interactable = false;
            _onClick?.Invoke();
        }
    }
}