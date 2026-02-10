using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using MyGame.Gifts;

namespace MyGame.Shop
{
    public sealed class ShopController : MonoBehaviour
    {
        [SerializeField] private GiftsSettings _giftsSettings;
        [SerializeField] private Image _background;
        [SerializeField] private Transform _window;
        [SerializeField] private ShopProduct[] _products;

        private Button _closeButton;
        private Sequence _seq;
        private readonly float _timeAnim = 0.5f;

        public UnityAction OnBuy { get; set; }

        public void Init()
        {
            gameObject.SetActive(false);
            _closeButton = _background.GetComponent<Button>();
            for (int i = 0; i < _products.Length; i++)
            {
                _products[i].Init(_giftsSettings, Close);
                _products[i].OnBuy += Buy;
            }
        }

        public void Open()
        {
            gameObject.SetActive(true);
            _closeButton.onClick.AddListener(Close);
            _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, 0);
            _window.localPosition = new Vector3(0, -1800, 0);
            TryStopAnim();
            _seq = DOTween.Sequence();
            _seq.Insert(0, _background.DOFade(0.9f, _timeAnim));
            _seq.Insert(0, _window.DOLocalMoveY(0, _timeAnim).SetEase(Ease.OutBack));
        }

        private void Close()
        {
            _closeButton.onClick.RemoveAllListeners();
            TryStopAnim();
            _seq = DOTween.Sequence();
            _seq.Insert(0, _background.DOFade(0, _timeAnim));
            _seq.Insert(0, _window.DOLocalMoveY(-1800, _timeAnim).SetEase(Ease.InBack));
            _seq.InsertCallback(_timeAnim, () => gameObject.SetActive(false));
        }

        private void Buy()
        {
            OnBuy?.Invoke();
        }

        private void TryStopAnim()
        {
            if (_seq != null)
                _seq.Kill();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _products.Length; i++)
                _products[i].OnBuy -= Buy;
            TryStopAnim();
        }
    }
}