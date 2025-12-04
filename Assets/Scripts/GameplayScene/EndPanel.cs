using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using MyGame.Gameplay.Dialogue;
using MyGame.Gifts;

namespace MyGame.Gameplay
{
    public sealed class EndPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _sympathyTransform;
        [SerializeField] private RectTransform _scaleOldSympathy;
        [SerializeField] private RectTransform _scaleNewSympathy;
        [SerializeField] private TextMeshProUGUI _sympathyValueText;
        [SerializeField] private RectTransform _respectTransform;
        [SerializeField] private TextMeshProUGUI _respectAddedValueText;
        [SerializeField] private TextMeshProUGUI _respectValueText;
        [SerializeField] private RectTransform _giftTransform;
        [SerializeField] private Sprite _openedGift;
        [SerializeField] private Button _exitButton;

        private Image _backgroundImage;
        private Button _giftButton;
        private Image _giftImage;
        private DialogueController _dialogueController;
        private GiftController _giftController;
        private Sequence _seq;
        private readonly float _timeBackgroundFadeAnim = 2f;
        private readonly float _timeSympathyAnim = 3f;
        private readonly float _timeRespectAnim = 2f;
        private readonly float _timeGiftShowAnim = 0.5f;

        public void Init(DialogueController dialogueController, GiftController giftController)
        {
            _backgroundImage = GetComponent<Image>();
            _giftButton = _giftTransform.GetComponent<Button>();
            _giftImage = _giftTransform.GetComponent<Image>();
            _sympathyTransform.gameObject.SetActive(false);
            _respectTransform.gameObject.SetActive(false);
            _giftTransform.gameObject.SetActive(false);
            _exitButton.gameObject.SetActive(false);
            _dialogueController = dialogueController;
            _giftController = giftController;
        }

        public void Show()
        {
            float waitTimeAnim = 0;
            int olsSympathy = GameData.Sympathy.Load(GameData.CurrentLevel);
            GameData.Sympathy.Save(GameData.CurrentLevel, _dialogueController.CurrentSympathy);
            TryStopAnim();
            _seq = DOTween.Sequence();
            _seq.Insert(0, _backgroundImage.DOFade(0.8f, _timeBackgroundFadeAnim));
            ShowSympathy(waitTimeAnim, olsSympathy);
            waitTimeAnim += _timeSympathyAnim;
            if (_dialogueController.CurrentSympathy > olsSympathy)
            {
                int addedRespect = _dialogueController.CurrentSympathy - olsSympathy;
                ShowRespect(waitTimeAnim, addedRespect);
                GameData.Score.Add(addedRespect);
                waitTimeAnim += _timeRespectAnim;
            }
            ShowGift(waitTimeAnim);
        }

        private void ShowSympathy(float timeWait, int olsSympathy)
        {
            _sympathyValueText.text = $"{_dialogueController.CurrentSympathy}/{_dialogueController.MaxSympathy}";
            float valueSize = _scaleOldSympathy.anchoredPosition.x / _dialogueController.MaxSympathy;
            _scaleOldSympathy.anchoredPosition = new Vector2(olsSympathy * valueSize, 0);
            _scaleNewSympathy.anchoredPosition = Vector2.zero;
            _sympathyTransform.gameObject.SetActive(true);
            _seq.Insert(timeWait, _sympathyTransform.DOLocalMoveY(0, _timeSympathyAnim / 2).SetEase(Ease.OutExpo));
            _seq.Insert(timeWait + _timeSympathyAnim / 4, _scaleNewSympathy.DOAnchorPosX(_dialogueController.CurrentSympathy * valueSize, _timeSympathyAnim / 2));
            _seq.Insert(timeWait + _timeSympathyAnim / 2, _sympathyTransform.DOAnchorPosY(0, _timeSympathyAnim / 2).SetEase(Ease.InExpo));
        }

        private void ShowRespect(float timeWait, int addedRespect)
        {
            _respectValueText.text = GameData.Score.Load().ToString();
            _respectAddedValueText.text = $"+{addedRespect}";
            _respectTransform.gameObject.SetActive(true);
            _seq.Insert(timeWait, _respectTransform.DOLocalMoveY(0, _timeRespectAnim / 2).SetEase(Ease.OutExpo));
            _seq.Insert(timeWait + _timeRespectAnim / 4, _respectValueText.transform.DOScale(Vector3.one * 1.25f, _timeRespectAnim / 8));
            _seq.InsertCallback(timeWait + _timeRespectAnim / 2, () => _respectValueText.text = GameData.Score.Load().ToString());
            _seq.Insert(timeWait + _timeRespectAnim / 2, _respectValueText.transform.DOScale(Vector3.one, _timeRespectAnim / 8));
            _seq.Insert(timeWait + _timeRespectAnim / 2, _respectTransform.DOAnchorPosY(-_sympathyTransform.sizeDelta.y, _timeRespectAnim / 2).SetEase(Ease.InExpo));
        }

        private void ShowGift(float timeWait)
        {
            _giftTransform.gameObject.SetActive(true);
            _seq.Insert(timeWait, _giftTransform.DOScale(Vector3.one * 1.1f, _timeGiftShowAnim).SetEase(Ease.OutBack));
            _seq.InsertCallback(timeWait + _timeGiftShowAnim, FixateGiftAnim);
        }

        private void FixateGiftAnim()
        {
            _giftButton.onClick.AddListener(OpenGiftAnim);
            TryStopAnim();
            _seq = DOTween.Sequence();
            _seq.Insert(0, _giftTransform.DOScale(Vector3.one, _timeGiftShowAnim));
            _seq.Insert(_timeGiftShowAnim, _giftTransform.DOScale(Vector3.one * 1.1f, _timeGiftShowAnim));
            _seq.SetLoops(-1);
        }

        private void OpenGiftAnim()
        {
            _giftButton.onClick.RemoveAllListeners();
            TryStopAnim();
            _seq = DOTween.Sequence();
            _seq.Insert(0, _giftTransform.DOScale(Vector3.one, 0.1f));
            _seq.Append(_giftTransform.DOLocalRotate(new Vector3(0, 0, -5), 0.05f));
            _seq.Append(_giftTransform.DOLocalRotate(new Vector3(0, 0, 5), 0.1f));
            _seq.Append(_giftTransform.DOLocalRotate(new Vector3(0, 0, -5), 0.1f));
            _seq.Append(_giftTransform.DOLocalRotate(new Vector3(0, 0, 5), 0.1f));
            _seq.Append(_giftTransform.DOLocalRotate(new Vector3(0, 0, -5), 0.1f));
            _seq.Append(_giftTransform.DOLocalRotate(new Vector3(0, 0, 0), 0.05f));
            _seq.Append(_giftTransform.DOScale(Vector3.one * 1.1f, 0.25f));
            _seq.AppendCallback(() => _giftImage.sprite = _openedGift);
            _seq.Append(_giftTransform.DOScale(Vector3.one, 0.25f));
            _seq.AppendCallback(() => _giftController.ShowRoulette(OnEndShowing));
        }

        private void OnEndShowing()
        {
            _exitButton.gameObject.SetActive(true);
        }

        private void TryStopAnim()
        {
            if (_seq != null)
                _seq.Kill();
        }

        private void OnDestroy()
        {
            TryStopAnim();
        }
    }
}