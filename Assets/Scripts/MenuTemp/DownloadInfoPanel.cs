using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyGame.Bundles;

namespace MyGame.MenuTemp
{
    public sealed class DownloadInfoPanel : MonoBehaviour
    {
        [SerializeField] private Image _progressImage;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private TextMeshProUGUI _infoText1;
        [SerializeField] private TextMeshProUGUI _infoText2;

        private BundlesController _bundlesController;

        public void Init()
        {
            _bundlesController = BundlesController.Instance;
            _bundlesController.OnChangeAllProgress += UpdateProgeress;
            _bundlesController.OnEndDownloadLevel += UpdateInfo1;
            _bundlesController.OnStartDownloadLevel += UpdateInfo2;
            _bundlesController.OnEndDownload += Disable;
            UpdateInfo1(_bundlesController.LastDownloadedLevel);
            UpdateInfo2(_bundlesController.DownloadingLevel);
        }

        private void UpdateProgeress(float value)
        {
            _progressImage.fillAmount = value;
            _progressText.text = (value * 100).ToString("0") + "%";
        }

        private void UpdateInfo1(int levelId)
        {
            _infoText1.text = $"Загружено уровней {_bundlesController.DownloadedCount}/{_bundlesController.NotDownloadedCount}";
        }

        private void UpdateInfo2(int levelId)
        {
            _infoText2.text = $"Загружается уровень {levelId + 1}";
        }

        private void Disable()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _bundlesController.OnChangeAllProgress -= UpdateProgeress;
            _bundlesController.OnEndDownloadLevel -= UpdateInfo1;
            _bundlesController.OnStartDownloadLevel -= UpdateInfo2;
            _bundlesController.OnEndDownload -= Disable;
        }
    }
}