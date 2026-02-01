using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyGame.Bundles;

namespace MyGame.Menu
{
    public sealed class UpInfoPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private TextMeshProUGUI _textStep;
        [SerializeField] private MenuButton _buttonPrevious;
        [SerializeField] private MenuButton _buttonNext;

        private Image _contentImage;
        private int _levelsCount;
        private int _step;

        public void Init(Image contentImage, int levelsCount)
        {
            _contentImage = contentImage;
            _levelsCount = levelsCount;
            _buttonPrevious.Init(() => ChangeStep(-1));
            _buttonNext.Init(() => ChangeStep(1));
            _textStep.gameObject.SetActive(false);
            GameData.CurrentStep = _step;
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void OnStartLoad()
        {
            _textStep.gameObject.SetActive(false);
            _buttonPrevious.Hide();
            _buttonNext.Hide();
        }

        public void OnEndLoad()
        {
            if(GameData.Achievements.IsUnlock(GameData.CurrentLevel, 0))
            {
                _textStep.gameObject.SetActive(true);
                _buttonPrevious.Show();
                _buttonNext.Show();
            }
        }

        public void UpdateTextName(int currentLevel, string mainInfo)
        {
            _textName.text = $"{currentLevel}/{_levelsCount} {mainInfo}";
            _step = 0;
            GameData.CurrentStep = _step;
            UpdateTextStep();
        }

        private void ChangeStep(int value)
        {
            _step += value;
            if (_step < 0) _step = 4;
            else if (_step > 4) _step = 0;
            GameData.CurrentStep = _step;
            _contentImage.sprite = BundlesController.Instance.MainResourcesBundle.Sprites[_step];
            UpdateTextStep();
        }

        private void UpdateTextStep()
        {
            if (_step == 0)
            {
                string currentLang = I2.Loc.LocalizationManager.CurrentLanguage;
                _textStep.text = currentLang switch
                {
                    "Russian" => "сюжет",
                    "English" =>  "story",
                    "German" => "Geschichte",
                    "Chinese" => "故事",
                    "French" => "histoire",
                    "Hindi" => "कहानी",
                    "Italian" => "storia",
                    "Japanese" => "ストーリー",
                    "Portuguese" => "história",
                    "Spanish" => "historia",
                    _ => "story"
                };
            }
            else
            {
                string currentLang = I2.Loc.LocalizationManager.CurrentLanguage;
                _textStep.text = currentLang switch
                {
                    "Russian" => $"этап {_step}",
                    "English" => $"stage {_step}",
                    "German" => $"Etappe {_step}",
                    "Chinese" => $"舞台 {_step}",
                    "French" => $"phase {_step}",
                    "Hindi" => $"स्टेज {_step}",
                    "Italian" => $"fase {_step}",
                    "Japanese" => $"ステージ {_step}",
                    "Portuguese" => $"fase {_step}",
                    "Spanish" => $"etapa {_step}",
                    _ => $"stage {_step}"
                };
            }

        }
    }
}