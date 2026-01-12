using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using MyGame.Gameplay.Dialogue;
using MyGame.Gifts;
using MyGame.Bundles;

namespace MyGame.Gameplay
{
    public sealed class EndPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _canvas;
        [SerializeField] private ProgressPanel _progressPanel;
        [SerializeField] private RespectController _respectController;
        [SerializeField] private RectTransform _giftTransform;
        [SerializeField] private Sprite _openedGift;
        [SerializeField] private Button _exitButton;

        private Image _backgroundImage;
        private Button _giftButton;
        private Image _giftImage;
        private DialogueController _dialogueController;
        private GiftController _giftController;
        private Achievements _achievements;
        private Sequence _seq;
        private readonly float _timeBackgroundFadeAnim = 2f;
        private readonly float _timeSympathyAnim = 3f;
        private readonly float _timeRespectAnim = 1.5f;
        private readonly float _timeAchievementsAnim = 2f;
        private readonly float _timeGiftShowAnim = 0.5f;
        private float _anchoredPosY = -160;

        public void Init(DialogueController dialogueController, GiftController giftController, Achievements achievements)
        {
            gameObject.SetActive(false);
            _backgroundImage = GetComponent<Image>();
            _giftButton = _giftTransform.GetComponent<Button>();
            _giftImage = _giftTransform.GetComponent<Image>();
            _giftTransform.gameObject.SetActive(false);
            _exitButton.gameObject.SetActive(false);
            _exitButton.onClick.AddListener(Exit);
            _dialogueController = dialogueController;
            _giftController = giftController;
            _achievements = achievements;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            float waitTimeAnim = 0;
            int oldSympathy = GameData.Sympathy.Load(GameData.CurrentLevel);
            GameData.Sympathy.Save(GameData.CurrentLevel, _dialogueController.CurrentSympathy);
            TryStopAnim();
            _seq = DOTween.Sequence();
            _seq.Insert(0, _backgroundImage.DOFade(0.8f, _timeBackgroundFadeAnim));
            _progressPanel.ShowEnd(transform, _canvas.sizeDelta.y, _dialogueController.CurrentSympathy, oldSympathy, _dialogueController.MaxSympathy, _timeSympathyAnim, ref waitTimeAnim);
            if (_dialogueController.CurrentSympathy > oldSympathy)
            {
                int addedRespect = _dialogueController.CurrentSympathy - oldSympathy;
                _respectController.ShowEnd(transform, _anchoredPosY, _canvas.sizeDelta.y, addedRespect, _timeRespectAnim, ref waitTimeAnim);
                GameData.Respect.Add(addedRespect);
                _anchoredPosY -= 160;
            }
            if (_achievements.IsHaveAchievements())
            {
                ShowAchievements(ref waitTimeAnim);
            }
            ShowGift(waitTimeAnim);
        }

        private void ShowAchievements(ref float waitTimeAnim)
        {
            RectTransform rectTransform = _achievements.GetComponent<RectTransform>();
            _achievements.gameObject.SetActive(true);
            _seq.Insert(waitTimeAnim, rectTransform.DOLocalMoveY(0, _timeAchievementsAnim * 2 / 3).SetEase(Ease.OutExpo));
            _seq.Insert(waitTimeAnim + _timeAchievementsAnim * 2 / 3, rectTransform.DOAnchorPosY(_anchoredPosY, _timeAchievementsAnim / 3).SetEase(Ease.InExpo));
            waitTimeAnim += _timeAchievementsAnim;
            _anchoredPosY -= rectTransform.sizeDelta.y;
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
            _seq.AppendCallback(() => _giftController.ShowRoulette(_anchoredPosY, OnEndShowing));
        }

        private void OnEndShowing()
        {
            _exitButton.gameObject.SetActive(true);
        }

        private void Exit()
        {
            BundlesController.Instance.OnlyGameplayBundle.TryUnload();
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