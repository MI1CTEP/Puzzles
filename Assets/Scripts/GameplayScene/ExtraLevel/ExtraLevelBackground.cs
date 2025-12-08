using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MyGame.Gameplay.ExtraLevel
{
    public class ExtraLevelBackground : MonoBehaviour
    {
        [SerializeField] private RectTransform _extraLevelUnlockerMask;
        [SerializeField] private Transform _masksParent;

        private Image _image;
        private CanvasGroup _canvasGroup;
        private RectTransform[] _masks;

        public void Init(Sprite sprite, int masksLength)
        {
            _image = GetComponent<Image>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _image.sprite = sprite;
            _masks = new RectTransform[masksLength];
            Hide();
        }

        public void Hide()
        {
            _image.color = Color.clear;
            _canvasGroup.alpha = 0;
        }

        public void CreateMask(Vector2 position, int id)
        {
            RectTransform mask = Instantiate(_extraLevelUnlockerMask, _masksParent);
            mask.anchoredPosition = position;
            _masks[id] = mask;
        }

        public void Show(Sequence seq, float timeAnim)
        {
            Hide();
            seq.Insert(0, _canvasGroup.DOFade(1, timeAnim));
            seq.Insert(timeAnim / 4, _image.DOColor(Color.white, timeAnim * 3 / 4));
        }

        public void DestroyMask(int id) => Destroy(_masks[id].gameObject);

        public Vector3 GetMaskPosition(int id) => _masks[id].localPosition;
    }
}