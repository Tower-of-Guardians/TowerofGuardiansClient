using UnityEngine;

public class ThrowUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("레이아웃 교체 뷰")]
    [SerializeField] private LayoutThrowView m_layout_throw_view;

    [Header("교체 카드 이벤트 관리자")]
    [SerializeField] private ThrowCardEventController m_event_controller;

    [Header("교체 카드 레이아웃 관리자")]
    [SerializeField] private ThrowCardLayoutController m_layout_controller;

    [Header("교체 카드 팩토리")]
    [SerializeField] private ThrowCardFactory m_throw_card_factory;

    [Header("교체 뷰 디자이너")]
    [SerializeField] private ThrowUIDesigner m_throw_ui_designer;

    [Header("알리미")]
    [SerializeField] private Notice m_notice;

    public void Inject()
    {
        InjectThrow();
    }

    private void InjectThrow()
    {
        DIContainer.Register<IThrowView>(m_layout_throw_view);

        var throw_card_container = new ThrowCardContainer();
        DIContainer.Register<ThrowCardContainer>(throw_card_container);

        var throw_presenter = new ThrowPresenter(m_layout_throw_view,
                                                 m_throw_card_factory,
                                                 m_notice,
                                                 throw_card_container,
                                                 DIContainer.Resolve<TurnManager>());
        DIContainer.Register<ThrowPresenter>(throw_presenter);

        m_layout_throw_view.Inject(throw_card_container,
                                   m_throw_ui_designer,
                                   m_layout_controller,
                                   m_event_controller,
                                   m_throw_card_factory);
    }
}