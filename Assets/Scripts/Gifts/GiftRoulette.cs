using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

namespace MyGame.Gifts
{
    public sealed class GiftRoulette : MonoBehaviour
    {
        [SerializeField] private Transform _parent;
        [SerializeField] private Transform _respawnPoint;
        [SerializeField] private Transform _arrows;

        private GiftRouletteBackground _background;
        private GiftsSettings _giftsSettings;
        private GiftsPool _giftsPool;
        private Gift[] _gifts;
        private Sequence _seq;
        private readonly float _sizeGift = 256f;
        private readonly float _offsetGifts = 10f;
        private readonly float _timeRouletreAnim = 10f;
        private readonly float _timeShowWinnerAnim = 0.5f;
        private readonly int _startGiftsOnLeft = 2;
        private int _respawnedGifts;
        private int _positionWinner;
        private bool _isAnim;

        public void Init(GiftsSettings giftsSettings, GiftsPool giftsPool)
        {
            _background = GetComponent<GiftRouletteBackground>();
            _giftsSettings = giftsSettings;
            _giftsPool = giftsPool;
            gameObject.SetActive(false);
        }

        public void Show(UnityAction onEndShowing)
        {
            gameObject.SetActive(true);
            _background.PlayFadeAnim();
            CreateGiftArray();
            SetGiftsInArray();
            MixGifts();
            for (int i = 0; i < _gifts.Length; i++)
                SetPositionGift(i, i);
            PlayRouletteAnim();
        }

        private void CreateGiftArray()
        {
            int length = 0;
            for (int i = 0; i < _giftsSettings.GiftsGroups.Length; i++)
                length += _giftsSettings.GiftsGroups[i].quantityInRoulette;
            _gifts = new Gift[length];
        }

        private void SetGiftsInArray()
        {
            int giftId = 0;
            for (int i = 0; i < _giftsSettings.GiftsGroups.Length; i++)
            {
                for (int j = 0; j < _giftsSettings.GiftsGroups[i].quantityInRoulette; j++, giftId++)
                {
                    _gifts[giftId] = _giftsPool.GetGift();
                    _gifts[giftId].Init(_giftsSettings, i, _giftsSettings.GiftsGroups[i].GetRandomGiftId);
                    _gifts[giftId].SetParent(_parent);
                }
            }
        }

        private void MixGifts()
        {
            for (int i = 0; i < _gifts.Length; i++)
            {
                int random = Random.Range(0, _gifts.Length);
                (_gifts[i], _gifts[random]) = (_gifts[random], _gifts[i]);
            }
        }

        private void SetPositionGift(int idGift, int idPosition)
        {
            _gifts[idGift].transform.localPosition = new Vector3((idPosition - _startGiftsOnLeft) * (_sizeGift + _offsetGifts), 0, 0);
        }

        private void PlayRouletteAnim()
        {
            _isAnim = true;
            _positionWinner = Random.Range(_gifts.Length * 3, _gifts.Length * 4);
            float offset = Random.Range(-_sizeGift / 2, _sizeGift / 2);

            TryStopAnim();
            _seq = DOTween.Sequence();
            _seq.Insert(0, _parent.DOLocalMoveX(-_positionWinner * (_sizeGift + _offsetGifts) + offset, _timeRouletreAnim).SetEase(Ease.InOutCubic));
            _seq.InsertCallback(_timeRouletreAnim, PlayReceivingGiftAnim);
        }

        private void PlayReceivingGiftAnim()
        {
            _isAnim = false;
            _arrows.SetParent(_parent);
            TryStopAnim();
            _seq = DOTween.Sequence();
            Gift winnerGift = _gifts[(_positionWinner + _startGiftsOnLeft) % _gifts.Length];
            GameData.Gifts.AddValue(winnerGift.GroupId, winnerGift.Id, 1);
            winnerGift.transform.SetParent(transform);
            _seq.Insert(0, _parent.DOLocalMoveY(350, _timeShowWinnerAnim));
            _seq.Insert(0, winnerGift.transform.DOScale(Vector3.one * 1.2f, _timeShowWinnerAnim));
            _seq.Insert(0, winnerGift.transform.DOLocalMove(Vector3.zero, _timeShowWinnerAnim));
        }

        private void Update()
        {
            if (!_isAnim)
                return;

            if(_gifts[_respawnedGifts % _gifts.Length].transform.position.x < _respawnPoint.transform.position.x)
            {
                SetPositionGift(_respawnedGifts % _gifts.Length, _respawnedGifts + _gifts.Length);
                _respawnedGifts++;
            }
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