using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using MyGame.Gifts;
using MyGame.Shop;

namespace MyGame.Gameplay
{
    public sealed class GiftsGiver : MonoBehaviour, IGameStage
    {
        [SerializeField] private Transform _giftsParent;
        [SerializeField] private GiftRespect _giftRespectPrefab;
        [SerializeField] private Button _continueButton;
        [SerializeField] private RespectController _respectController;
        [SerializeField] private Image _background;

        private Sequence _seq;
        private GiftsSettings _giftsSettings;
        private GiftsPool _giftsPool;
        private GiftRespect[] _giftRespects;
        private Gift[] _gifts;
        private ShopController _shopController;
        private readonly float _timeShowGiftsAnim = 0.8f;

        public UnityAction OnEnd { get; set; }
        public int GivedId { get; set; } = -1;

        public void Init(GiftController giftController, ShopController shopController)
        {
            gameObject.SetActive(false);
            _respectController.Init();
            _giftsSettings = giftController.GiftsSettings;
            _giftsPool = giftController.GiftsPool;
            CreateGiftRespects();
            _continueButton.onClick.AddListener(End);
            _shopController = shopController;
            _shopController.OnBuy += UpdateGiftsValue;
        }

        public void Play(ScenarioStage scenarioStage)
        {
            _background.color = Color.clear;
            _continueButton.gameObject.SetActive(false);
            gameObject.SetActive(true);
            _gifts = new Gift[_giftsSettings.GiftsGroups.Length];
            for (int i = 0; i < _gifts.Length; i++)
            {
                int randomGiftId = _giftsSettings.GiftsGroups[i].GetRandomGiftId;
                _gifts[i] = _giftsPool.GetGift();
                _gifts[i].Init(_giftsSettings, i, randomGiftId);
                _gifts[i].SetParent(_giftsParent);
                _gifts[i].ShowValue(0);
                _gifts[i].transform.localPosition = new Vector3(-405 + i * 270,  -600, 0);
                _gifts[i].SetClickLogic(TryGiveGift);

                _giftRespects[i].gameObject.SetActive(false);
                _giftRespects[i].transform.SetParent(_gifts[i].transform);
                _giftRespects[i].transform.localPosition = new Vector3(0, -175, 0);
                _giftRespects[i].SetRespect(_giftsSettings.GiftsGroups[i].respect);
            }
            ShowGiftsAnim();
        }

        private void ShowGiftsAnim()
        {
            TryStopAnim();
            _seq = DOTween.Sequence();
            _seq.Insert(0, _background.DOFade(0.9f, _timeShowGiftsAnim));
            for (int i = 0; i < _gifts.Length; i++)
            {
                int id = i;
                _seq.Insert(id * 0.1f, _gifts[id].transform.DOLocalMoveY(0, _timeShowGiftsAnim - 0.1f * _gifts.Length));
            }
            _seq.InsertCallback(_timeShowGiftsAnim, () => _continueButton.gameObject.SetActive(true));
            for (int i = 0; i < _giftRespects.Length; i++)
            {
                int id = i;
                _seq.InsertCallback(_timeShowGiftsAnim + id * 0.1f, () => _giftRespects[id].gameObject.SetActive(true));
            }
        }

        private void CreateGiftRespects()
        {
            _giftRespects = new GiftRespect[_giftsSettings.GiftsGroups.Length];
            for (int i = 0; i < _giftRespects.Length; i++)
            {
                _giftRespects[i] = Instantiate(_giftRespectPrefab, transform);
                _giftRespects[i].gameObject.SetActive(false);
            }
        }

        private void TryGiveGift(Gift gift)
        {
            if (gift.Value > 0)
            {
                _respectController.Add(_giftsSettings.GiftsGroups[gift.GroupId].respect);
                GameData.Gifts.AddValue(gift.GroupId, gift.Id, -1);
                GivedId = gift.GroupId;
                End();
            }
            else
            {
                _shopController.Open();
            }
        }

        private void UpdateGiftsValue()
        {
            if (_gifts != null)
                for (int i = 0; i < _gifts.Length; i++)
                    _gifts[i].ShowValue(0);
        }

        private void End()
        {
            gameObject.SetActive(false);
            for (int i = 0; i < _giftRespects.Length; i++)
            {
                _giftRespects[i].gameObject.SetActive(false);
                _giftRespects[i].transform.SetParent(transform);
            }
            for (int i = 0; i < _gifts.Length; i++)
                _giftsPool.ReturnGift(_gifts[i]);
            _gifts = null;
            OnEnd?.Invoke();
        }

        private void TryStopAnim()
        {
            if (_seq != null)
                _seq.Kill();
        }

        private void OnDestroy()
        {
            TryStopAnim();
            _shopController.OnBuy -= UpdateGiftsValue;
        }
    }
}