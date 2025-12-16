using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardUIManager : MonoBehaviour
{
    public static CardUIManager Instance;           // Синглтон

    public CardItem _currentCard;                   // Текущая карточка
    public CardItem cardPrefab;                     // Префаб карточки
    public CardData[] cardDatas;                    // Данные карточек

    [Header("UI Элементы (общие)")]
    [SerializeField] private Slider progressSlider;     // Полоса прогресса
    [SerializeField] private Slider sympathySlider;     // Симпатия
    [SerializeField] private Button revealButton;       // Кнопка "Открыть"
    [SerializeField] private Button leftButton;         // Кнопка "Назад"
    [SerializeField] private Button rightButton;        // Кнопка "Вперед"

    [Header("Анимации")]
    public float swipeThreshold = 100f;     // Минимальное движение для свайпа
    public float flyAwayDistance = 1000f;   // расстояние улетания по Y

    private Vector3 cardInitialPosition;        // Начальная позиция карточки
    private Vector2 startSwipePosition;         // Начальная позиция свайпа

    private bool isDragging = false;            // Флаг перетаскивания
    private int currentCardIndex = 0;  // Индекс текущей карточки (для зацикливания)

    private void OnEnable() => CreateNewCard();      // Создать новую карточку при активации

    private void CreateNewCard()                     // Создание новой карточки
    {
        if (cardDatas.Length == 0) return;

        int index = currentCardIndex % cardDatas.Length;
        var data = cardDatas[index];

        var cardObj = Instantiate(cardPrefab, transform);
        cardObj.name = cardDatas[index].characterName;
        cardObj.transform.localScale = Vector3.zero;

        cardObj.SetData(data);

        _currentCard = cardObj;
        cardInitialPosition = cardObj.transform.position;

        cardObj.transform.DOScale(new Vector3(4.5f, 5, 5), 0.5f).SetEase(Ease.OutBack).SetUpdate(true);   // Анимация появления

        UpdateUI();     // Обновление UI

        SetupEventTriggers(cardObj.gameObject); // Настройка свайпа
    }

    private void SetupEventTriggers(GameObject cardObj)      // Настройка триггеров событий для карточки
    {
        if (!cardObj.TryGetComponent<EventTrigger>(out var eventTrigger)) eventTrigger = cardObj.AddComponent<EventTrigger>();

        eventTrigger.triggers.Clear();

        var pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.BeginDrag };
        pointerDown.callback.AddListener(data => { OnBeginDrag((PointerEventData)data); });
        eventTrigger.triggers.Add(pointerDown);

        var drag = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
        drag.callback.AddListener(data => { OnDrag((PointerEventData)data); });
        eventTrigger.triggers.Add(drag);

        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.EndDrag };
        pointerUp.callback.AddListener(data => { OnEndDrag((PointerEventData)data); });
        eventTrigger.triggers.Add(pointerUp);
    }

    private void OnBeginDrag(PointerEventData data)      // Начало перетаскивания карточки
    {
        isDragging = true;
        startSwipePosition = data.position;  // Фиксируем начало свайпа
    }

    private void OnDrag(PointerEventData data)           // Перетаскивание карточки
    {
        if (_currentCard == null || !isDragging) return;

        Vector3 pos = _currentCard.transform.position;  // Только по оси Y
        pos.y += data.delta.y;
        _currentCard.transform.position = pos;
    }

    private void OnEndDrag(PointerEventData data)        // Окончание перетаскивания карточки
    {
        if (_currentCard == null) return;

        isDragging = false;

        float swipeDeltaY = data.position.y - startSwipePosition.y; // Общее смещение по Y за весь свайп

        if (Mathf.Abs(swipeDeltaY) > swipeThreshold)    // Проверяем, превышено ли пороговое значение
        {
            Vector3 direction = swipeDeltaY > 0 ? Vector3.up : Vector3.down;
            SwipeCard(direction);
        }
        else _currentCard.transform.DOMove(cardInitialPosition, 0.3f).SetEase(Ease.OutQuad).SetUpdate(true); // Возврат на место
    }

    private void SwipeCard(Vector3 direction)           // Свайп карточки
    {
        if (_currentCard == null) return;

        Vector3 endPos = _currentCard.transform.position + direction * flyAwayDistance;

        _currentCard.transform.DOMove(endPos, 0.5f).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                Destroy(_currentCard.gameObject);
                currentCardIndex++;
                CreateNewCard();
            }).SetUpdate(true);
    }

    private void UpdateUI()                             // Обновление UI элемента
    {
        if (_currentCard == null) return;

        var data = _currentCard.GetData();              // Получаем данные карточки

        progressSlider.value = data.GetUnlockProgress();    // Обновляем прогресс
        sympathySlider.value = data.sympathy;        // Обновляем симпатию

        revealButton.onClick.RemoveAllListeners();
        revealButton.onClick.AddListener(() => { _currentCard.FlipCard(); });  // Запускаем анимацию переворота
        
        leftButton.onClick.RemoveAllListeners();
        leftButton.onClick.AddListener(_currentCard.OnLeft);    // Обработчик кнопки "Назад"

        rightButton.onClick.RemoveAllListeners();
        rightButton.onClick.AddListener(_currentCard.OnRight);  // Обработчик кнопки "Вперед"
    }

    public void OnCardMediaChanged() => UpdateUI();     // Обновление UI при изменении медиа

    private void OnDisable() => Destroy(_currentCard.gameObject);        // Уничтожение карточки при закрытии менеджера
}