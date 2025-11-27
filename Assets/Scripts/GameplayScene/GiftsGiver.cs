using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
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

        private GiftsSettings _giftsSettings;
        private GiftsPool _giftsPool;
        private GiftRespect[] _giftRespects;
        private Gift[] _gifts;

        public UnityAction OnEnd { get; set; }

        public void Init(GiftController giftController)
        {
            _giftsSettings = giftController.GiftsSettings;
            _giftsPool = giftController.GiftsPool;
            CreateGiftRespects();
            _continueButton.onClick.AddListener(End);
            ShopController.Instance.OnBuy += UpdateGiftsValue;
        }

        public void Play(ScenarioStage scenarioStage)
        {
            gameObject.SetActive(true);
            _gifts = new Gift[_giftsSettings.GiftsGroups.Length];
            for (int i = 0; i < _gifts.Length; i++)
            {
                int randomGiftId = _giftsSettings.GiftsGroups[i].GetRandomGiftId;
                _gifts[i] = _giftsPool.GetGift();
                _gifts[i].Init(_giftsSettings, i, randomGiftId);
                _gifts[i].SetParent(_giftsParent);
                _gifts[i].ShowValue(0);
                _gifts[i].SetClickLogic(TryGiveGift);

                _giftRespects[i].gameObject.SetActive(true);
                _giftRespects[i].transform.SetParent(_gifts[i].transform);
                _giftRespects[i].transform.localPosition = new Vector3(0, -175, 0);
                _giftRespects[i].SetRespect(_giftsSettings.GiftsGroups[i].respect);
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
                End();
            }
            else
            {
                ShopController.Instance.Open();
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

        private void OnDestroy()
        {
            ShopController.Instance.OnBuy -= UpdateGiftsValue;
        }
    }
}