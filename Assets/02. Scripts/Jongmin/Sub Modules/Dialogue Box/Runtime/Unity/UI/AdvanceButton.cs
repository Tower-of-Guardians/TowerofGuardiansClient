using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AdvanceButton : MonoBehaviour
{
    [Header("Button Animator")]
    [SerializeField] private Animator m_animator;

    [SerializeField] private Button m_advance_button;
    private bool m_is_hide;

    public bool Hiding => m_is_hide;

    public void Bind(UnityAction advance_action)
    {
        if(m_advance_button)
        {
            m_advance_button.onClick.RemoveAllListeners();
            m_advance_button.onClick.AddListener(advance_action);
        }
    }

    public void Hightlight()
        => m_animator.SetBool("Highlight", true);

    public void Normal()
        => m_animator.SetBool("Highlight", false);

    public void Show()
    {
        m_is_hide = false;
        m_animator.SetBool("Hide", false);
    }

    public void Hide()
    {
        m_is_hide = true;
        m_animator.SetBool("Hide", true);
    }
}
