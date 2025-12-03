using System.Collections.Generic;
using UnityEngine;

public class FieldView : MonoBehaviour, IFieldView
{
    [Header("UI 관련 컴포넌트")]
    [Header("프리뷰 오브젝트")]
    [SerializeField] private GameObject m_preview_object;

    private FieldPresenter m_presenter;
    private FieldCardLayoutController m_layout_controller;

    private void OnDestroy()
        => m_presenter?.Dispose();

    public void Inject(FieldCardEventController event_controller,
                       FieldCardFactory factory,
                       FieldCardLayoutController layout_controller,
                       FieldCardContainer container,
                       FieldUIDesigner designer,
                       FieldPresenter another_presenter,
                       FieldCardEventController another_event_controller,
                       List<CardData> model)
    {
        event_controller.Inject(this, m_presenter, another_presenter, layout_controller, another_event_controller, container, designer, model);
        factory.Inject(event_controller);
        m_layout_controller = layout_controller;
    }

    public void Inject(FieldPresenter presenter)
        => m_presenter = presenter;

    public void ToggleManual(bool active)
    {
        m_layout_controller.UpdateLayout(active);
        m_preview_object.SetActive(active);
        m_preview_object.transform.SetAsFirstSibling();
    }
}
