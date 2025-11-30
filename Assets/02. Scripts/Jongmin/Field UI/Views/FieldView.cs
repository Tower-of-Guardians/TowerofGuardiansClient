using UnityEngine;

public class FieldView : MonoBehaviour, IFieldView
{
    [Header("UI 관련 컴포넌트")]
    [Header("프리뷰 오브젝트")]
    [SerializeField] private GameObject m_preview_object;

    private FieldPresenter m_presenter;

    private void OnDestroy()
        => m_presenter?.Dispose();

    public void Inject(FieldCardEventController event_controller,
                       FieldCardFactory factory)
    {
        event_controller.Inject(this, m_presenter);
        factory.Inject(event_controller);
    }

    public void Inject(FieldPresenter presenter)
        => m_presenter = presenter;

    public void ToggleManual(bool active)
        => m_preview_object.SetActive(active);
}
