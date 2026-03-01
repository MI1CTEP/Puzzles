using UnityEngine;
using UnityEngine.Events;

namespace MyGame.Menu
{
    public sealed class SettingsPanel : MenuPanel
    {
        [SerializeField] private GameObject _audio;
        [SerializeField] private GameObject _languages;
        [SerializeField] private MenuButton _closeButton;

        public void Init(UnityAction onShowMainPanel)
        {
            MenuPanelInit();
            gameObject.SetActive(false);
            _audio.SetActive(false);
            _languages.SetActive(false);
            _closeButton.Init(onShowMainPanel);

            SetSettingsScreen();
        }

        protected override void OnStartShow() { }

        protected override void OnEndShow() 
        {
            _audio.SetActive(true);
            _languages.SetActive(true);
            _closeButton.Show();
        }


        protected override void OnHide()
        {
            _audio.SetActive(false);
            _languages.SetActive(false);
            _closeButton.Hide();
        }

        //Установка экрана в зависимости от платформы
        private void SetSettingsScreen()
        {
            if (NutakuAPIInitializator.instance.TypePlatform == TypePlatform.Editor || NutakuAPIInitializator.instance.TypePlatform == TypePlatform.WebGL)
            {
                //float scale = 0.7f;
                //transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }
}