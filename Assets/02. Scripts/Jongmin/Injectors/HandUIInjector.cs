using UnityEngine;

public class HandUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("핸드 UI 디자이너")]
    [SerializeField] private HandUIDesigner m_hand_ui_designer;

    [Header("핸드 뷰")]
    [SerializeField] private HandView m_hand_view;

    [Header("핸드 카드 팩토리")]
    [SerializeField] private HandCardFactory m_hand_card_factory;

    [Header("핸드 카드 이벤트 컨트롤러")]
    [SerializeField] private HandCardEventController m_hand_card_event_controller;

    [Header("핸드 카드 레이아웃 컨트롤러")]
    [SerializeField] private HandCardLayoutController m_hand_card_layout_controller;

    [Header("턴 매니저")]
    [SerializeField] private TurnManager m_turn_manager;

    [Header("카드 정보 뷰")]
    [SerializeField] private CardInfoUI m_card_info_ui;

    public void Inject()
    {
        InjectHand();
        InjectTurnManager();
    }

    private void InjectHand()
    {
        DIContainer.Register<IHandView>(m_hand_view);

        var hand_card_container = new HandCardContainer();

        var hand_presenter = new HandPresenter(m_hand_view,
                                               hand_card_container,
                                               m_hand_card_factory,
                                               m_hand_card_layout_controller,
                                               DIContainer.Resolve<AttackFieldPresenter>(),
                                               DIContainer.Resolve<DefendFieldPresenter>(),
                                               DIContainer.Resolve<ThrowPresenter>(),
                                               m_turn_manager);
        DIContainer.Register<HandPresenter>(hand_presenter);

        m_hand_view.Inject(m_hand_ui_designer,
                           hand_card_container,
                           m_hand_card_factory,
                           m_hand_card_layout_controller,
                           m_hand_card_event_controller,
                           m_card_info_ui,
                           m_turn_manager);
    }

    private void InjectTurnManager()
        => m_turn_manager.Inject(DIContainer.Resolve<HandPresenter>());
}
