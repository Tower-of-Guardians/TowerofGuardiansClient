using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class CardInventoryUI : MonoBehaviour, IPointerClickHandler
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

    [Header("Enhancement UI")]
    [SerializeField] private GameObject cardInventoryPanel;
    [SerializeField] private GameObject enhancementPanel;
    [SerializeField] private Toggle enhancementPreviewToggle;
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text cardDescriptionText;
    [SerializeField] private GameObject cardGridParent;

    [Header("Card UI")]
    [SerializeField] private GameObject cardInventoryContent;
    //[SerializeField] private GameObject cardUIPrefab;
    //[SerializeField] private List<GameObject> instantiatedCards = new List<GameObject>();
    private CardData selectedCardData;

    private bool isAcquisitionAscending = true;
    private bool isGradeAscending = false;
    private bool isStrengthAscending = true;

    void Start()
    {
        InitializeButtons();
        InitializeEnhancementUI();
    }

    private void InitializeEnhancementUI()
    {

        if (testButton != null)
        {
            // Test 기능 : 패널 열기
            testButton.onClick.AddListener(OpenPanel);
        }

        if (enhancementPanel != null)
        {
            enhancementPanel.SetActive(false);
        }

        if (enhancementPreviewToggle != null)
        {
            enhancementPreviewToggle.onValueChanged.AddListener(OnEnhancementPreviewToggleChanged);
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
        SetupCardClickHandlers();
    }

    /// Content 안의 게임오브젝트들에 OnCardClicked를 AddListener로 연결
    /// TODO: 추후 CardUI 스크립트 생성 시 변경 예정
    private void SetupCardClickHandlers()
    {
        if (cardInventoryContent == null)
        {
            return;
        }

        // Content 안의 모든 자식 게임오브젝트 찾기
        for (int i = 0; i < cardInventoryContent.transform.childCount; i++)
        {
            GameObject cardObject = cardInventoryContent.transform.GetChild(i).gameObject;
            
            Button button = cardObject.GetComponent<Button>();
            if (button == null)
            {
                button = cardObject.AddComponent<Button>();
            }

            // 기존 리스너 제거 후 새로 추가
            button.onClick.RemoveAllListeners();
            
            // 클릭 핸들러 추가 (CardData는 GetClickedCardData로 가져오기)
            button.onClick.AddListener(() =>
            {
                CardData cardData = GetClickedCardData(cardObject);
                OnCardClicked(/*cardData*/);
            });
        }
    }

    // 패널 닫기
    public void ClosePanel()
    {
        HideEnhancementPanel();
        cardInventoryPanel.SetActive(false);
    }

    // 카드 클릭 시 호출되는 메서드
    public void OnCardClicked(/*CardData cardData*/)
    {
        // if (cardData == null)
        // {
        //     return;
        // }

        //selectedCardData = cardData;
        ShowEnhancementPanel();
        UpdateEnhancementUI();
        
        // TODO: 클릭한 카드 데이터를 CardEnhancementUI에 적용
        // CardEnhancementUI에 선택된 카드 데이터를 전달하여 UI 업데이트
    }

    private void ShowEnhancementPanel()
    {
        if (enhancementPanel != null)
        {
            enhancementPanel.SetActive(true);
        }
    }

    private void HideEnhancementPanel()
    {
        if (enhancementPanel != null)
        {
            enhancementPanel.SetActive(false);
        }

        selectedCardData = null;
    }

    private void UpdateEnhancementUI()
    {
        if (selectedCardData == null)
        {
            return;
        }

        bool isPreviewMode = enhancementPreviewToggle != null && enhancementPreviewToggle.isOn;

        if (cardNameText != null)
        {
            cardNameText.text = selectedCardData.Name;
        }

        if (cardDescriptionText != null)
        {
            if (isPreviewMode)
            {
                cardDescriptionText.text = GetEnhancedDescription(selectedCardData);
            }
            else
            {
                cardDescriptionText.text = selectedCardData.Description;
            }
        }
    }

    private void OnEnhancementPreviewToggleChanged(bool isOn)
    {
        UpdateEnhancementUI();
    }

    // 배경 클릭 시 강화 패널 닫기
    public void OnPointerClick(PointerEventData eventData)
    {
        // 강화 패널이 열려있지 않으면 무시
        if (enhancementPanel == null || !enhancementPanel.activeSelf)
        {
            return;
        }

        // 클릭된 UI 요소 확인
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // 카드 그리드를 클릭한 경우는 무시
        foreach (var result in results)
        {
            if (result.gameObject == cardGridParent || 
                (cardGridParent != null && result.gameObject.transform.IsChildOf(cardGridParent.transform)))
            {
                return;
            }
        }

        // 강화 패널 내부를 클릭한 경우
        bool isClickedOnEnhancementPanel = false;
        bool isClickedOnToggle = false;

        foreach (var result in results)
        {
            if (result.gameObject == enhancementPanel ||
                (enhancementPanel != null && result.gameObject.transform.IsChildOf(enhancementPanel.transform)))
            {
                isClickedOnEnhancementPanel = true;

                // 토글을 클릭한 경우인지 확인
                if (enhancementPreviewToggle != null &&
                    (result.gameObject == enhancementPreviewToggle.gameObject ||
                     result.gameObject.transform.IsChildOf(enhancementPreviewToggle.transform)))
                {
                    isClickedOnToggle = true;
                    break;
                }
            }
        }

        // 강화 패널 내부를 클릭했지만 토글이 아닌 경우, 또는 배경을 클릭한 경우 창 닫기
        if (!isClickedOnEnhancementPanel || (isClickedOnEnhancementPanel && !isClickedOnToggle))
        {
            HideEnhancementPanel();
        }
    }

    // TODO: 클릭한 카드의 정보를 받아오기
    private CardData GetClickedCardData(GameObject clickedCard)
    {
        if (clickedCard == null)
        {
            return null;
        }

        // TODO: CardUI 스크립트 생성 시 변경 예정
        // 임시: DataCenter에서 첫 번째 카드 데이터 반환 (테스트용)
        if (DataCenter.Instance != null && DataCenter.Instance.cardDatas != null && DataCenter.Instance.cardDatas.Count > 0)
        {
            return DataCenter.Instance.cardDatas[0];
        }

        return null;
    }

    // TODO: 강화했을 때 효과로 설명 바꾸기
    private string GetEnhancedDescription(CardData cardData)
    {
        // 카드 강화 시 변경될 설명을 반환하는 로직 구현
        // 예: 강화 레벨에 따라 Description을 수정하거나 강화 효과를 추가
        return cardData.Description;
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

        if (enhancementPreviewToggle != null)
        {
            enhancementPreviewToggle.onValueChanged.RemoveListener(OnEnhancementPreviewToggleChanged);
        }
    }
}
