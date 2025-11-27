using UnityEngine;

namespace MyGame.Gifts
{
    [CreateAssetMenu(menuName = "ScriptableObjects/GiftsSettings")]
    public sealed class GiftsSettings : ScriptableObject
    {
        [SerializeField] private GiftsGroup[] _giftsGroups;

        public GiftsGroup[] GiftsGroups => _giftsGroups;

    }

    [System.Serializable]
    public sealed class GiftsGroup
    {
        public Color outlineColor;
        public Sprite[] sprites;
        public int quantityInRoulette;
        public int respect;

        public int GetRandomGiftId => Random.Range(0, sprites.Length);
    }
}