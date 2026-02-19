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

        [SerializeField] private Image _imageBlur;

        private MenuButton _buttonPlay;
        private Image _contentImage;
        private int _levelsCount;
        private int _step;

        public void Init(MenuButton buttonPlay, Image contentImage, int levelsCount)
        {
            _buttonPlay = buttonPlay;
            _contentImage = contentImage;
            _levelsCount = levelsCount;
            _buttonPrevious.Init(() => ChangeStep(-1));
            _buttonNext.Init(() => ChangeStep(1));
            _textStep.gameObject.SetActive(false);
            GameData.CurrentPuzzleStep = _step;
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
            if (!value)
                _imageBlur.gameObject.SetActive(false);
        }

        public void OnStartLoad()
        {
            _textStep.gameObject.SetActive(false);
            _buttonPrevious.Hide();
            _buttonNext.Hide();
        }

        public void OnEndLoad()
        {
            _textStep.gameObject.SetActive(true);
            _buttonPrevious.Show();
            _buttonNext.Show();
            //Почему ачивка и именно 0? Типа Откроется только прохождении всего уровня?
            //if(GameData.Achievements.IsUnlock(GameData.CurrentLevel, 0))
            //{
            //    _textStep.gameObject.SetActive(true);
            //    _buttonPrevious.Show();
            //    _buttonNext.Show();
            //}
        }

        public void UpdateAll(int currentLevel, string mainInfo)
        {
            _textName.text = $"{currentLevel}/{_levelsCount} {mainInfo}";
            _step = 0;
            ChangeStep(0);
        }

        //Тут что за логика? Маг числа. Почему именно 4? Если будем добавлять контент, к этой-же девке, то все поламается. Походу ломается на экстра левеле
        private void ChangeStep(int value)
        {
            _step += value;
            int maxCurentIndexStage = BundlesController.Instance.MainResourcesBundle.Sprites.Count - 1;
            if (_step < 0) _step = maxCurentIndexStage;
            else if (_step > maxCurentIndexStage) _step = 0;
            GameData.CurrentPuzzleStep = _step;
            if (BundlesController.Instance.MainResourcesBundle.Sprites.Count > _step)
                _contentImage.sprite = BundlesController.Instance.MainResourcesBundle.Sprites[_step];
            UpdateTextStep();

            //Если доступен
            if (_step == 0 || GameData.StageGirlLevel.IsUnlockedStage(GameData.CurrentLevel, _step) || GameData.PaidContent.IsUnlock(GameData.CurrentLevel))
            {
                _imageBlur.gameObject.SetActive(false);
                _buttonPlay.SetInteractable(true);
            }
            else
            {
                _imageBlur.gameObject.SetActive(true);
                _buttonPlay.SetInteractable(false);
            }
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