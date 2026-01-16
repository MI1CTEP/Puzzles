using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using MyGame.Bundles;

namespace MyGame.Gameplay.ExtraLevel
{
    public sealed class ExtraLevelUnlocker : MonoBehaviour
    {
        [SerializeField] private ExtraLevelValue _value;
        [SerializeField] private ExtraLevelBackground _background;
        [SerializeField] private ExtraLevelDetail _detail;
        [SerializeField] private CanvasGroup _detailBackground;
        [SerializeField] private Button _continueButton;

        private Sequence _seq;
        private UnityAction _onEnd;
        private Sprite _sprite;
        private List<int> _detailIds = new();
        private readonly Vector2Int _offset = new(4, 45);
        private readonly float _scale = 118f;

        public void Init()
        {
            _value.Init(GameData.ExtraLevel.PartSize.x * GameData.ExtraLevel.PartSize.y);
            _background.Init(GameData.ExtraLevel.PartSize.x * GameData.ExtraLevel.PartSize.y);
            LoadNewSprite();
            _detail.Init(Open);
            CreateArays(true);
            MixDetailIds();
            gameObject.SetActive(false);
            _continueButton.onClick.AddListener(End);
        }

        public void Show(UnityAction onEnd)
        {
            _onEnd = onEnd;
            _value.UpdateValue();
            _detailBackground.alpha = 0;
            _continueButton.gameObject.SetActive(false);
            _background.Hide();
            gameObject.SetActive(true);

            TryStopAnim();
            float timeAnim = 0.5f;
            _seq = DOTween.Sequence();
            _seq.Insert(0, _detailBackground.DOFade(1, timeAnim));
            _detail.Show(_seq, timeAnim);
            _seq.InsertCallback(timeAnim, () =>
                {
                    timeAnim = 1.2f;
                    TryStopAnim();
                    _seq = DOTween.Sequence();
                    _detail.FixateShowingAnim(_seq, timeAnim);
                });
        }

        private void LoadNewSprite()
        {
            _sprite = BundlesController.Instance.ForCollectBundle.TryGetSprite();
            _background.SetSprite(_sprite);
        }

        private void Open()
        {
            TryStopAnim();
            float timeAnim = 2f;
            _seq = DOTween.Sequence();
            _detail.Open(GetDetailSprite(), GetDetailPosition(), _scale, _seq, timeAnim);
            _seq.Insert(0, _detailBackground.DOFade(0, timeAnim / 2));
            _seq.InsertCallback(timeAnim, OnEndSetDetail);
            _background.Show(_seq, timeAnim / 2);
        }

        private Sprite GetDetailSprite()
        {
            if (_sprite == null)
                LoadNewSprite();
            Vector2Int idPosition = Vector2Int.zero;
            idPosition.x = _detailIds[0] % GameData.ExtraLevel.PartSize.x;
            idPosition.y = _detailIds[0] / GameData.ExtraLevel.PartSize.x;
            Rect rect = new Rect(idPosition.x * _scale + _offset.x, idPosition.y * _scale + _offset.y, _scale, _scale);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            return Sprite.Create(_sprite.texture, rect, pivot);
        }

        private Vector3 GetDetailPosition()
        {
            return _background.GetMaskPosition(_detailIds[0]) + new Vector3(_scale / 2, _scale / 2, 0);
        }

        private void OnEndSetDetail()
        {
            GameData.ExtraLevel.UnlockPart(_detailIds[0]);
            _background.DestroyMask(_detailIds[0]);
            _detailIds.RemoveAt(0);
            if(_detailIds.Count == 0)
            {
                GameData.ExtraLevel.UnlockLevel();
                _sprite = null;
                _detailIds = new();
                _value.ResetValue();
                CreateArays(false);
                MixDetailIds();
                BundlesController.Instance.ForCollectBundle.TryLoadLast(LoadNewSprite);
            }
            _continueButton.gameObject.SetActive(true);
        }

        private void End()
        {
            _onEnd?.Invoke();
            gameObject.SetActive(false);
        }

        private void CreateArays(bool createMask)
        {
            int id;
            for (int y = 0; y < GameData.ExtraLevel.PartSize.y; y++)
            {
                for (int x = 0; x < GameData.ExtraLevel.PartSize.x; x++)
                {
                    id = x + y * GameData.ExtraLevel.PartSize.x;
                    if (GameData.ExtraLevel.IsUnlockPart(id) == false)
                    {
                        _detailIds.Add(id);
                        if (createMask)
                            _background.CreateMask(new Vector2(x, y) * _scale, id);
                    }
                    else
                        _value.Addvalue();
                }
            }
        }

        private void MixDetailIds()
        {
            for (int i = 0; i < _detailIds.Count; i++)
            {
                int r = Random.Range(0, _detailIds.Count);
                (_detailIds[i], _detailIds[r]) = (_detailIds[r], _detailIds[i]);
            }
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