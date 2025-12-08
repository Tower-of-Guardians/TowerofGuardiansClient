using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardInventoryUI : MonoBehaviour
{

    [Header("Test Button")]
    public Button testButton;

    [Header("Left UI Tabs")]
    public Button cardTabButton;
    public Button otherTabButton;

    [Header("Right UI Sort Buttons")]
    public Button sortByAcquisitionButton;
    public Button sortByGradeButton;
    public Button sortByStrengthButton;
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

    private bool isAcquisitionAscending = true;
    private bool isGradeAscending = false;
    private bool isStrengthAscending = true;

    void Start()
    {
        InitializeButtons();
        InitializeCardInventoryUI();
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

        // 오른쪽 정렬 버튼 초기화
        if (sortByAcquisitionButton != null)
        {
            sortByAcquisitionButton.onClick.AddListener(OnSortByAcquisitionClicked);
        }

        if (sortByGradeButton != null)
        {
            sortByGradeButton.onClick.AddListener(OnSortByGradeClicked);
        }

        if (sortByStrengthButton != null)
        {
            sortByStrengthButton.onClick.AddListener(OnSortByStrengthClicked);
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

    // 획득순 정렬 기능
    private void OnSortByAcquisitionClicked()
    {
        // 오름차순/내림차순 토글
        isAcquisitionAscending = !isAcquisitionAscending;
        
        // TODO: 카드를 획득순으로 정렬하는 로직 구현
    }

    // 등급순 정렬 기능
    private void OnSortByGradeClicked()
    {
        // 오름차순/내림차순 토글
        isGradeAscending = !isGradeAscending;
        
        // TODO: 카드를 등급순으로 정렬하는 로직 구현
    }

    // 강화순 정렬 기능
    private void OnSortByStrengthClicked()
    {
        // 오름차순/내림차순 토글
        isStrengthAscending = !isStrengthAscending;
        
        // TODO: 카드를 강화순으로 정렬하는 로직 구현
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

        if (DataCenter.Instance == null || !DataCenter.IsDataLoaded)
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

        foreach (string cardId in DataCenter.Instance.userDeck)
        {
            if (DataCenter.card_datas.TryGetValue(cardId, out CardData cardData))
            {
                cards[cardId] = cardData;
            }
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

    /// Content Height 동적 조정
    private void UpdateContentHeight(int cardCount)
    {
        if (cardInventoryContent == null)
        {
            return;
        }

        RectTransform contentRect = cardInventoryContent.GetComponent<RectTransform>();
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
        if (DataCenter.Instance != null && DataCenter.IsDataLoaded)
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

        if (sortByAcquisitionButton != null)
        {
            sortByAcquisitionButton.onClick.RemoveListener(OnSortByAcquisitionClicked);
        }

        if (sortByGradeButton != null)
        {
            sortByGradeButton.onClick.RemoveListener(OnSortByGradeClicked);
        }

        if (sortByStrengthButton != null)
        {
            sortByStrengthButton.onClick.RemoveListener(OnSortByStrengthClicked);
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(OnNextButtonClicked);
        }
    }
}
