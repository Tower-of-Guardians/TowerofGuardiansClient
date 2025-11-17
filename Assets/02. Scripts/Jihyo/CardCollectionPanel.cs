using UnityEngine;
using UnityEngine.UI;

public class CardCollectionPanel : MonoBehaviour
{
    [Header("Left UI Tabs")]
    public Button cardTabButton;
    public Button otherTabButton;

    [Header("Right UI Sort Buttons")]
    public Button sortByAcquisitionButton;
    public Button sortByGradeButton;
    public Button sortByStrengthButton;

    [Header("Navigation")]
    public Button nextButton;

    private bool isAcquisitionAscending = true;
    private bool isGradeAscending = false;
    private bool isStrengthAscending = true;

    void Start()
    {
        InitializeButtons();
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
        gameObject.SetActive(true);
    }

    // 패널 닫기
    public void ClosePanel()
    {
        gameObject.SetActive(false);
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
