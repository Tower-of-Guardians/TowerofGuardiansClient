using UnityEngine;

public class FieldUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("필드 디자이너")]
    [SerializeField] private FieldUIDesigner m_designer;

    [Header("알리미")]
    [SerializeField] private Notice m_notice;

    [Space(30f), Header("공격 필드 관련")]
    [Header("공격 필드 뷰")]
    [SerializeField] private FieldView m_attack_field_view;

    [Header("공격 필드 카드 팩토리")]
    [SerializeField] private FieldCardFactory m_attack_field_factory;

    [Header("공격 필드 이벤트 관리자")]
    [SerializeField] private FieldCardEventController m_attack_event_controller;

    [Header("공격 필드 레이아웃 관리자")]
    [SerializeField] private FieldCardLayoutController m_attack_layout_controller;

    [Space(30f), Header("방어 필드 관련")]
    [Header("방어 필드 뷰")]
    [SerializeField] private FieldView m_defend_field_view;

    [Header("방어 필드 카드 팩토리")]
    [SerializeField] private FieldCardFactory m_defend_field_factory;

    [Header("방어 필드 이벤트 관리자")]
    [SerializeField] private FieldCardEventController m_defend_event_controller;

    [Header("방어 필드 레이아웃 관리자")]
    [SerializeField] private FieldCardLayoutController m_defend_layout_controller;

    public void Inject()
    {
        InjectField();
    }

    private void InjectField()
    {
        var turn_manager = DIContainer.Resolve<TurnManager>();
        var throw_presenter = DIContainer.Resolve<ThrowPresenter>();

        var atk_field_card_container = new FieldCardContainer();
        var attack_field_presenter = new AttackFieldPresenter(m_attack_field_view,
                                                              atk_field_card_container,
                                                              m_attack_field_factory,
                                                              m_attack_layout_controller,
                                                              m_notice,
                                                              m_designer,
                                                              turn_manager,
                                                              throw_presenter);
        DIContainer.Register<AttackFieldPresenter>(attack_field_presenter);

        var def_field_card_container = new FieldCardContainer();
        var defend_field_presenter = new DefendFieldPresenter(m_defend_field_view,
                                                              def_field_card_container,
                                                              m_defend_field_factory,
                                                              m_defend_layout_controller,
                                                              m_notice,
                                                              m_designer,
                                                              turn_manager,
                                                              throw_presenter);
        DIContainer.Register<DefendFieldPresenter>(defend_field_presenter);

        m_attack_field_view.Inject(m_attack_event_controller, 
                                   m_attack_field_factory,
                                   m_attack_layout_controller,
                                   atk_field_card_container,
                                   m_designer,
                                   defend_field_presenter,
                                   m_defend_event_controller);

        m_attack_layout_controller.Inject(atk_field_card_container,
                                          attack_field_presenter);

        m_defend_field_view.Inject(m_defend_event_controller,
                                   m_defend_field_factory,
                                   m_defend_layout_controller,
                                   def_field_card_container,
                                   m_designer,
                                   attack_field_presenter,
                                   m_attack_event_controller);

        m_defend_layout_controller.Inject(def_field_card_container,
                                          defend_field_presenter);
    }
}
