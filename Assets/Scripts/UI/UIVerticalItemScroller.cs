using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIVerticalItemScroller : MonoBehaviour
{
    [SerializeField] private CardData[] cardDatas;           // Массив данных
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private GameObject scrollView;
    [SerializeField] private GameObject cardPrefab;         // Теперь общий префаб
    [SerializeField] private float closeShrinkDuration = 0.3f;
    [SerializeField] private float checkInterval = 0.1f;    // Как часто проверять видимую карточку
    [SerializeField] private Transform viewCenter;         // Позиция центра просмотра

    private readonly List<CardItem> spawnedCards = new();
    private CardItem _currentVisibleCard;
    private Coroutine _checkVisibleCardRoutine;

    public static event System.Action<CardData> OnCardVisible; // Для UI, например, заголовок
    public static event System.Action<CardItem> OnCardRevealed;

    private void Awake()
    {
        OnCardRevealed += OnCardWasRevealed;
    }

    private void OnCardWasRevealed(CardItem revealedCard)
    {
        // Перезапустить проверку видимой карточки, если она изменилась
        if (_checkVisibleCardRoutine != null)
            StopCoroutine(_checkVisibleCardRoutine);
        _checkVisibleCardRoutine = StartCoroutine(CheckVisibleCardRoutine());
    }

    public void Open()
    {
        Close(); // Очистка

        contentPanel.anchoredPosition = new Vector2(0, -8000);
        scrollView.SetActive(true);
        contentPanel.DOAnchorPosY(0, 1);

        for (int i = 0; i < cardDatas.Length; i++)
        {
            GameObject go = Instantiate(cardPrefab, contentPanel);
            CardItem card = go.GetComponent<CardItem>();
            card.SetData(cardDatas[i], OnCardRevealed);
            spawnedCards.Add(card);
        }

        // Запускаем отслеживание
        if (_checkVisibleCardRoutine != null)
            StopCoroutine(_checkVisibleCardRoutine);
        _checkVisibleCardRoutine = StartCoroutine(CheckVisibleCardRoutine());
    }

    private System.Collections.IEnumerator CheckVisibleCardRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            CardItem bestCard = null;
            float closestDistance = float.MaxValue;

            foreach (var card in spawnedCards)
            {
                if (card == null) continue;
                float dist = Vector2.Distance(
                    RectTransformUtility.WorldToScreenPoint(null, card.transform.position),
                    RectTransformUtility.WorldToScreenPoint(null, viewCenter.position)
                );
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    bestCard = card;
                }
            }

            if (bestCard != null && bestCard != _currentVisibleCard)
            {
                _currentVisibleCard = bestCard;
                OnCardVisible?.Invoke(cardDatas[spawnedCards.IndexOf(bestCard)]);
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    public void Close()
    {
        DOTween.Kill(contentPanel);
        if (_checkVisibleCardRoutine != null)
        {
            StopCoroutine(_checkVisibleCardRoutine);
            _checkVisibleCardRoutine = null;
        }

        foreach (var card in spawnedCards)
        {
            if (card == null) continue;
            RectTransform rt = card.GetComponent<RectTransform>();
            rt.DOScale(Vector3.zero, closeShrinkDuration).OnComplete(() => Destroy(card.gameObject));
        }

        spawnedCards.Clear();
        scrollView.SetActive(false);
    }
}