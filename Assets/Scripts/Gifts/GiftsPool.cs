using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Gifts
{
    public sealed class GiftsPool
    {
        private Gift _giftPrefab;
        private Stack<Gift> _gifts;

        public GiftsPool(Gift giftPrefab)
        {
            _giftPrefab = giftPrefab;
            _gifts = new();
        }

        public Gift GetGift()
        {
            Gift gift;
            if(_gifts.Count == 0)
            {
                gift = GameObject.Instantiate(_giftPrefab);
            }
            else
            {
                gift = _gifts.Pop();
            }
            gift.gameObject.SetActive(true);
            return gift;
        }

        public void ReturnGift(Gift gift)
        {
            gift.gameObject.SetActive(false);
            _gifts.Push(gift);
        }
    }
}