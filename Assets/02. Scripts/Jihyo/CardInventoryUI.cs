using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CardInventoryUI : MonoBehaviour
{

    [Header("Test Button")]
    public Button testButton;

    [Header("Left UI Tabs")]
    public Button cardTabButton;
    public Button otherTabButton;

    [Header("Right UI Sort")]
    public TextMeshProUGUI sortText;
    public Button leftButton;
    public Button rightButton;
    public Button asDesButton;
    public Button nextButton;

    [Header("Card Inventory Panel")]
    [SerializeField] private GameObject cardInventoryPanel;
    
    [Header("Card Info UI")]
    [SerializeField] private CardInfoUI cardInfoUI;

    [Header("Card UI")]
    [SerializeField] private GameObject cardInventoryContent;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<GameObject> instantiatedCards = new List<GameObject>();

    private const int CARDS_PER_ROW = 6;
    private const int INITIAL_HEIGHT = 800;
    private const int HEIGHT_INCREMENT = 300;
    private const int INITIAL_CARD_COUNT = 18;

    private enum SortType
    {
        Acquisition,
        Grade,
        Attack,
        Defense
    }

    private SortType currentSortType = SortType.Acquisition;
    private bool isAscending = true;

    void Start()
    {
        InitializeButtons();
        InitializeCardInventoryUI();
        UpdateSortText();
    }

    private void InitializeCardInventoryUI()
    {
        if (cardInfoUI == null)
        {
            cardInfoUI = FindAnyObjectByType<CardInfoUI>();
        }

        if (testButton != null)
        {
            // Test 기능 : 패널 열기
            testButton.onClick.AddListener(OpenPanel);
        }

        if (cardInventoryPanel != null)
        {
            cardInventoryPanel.SetActive(false);
        }
    }

    private void InitializeButtons()
    {
        // 왼쪽 탭 버튼 초기화
        if (cardTabButton != null)
        {
            cardTabButton.onClick.AddListener(OnCardTabClicked);
        }

        if (otherTabButton != null)
        {
            otherTabButton.onClick.AddListener(OnOtherTabClicked);
        }

        // 정렬 버튼 초기화
        if (leftButton != null)
        {
            leftButton.onClick.AddListener(OnLeftButtonClicked);
        }

        if (rightButton != null)
        {
            rightButton.onClick.AddListener(OnRightButtonClicked);
        }

        if (asDesButton != null)
        {
            asDesButton.onClick.AddListener(OnAsDesButtonClicked);
        }

        // Next 버튼 초기화
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }
    }

    // 왼쪽 UI의 탭 전환 기능
    private void OnCardTabClicked()
    {
        // TODO: 카드 탭 UI로 전환하는 로직 구현
    }

    // 왼쪽 UI의 탭 전환 기능
    private void OnOtherTabClicked()
    {
        // TODO: 다른 탭 UI로 전환하는 로직 구현
    }

    // 왼쪽 버튼 클릭 (이전 정렬 타입)
    private void OnLeftButtonClicked()
    {
        currentSortType = GetPreviousSortType();
        UpdateSortText();
        
        // TODO: 데이터 인덱스 작업 중 - 정렬 기능 구현 예정
    }

    // 오른쪽 버튼 클릭 (다음 정렬 타입)
    private void OnRightButtonClicked()
    {
        currentSortType = GetNextSortType();
        UpdateSortText();
        
        // TODO: 데이터 인덱스 작업 중 - 정렬 기능 구현 예정
    }

    // 오름차순/내림차순 토글 버튼 클릭
    private void OnAsDesButtonClicked()
    {
        isAscending = !isAscending;
        
        // TODO: UI 작업 중 - 일단 Rotation Z를 180도 돌려서 위아래로 표시
        if (asDesButton != null)
        {
            RectTransform buttonRect = asDesButton.GetComponent<RectTransform>();
            if (buttonRect != null)
            {
                float currentRotation = buttonRect.localEulerAngles.z;
                float newRotation = isAscending ? 0f : 180f;
                buttonRect.localRotation = Quaternion.Euler(0f, 0f, newRotation);
            }
        }
        
        // TODO: 데이터 인덱스 작업 중 - 정렬 기능 구현 예정
    }

    // 이전 정렬 타입 가져오기
    private SortType GetPreviousSortType()
    {
        switch (currentSortType)
        {
            case SortType.Acquisition:
                return SortType.Defense;
            case SortType.Grade:
                return SortType.Acquisition;
            case SortType.Attack:
                return SortType.Grade;
            case SortType.Defense:
                return SortType.Attack;
            default:
                return SortType.Acquisition;
        }
    }

    // 다음 정렬 타입 가져오기
    private SortType GetNextSortType()
    {
        switch (currentSortType)
        {
            case SortType.Acquisition:
                return SortType.Grade;
            case SortType.Grade:
                return SortType.Attack;
            case SortType.Attack:
                return SortType.Defense;
            case SortType.Defense:
                return SortType.Acquisition;
            default:
                return SortType.Acquisition;
        }
    }

    // 정렬 텍스트 업데이트
    private void UpdateSortText()
    {
        if (sortText == null)
        {
            return;
        }

        string sortTypeText = currentSortType switch
        {
            SortType.Acquisition => "획득순 정렬",
            SortType.Grade => "등급순 정렬",
            SortType.Attack => "공격력순 정렬",
            SortType.Defense => "보호력순 정렬",
            _ => "획득순 정렬"
        };

        sortText.text = sortTypeText;
    }

    private void OnNextButtonClicked()
    {
        ClosePanel();
    }

    // 패널 열기
    public void OpenPanel()
    {
        cardInventoryPanel.SetActive(true);
        RefreshCardInventory();
    }

    /// 특정 컨테이너에 카드 목록 표시
    public void DisplayCardsInContainer(GameObject container)
    {
        if (container == null)
        {
            Debug.LogWarning("CardInventoryUI: container가 설정되지 않았습니다.");
            return;
        }

        if (cardPrefab == null)
        {
            Debug.LogWarning("CardInventoryUI: cardPrefab이 설정되지 않았습니다.");
            return;
        }

        if (DataCenter.Instance == null || !DataCenter.IsCardDataLoaded)
        {
            Debug.LogWarning("CardInventoryUI: DataCenter가 아직 데이터를 로드하지 않았습니다.");
            return;
        }

        // 기존 카드 제거 (해당 컨테이너의 자식만)
        ClearCardsInContainer(container);

        // 덱에서 카드 가져오기
        Dictionary<string, CardData> displayCard = GetUserCards();

        foreach (var kvp in displayCard)
        {
            string cardId = kvp.Key;
            CardData cardData = kvp.Value;

            if (cardData == null)
            {
                continue;
            }

            GameObject cardObject = Instantiate(cardPrefab, container.transform);
            instantiatedCards.Add(cardObject);

            SetupCardData(cardObject, cardData, cardId);

            SetupCardClickHandler(cardObject);
        }

        // Content Height 동적 조정
        UpdateContentHeightInContainer(container, displayCard.Count);
    }

    /// 인벤토리 열 때마다 카드 목록을 새로고침
    private void RefreshCardInventory()
    {
        if (cardInventoryContent == null)
        {
            Debug.LogWarning("CardInventoryUI: cardInventoryContent가 설정되지 않았습니다.");
            return;
        }

        if (cardPrefab == null)
        {
            Debug.LogWarning("CardInventoryUI: cardPrefab이 설정되지 않았습니다.");
            return;
        }

        if (DataCenter.Instance == null || !DataCenter.IsCardDataLoaded)
        {
            Debug.LogWarning("CardInventoryUI: DataCenter가 아직 데이터를 로드하지 않았습니다.");
            return;
        }

        ClearCards();

        // 덱에서 카드 가져오기
        Dictionary<string, CardData> displayCard = GetUserCards();

        foreach (var kvp in displayCard)
        {
            string cardId = kvp.Key;
            CardData cardData = kvp.Value;

            if (cardData == null)
            {
                continue;
            }

            GameObject cardObject = Instantiate(cardPrefab, cardInventoryContent.transform);
            instantiatedCards.Add(cardObject);

            SetupCardData(cardObject, cardData, cardId);

            SetupCardClickHandler(cardObject);
        }

        // Content Height 동적 조정
        UpdateContentHeight(displayCard.Count);
    }

    /// 사용자가 보유한 카드 목록 가져오기
    private Dictionary<string, CardData> GetUserCards()
    {
        Dictionary<string, CardData> cards = new Dictionary<string, CardData>();

        if (DataCenter.Instance == null || DataCenter.Instance.userDeck == null)
        {
            return cards;
        }

        foreach (CardData data in DataCenter.Instance.userDeck)
        {
            cards[data.id] = data;
        }

        return cards;
    }

    /// 카드 오브젝트에 데이터 설정
    private void SetupCardData(GameObject cardObject, CardData cardData, string cardId)
    {
        if (cardObject == null || cardData == null)
        {
            return;
        }

        // 카드 오브젝트 이름에 카드 ID 저장
        cardObject.name = $"Card_{cardId}";

        InventoryCard inventoryCard = cardObject.GetComponent<InventoryCard>();
        if (inventoryCard == null)
        {
            inventoryCard = cardObject.AddComponent<InventoryCard>();
        }

        inventoryCard.InitUI(cardData);
    }

    /// 카드 클릭 핸들러 설정
    private void SetupCardClickHandler(GameObject cardObject)
    {
        if (cardObject == null)
        {
            return;
        }

        Button button = cardObject.GetComponent<Button>();
        if (button == null)
        {
            button = cardObject.AddComponent<Button>();
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            CardData cardData = GetClickedCardData(cardObject);
            OnCardClicked(cardData);
        });
    }

    /// 기존 카드 제거
    private void ClearCards()
    {
        foreach (GameObject card in instantiatedCards)
        {
            if (card != null)
            {
                Destroy(card);
            }
        }
        instantiatedCards.Clear();
    }

    /// 특정 컨테이너의 카드만 제거
    private void ClearCardsInContainer(GameObject container)
    {
        if (container == null)
        {
            return;
        }

        // 해당 컨테이너의 자식 카드들만 제거
        List<GameObject> cardsToRemove = new List<GameObject>();
        foreach (GameObject card in instantiatedCards)
        {
            if (card != null && card.transform.parent == container.transform)
            {
                cardsToRemove.Add(card);
            }
        }

        foreach (GameObject card in cardsToRemove)
        {
            if (card != null)
            {
                Destroy(card);
            }
            instantiatedCards.Remove(card);
        }
    }

    /// Content Height 동적 조정
    private void UpdateContentHeight(int cardCount)
    {
        if (cardInventoryContent == null)
        {
            return;
        }

        UpdateContentHeightInContainer(cardInventoryContent, cardCount);
    }

    /// 특정 컨테이너의 Content Height 동적 조정
    private void UpdateContentHeightInContainer(GameObject container, int cardCount)
    {
        if (container == null)
        {
            return;
        }

        RectTransform contentRect = container.GetComponent<RectTransform>();
        if (contentRect == null)
        {
            return;
        }

        float newHeight = INITIAL_HEIGHT;
        
        if (cardCount > INITIAL_CARD_COUNT)
        {
            int extraRows = Mathf.CeilToInt((float)(cardCount - INITIAL_CARD_COUNT) / CARDS_PER_ROW);
            newHeight = INITIAL_HEIGHT + (extraRows * HEIGHT_INCREMENT);
        }

        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, newHeight);
    }

    // 패널 닫기
    public void ClosePanel()
    {
        if (cardInfoUI != null)
        {
            cardInfoUI.HidePanel();
        }
        cardInventoryPanel.SetActive(false);
    }

    // 카드 클릭 시 호출되는 메서드
    public void OnCardClicked(CardData cardData)
    {
        if (cardData == null)
        {
            return;
        }

        // CardInfoUI에 카드 정보 표시 요청
        if (cardInfoUI != null)
        {
            cardInfoUI.ShowCardInfo(cardData);
        }
        else
        {
            Debug.LogWarning("CardInventoryUI: cardInfoUI가 설정되지 않았습니다.");
        }
    }

    // 클릭한 카드의 정보를 받아오기
    private CardData GetClickedCardData(GameObject clickedCard)
    {
        if (clickedCard == null)
        {
            return null;
        }

        string cardName = clickedCard.name;
        if (string.IsNullOrEmpty(cardName) || !cardName.StartsWith("Card_"))
        {
            return null;
        }

        string cardId = cardName.Substring(5); // "Card_" 제거

        // DataCenter에서 카드 데이터 가져오기
        if (DataCenter.Instance != null && DataCenter.IsCardDataLoaded)
        {
            if (DataCenter.card_datas.TryGetValue(cardId, out CardData cardData))
            {
                return cardData;
            }
        }

        return null;
    }


    void OnDestroy()
    {
        // 이벤트 리스너 제거
        if (cardTabButton != null)
        {
            cardTabButton.onClick.RemoveListener(OnCardTabClicked);
        }

        if (otherTabButton != null)
        {
            otherTabButton.onClick.RemoveListener(OnOtherTabClicked);
        }

        if (leftButton != null)
        {
            leftButton.onClick.RemoveListener(OnLeftButtonClicked);
        }

        if (rightButton != null)
        {
            rightButton.onClick.RemoveListener(OnRightButtonClicked);
        }

        if (asDesButton != null)
        {
            asDesButton.onClick.RemoveListener(OnAsDesButtonClicked);
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(OnNextButtonClicked);
        }
    }
}
