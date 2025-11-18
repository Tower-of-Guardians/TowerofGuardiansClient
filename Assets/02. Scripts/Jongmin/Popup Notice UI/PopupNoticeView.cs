using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PopupNoticeView : MonoBehaviour, IPopupNoticeView
{
    [Header("UI 관련 컴포넌트")]
    [Header("캔버스 그룹")]
    [SerializeField] private CanvasGroup m_canvas_group;

    [Header("안내 텍스트")]
    [SerializeField] private TMP_Text m_notice_label;

    private Animator m_animator;

    private void Awake()
        => m_animator = GetComponent<Animator>();

    private void OnEnable()
        => m_animator.SetTrigger("Popup");

    public void OpenUI(string notice_text)
        => m_notice_label.text = notice_text;

    public void CloseUI()
        => ObjectPoolManager.Instance.Return(gameObject);
}