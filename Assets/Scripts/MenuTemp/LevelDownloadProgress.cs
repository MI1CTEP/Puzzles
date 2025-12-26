using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyGame.Bundles;

namespace MyGame.MenuTemp
{
    public sealed class LevelDownloadProgress : MonoBehaviour
    {
        [SerializeField] private Image _progressImage;
        [SerializeField] private TextMeshProUGUI _progressText;

        public void SetActive(bool value)
        {
            if (gameObject.activeSelf == value)
                return;

            gameObject.SetActive(value);
            if (value)
                BundlesController.Instance.OnChangeLevelProgress += UpdateProgress;
            else
                BundlesController.Instance.OnChangeLevelProgress -= UpdateProgress;
        }

        private void UpdateProgress(float value)
        {
            _progressImage.fillAmount = value;
            _progressText.text = (value * 100).ToString("0") + "%";
        }
    }
}