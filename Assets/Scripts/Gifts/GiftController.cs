using UnityEngine;
using UnityEngine.Events;

namespace MyGame.Gifts
{
    public sealed class GiftController : MonoBehaviour
    {
        [SerializeField] private GiftsSettings _giftsSettings;
        [SerializeField] private Gift _giftPrefab;
        [SerializeField] private GiftRoulette _giftRoulette;

        private GiftsPool _giftsPool;

        public GiftsSettings GiftsSettings => _giftsSettings;
        public GiftsPool GiftsPool => _giftsPool;

        public void Init()
        {
            _giftsPool = new(_giftPrefab);
            _giftRoulette?.Init(_giftsSettings, _giftsPool);
        }

        public void ShowRoulette(UnityAction onEndShowing)
        {
            if(_giftRoulette != null)
            {
                _giftRoulette.Show(onEndShowing);
                return;
            }
        }
    }
}