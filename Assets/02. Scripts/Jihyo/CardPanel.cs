using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ShowDrawingDeck() : Drawing Dummy Button과 연결
/// ShowThrowingDeck() : Throwing Dummy Button과 연결
/// </summary>

public class CardPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private GameObject scrollViewDrawing;
    [SerializeField] private GameObject scrollViewThrowing;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(Hide);
        }
    }

    public void ShowDrawingDeck()
    {
        Show("뽑을 카드 더미", scrollViewDrawing, scrollViewThrowing);
    }

    public void ShowThrowingDeck()
    {
        Show("버린 카드 더미", scrollViewThrowing, scrollViewDrawing);
    }

    private void Show(string title, GameObject activeScrollView, GameObject inactiveScrollView)
    {
        // 패널 표시
        gameObject.SetActive(true);

        // 제목 설정
        if (titleText != null)
        {
            titleText.text = title;
        }

        // 활성화할 Scroll View 표시
        if (activeScrollView != null)
        {
            activeScrollView.SetActive(true);
        }

        // 비활성화할 Scroll View 숨김
        if (inactiveScrollView != null)
        {
            inactiveScrollView.SetActive(false);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(Hide);
        }
    }
}
