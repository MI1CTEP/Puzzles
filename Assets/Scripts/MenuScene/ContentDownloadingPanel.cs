using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using MyGame.Bundles;

namespace MyGame.Menu
{
    public sealed class ContentDownloadingPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private Image _progressImage;
        [SerializeField] private RectTransform _loadingUp;

        private Sequence _seq;
        private bool _isShowingProgress;

        public void Show(bool isNowDownloading)
        {
            TryShowProgress(isNowDownloading);

            if (gameObject.activeSelf)
                return;
            gameObject.SetActive(true);
            _seq = DOTween.Sequence();
            StartLoadingAnim();
        }

        public void Hide()
        {
            if (!gameObject.activeSelf)
                return;
            TryStopAnim();
            gameObject.SetActive(false);
        }

        private void TryShowProgress(bool isNowDownloading)
        {
            UpdateProgress(0);
            if (isNowDownloading)
            {
                BundlesController.Instance.OnChangeLevelProgress += UpdateProgress;
                _isShowingProgress = true;
            }
            else if (_isShowingProgress)
            {
                BundlesController.Instance.OnChangeLevelProgress -= UpdateProgress;
                _isShowingProgress = false;
            }
        }

        private void UpdateProgress(float value)
        {
            _progressImage.fillAmount = value;
            _progressText.text = (value * 100).ToString("0") + "%";
        }

        private void StartLoadingAnim()
        {
            _seq.Append(_loadingUp.DOAnchorPosY(0, 0.5f));
            _seq.Append(_loadingUp.DOAnchorPosY(15, 0.5f));
            _seq.SetLoops(-1);
        }

        private void TryStopAnim()
        {
            if (_seq != null)
                _seq.Kill();
        }

        private void OnDestroy()
        {
            TryStopAnim();
            if(_isShowingProgress)
                BundlesController.Instance.OnChangeLevelProgress -= UpdateProgress;
        }
    }
}