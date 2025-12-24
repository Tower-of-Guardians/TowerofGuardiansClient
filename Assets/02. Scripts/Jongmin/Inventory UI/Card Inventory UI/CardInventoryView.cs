using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CardInventoryView : MonoBehaviour, ICardInventoryView
{
    private Animator m_animator;

    private void Awake()
        => m_animator = GetComponent<Animator>();

    public virtual void Inject(CardInventoryPresenter presenter) {}

    public void OpenUI()
        => ToggleUI(true);

    public void CloseUI()
        => ToggleUI(false);

    private void ToggleUI(bool active)
        => m_animator.SetBool("Open", active);
}
