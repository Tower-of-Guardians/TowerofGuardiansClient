using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CardInfoUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Card Info Panel")]
    [SerializeField] private GameObject cardInfoPanel;
    
    [Header("Card UI Components")]
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text cardDescriptionText;
    [SerializeField] private Toggle enhancementPreviewToggle;
    [SerializeField] private GameObject cardGridParent;

    private CardData currentCardData;

    void Start()
    {
        InitializeCardInfoUI();
    }

    private void InitializeCardInfoUI()
    {
        if (cardInfoPanel != null)
        {
            cardInfoPanel.SetActive(false);
        }

        if (enhancementPreviewToggle != null)
        {
            enhancementPreviewToggle.onValueChanged.AddListener(OnEnhancementPreviewToggleChanged);
            enhancementPreviewToggle.isOn = false;
        }
    }

    /// 카드 정보를 표시합니다.
    public void ShowCardInfo(CardData cardData)
    {
        if (cardData == null)
        {
            Debug.LogWarning("CardInfoUI: cardData가 null입니다.");
            return;
        }

        currentCardData = cardData;
        ShowPanel();
        UpdateCardInfoUI();
    }

    /// 카드 정보 패널을 표시합니다.
    public void ShowPanel()
    {
        if (cardInfoPanel != null)
        {
            cardInfoPanel.SetActive(true);
        }
    }

    /// 카드 정보 패널을 숨깁니다.
    public void HidePanel()
    {
        if (cardInfoPanel != null)
        {
            cardInfoPanel.SetActive(false);
        }

        currentCardData = null;
    }

    /// 카드 정보 UI를 업데이트합니다.
    private void UpdateCardInfoUI()
    {
        if (currentCardData == null)
        {
            return;
        }

        if (cardImage != null)
        {
            cardImage.sprite = currentCardData.iconimage;
        }

        if (cardNameText != null)
        {
            cardNameText.text = currentCardData.itemName;
        }

        bool isPreviewMode = enhancementPreviewToggle != null && enhancementPreviewToggle.isOn;
        if (cardDescriptionText != null)
        {
            if (isPreviewMode)
            {
                cardDescriptionText.text = GetEnhancedDescription(currentCardData);
            }
            else
            {
                cardDescriptionText.text = currentCardData.effectDescription;
            }
        }

        // TODO: 강화가 되어있으면 토글 체크
    }

    /// 강화 미리보기 토글 변경 시 호출됩니다.
    private void OnEnhancementPreviewToggleChanged(bool isOn)
    {
        // TODO: 토글을 체크하면 설명이 강화된 효과로 바뀜
        UpdateCardInfoUI();
    }

    /// 배경 클릭 시 카드 정보 패널을 닫습니다.
    public void OnPointerClick(PointerEventData eventData)
    {
        // 카드 정보 패널이 열려있지 않으면 무시
        if (cardInfoPanel == null || !cardInfoPanel.activeSelf)
        {
            return;
        }

        // 클릭된 UI 요소 확인
        System.Collections.Generic.List<RaycastResult> results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // cardGridParent 영역을 클릭한 경우 패널을 닫지 않음
        foreach (var result in results)
        {
            if (cardGridParent != null && 
                (result.gameObject == cardGridParent || 
                 result.gameObject.transform.IsChildOf(cardGridParent.transform)))
            {
                return;
            }
        }

        bool isClickedOnCardInfoPanel = false;
        bool isClickedOnToggle = false;

        foreach (var result in results)
        {
            if (cardInfoPanel != null &&
                (result.gameObject == cardInfoPanel ||
                 result.gameObject.transform.IsChildOf(cardInfoPanel.transform)))
            {
                isClickedOnCardInfoPanel = true;

                if (enhancementPreviewToggle != null &&
                    (result.gameObject == enhancementPreviewToggle.gameObject ||
                     result.gameObject.transform.IsChildOf(enhancementPreviewToggle.transform)))
                {
                    isClickedOnToggle = true;
                    break;
                }
            }
        }

        // 카드 정보 패널 외부를 클릭했거나, 토글이 아닌 패널 내부를 클릭한 경우 닫기
        if (!isClickedOnCardInfoPanel || (isClickedOnCardInfoPanel && !isClickedOnToggle))
        {
            HidePanel();
        }
    }

    /// 강화된 카드 설명을 반환합니다.
    private string GetEnhancedDescription(CardData cardData)
    {
        if (cardData == null)
        {
            return string.Empty;
        }

        // 카드 강화 시 변경될 설명을 반환하는 로직 구현
        // 예: 강화 레벨에 따라 Description을 수정하거나 강화 효과를 추가
        // TODO: 현재는 기본 설명 반환 (추후 강화 로직 구현 시 수정 필요)
        return cardData.effectDescription;
    }

    void OnDestroy()
    {
        if (enhancementPreviewToggle != null)
        {
            enhancementPreviewToggle.onValueChanged.RemoveListener(OnEnhancementPreviewToggleChanged);
        }
    }
}

