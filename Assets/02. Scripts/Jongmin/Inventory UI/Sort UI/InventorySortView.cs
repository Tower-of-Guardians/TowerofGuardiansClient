using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySortView : MonoBehaviour, IInventorySortView
{
    [Header("UI 관련 컴포넌트")]
    [Header("좌측 정렬 기준 변경 버튼")]
    [SerializeField] private Button m_left_button;

    [Header("우측 정렬 기준 변경 버튼")]
    [SerializeField] private Button m_right_button;

    [Header("오름차순/내림차순 버튼")]
    [SerializeField] private Button m_criterion_button;

    [Header("정렬 텍스트")]
    [SerializeField] private TMP_Text m_sort_label;

    private InventorySortPresenter m_presenter;

    public void Inject(InventorySortPresenter presenter)
    {
        m_presenter = presenter;

        m_left_button.onClick.AddListener(m_presenter.OnClickedLeftButton);
        m_right_button.onClick.AddListener(m_presenter.OnClickedRightButton);
        m_criterion_button.onClick.AddListener(m_presenter.OnClickedCriterionButton);
    } 

    public void UpdateSortLabel(string sort_text)
        => m_sort_label.text = sort_text;

    public void UpdateSortButton(bool is_ascending)
    {
        var button_rt = m_criterion_button.transform as RectTransform;
        if (button_rt != null)
        {
            var new_rotation = is_ascending ? 0f : 180f;
            button_rt.localRotation = Quaternion.Euler(0f, 0f, new_rotation);
        }
    }
}
