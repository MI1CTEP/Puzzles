using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MyGame.Gifts
{
    public sealed class GiftRouletteBackground : MonoBehaviour
    {
        [SerializeField] private Image _mainImage;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Transform _light;

        private Sequence _seq;
        private readonly float _timeFadeAnim = 1f;
        private readonly float _timeLightAnim = 4f;

        public void PlayFadeAnim()
        {
            _seq = DOTween.Sequence();
            _seq.Insert(0, _backgroundImage.DOFade(1, _timeFadeAnim));
            _seq.Insert(0, _mainImage.DOFade(0.9f, _timeFadeAnim * 2));
            _seq.InsertCallback(_timeFadeAnim * 2, PlayLightAnim);
        }

        private void PlayLightAnim()
        {
            _seq.Kill();
            _seq = DOTween.Sequence();
            _light.localPosition = new Vector3(0, -2500, 0);
            _seq.Insert(0, _light.DOLocalMoveY(2500, _timeLightAnim));
            _seq.InsertCallback(_timeLightAnim + 1, PlayLightAnim);
        }
    }
}