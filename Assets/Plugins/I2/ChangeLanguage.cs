using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class ChangeLanguage : MonoBehaviour
{
    [SerializeField] private Text _label;
    [SerializeField] private Dropdown _dropdown;

    private static ChangeLanguage _instance;

    private void Start()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }

        _dropdown.ClearOptions();
        foreach (string languageName in LocalizationManager.GetAllLanguages())
        {
            var langAdd = new Dropdown.OptionData {  text = languageName };
            _dropdown.options.Add(langAdd);
        }
        _dropdown.onValueChanged.AddListener(delegate { PressLanguage(); });
    }

    private void PressLanguage()
    {
        string lang = _label.text;
        LocalizationManager.CurrentLanguage = lang;
    }
}