using System;

public class ThrowPresenter : IDisposable
{
    private readonly IThrowView m_view;
    private readonly IThrowCardFactory m_factory;
    private readonly ThrowCardContainer m_container;
    private readonly ThrowViewController m_controller;
    private readonly ThrowCardService m_service;
    private readonly TurnManager m_turn_manager;
    
    private HandPresenter m_hand_presenter;
    private IThrowCardView m_hover_card;
    public event Action<bool> OnUpdatedToggleUI;

    public IThrowCardView HoverCard
    {
        get => m_hover_card;
        set => m_hover_card = value;
    }

    public ThrowPresenter(IThrowView view,
                          IThrowCardFactory factory,
                          INotice notice,
                          ThrowCardContainer container,
                          TurnManager turn_manager)
    {
        m_view = view;
        m_factory = factory;
        m_container = container;
        m_controller = new ThrowViewController(view, notice);
        m_service = new ThrowCardService(view, factory, container, turn_manager);
        m_turn_manager = turn_manager;

        m_turn_manager.OnUpdatedThrowActionState += m_controller.UpdateThrowState;
        m_turn_manager.OnUpdatedThrowCount += m_controller.UpdateThrowCount;

        m_view.Inject(this);
        m_turn_manager.Initialize();
    }

    public void Inject(HandPresenter hand_presenter)
        => m_hand_presenter = hand_presenter;

    public void OnClickedOpenUI()
    {
        m_controller.OpenUI();
        OnUpdatedToggleUI?.Invoke(true);
    }

    public void OnClickedCloseUI()
    {
        m_controller.CloseUI();

        OnUpdatedToggleUI?.Invoke(false);
    }

    public void OnClickedThrowCards()
    {
        m_controller.ThrowUI();
        m_turn_manager.UpdateThrowAction(false);

        OnUpdatedToggleUI?.Invoke(false);
    }

    public void RemoveCard(IThrowCardView card_view)
        => m_service.Remove(card_view);

    public void ToggleManual(bool active)
    {
        if(!m_controller.Active)
            return;

        if(m_turn_manager.CanThrow() || !active)
            m_controller.ToggleManual(active);
    }

    public IThrowCardView GetCardView(BattleCardData battle_card_data)
        => m_container.GetCardView(battle_card_data);

    public IThrowCardView[] GetCardViews()
        => m_container.GetCardViews();

    public BattleCardData[] GetCardDatas()
        => m_container.GetDatas();

    public BattleCardData GetCardData(IThrowCardView card_view)
        => m_container.GetData(card_view);

    public void OnDroped(IHandCardView card_view)
    {
        if(!m_turn_manager.CanThrow())
        {
            m_controller.Notify("<color=red>더 이상 버릴 수 없습니다.</color>");
            return;
        }

        m_service.Add(m_hand_presenter.GetCardData(card_view));
        

        var card_data = m_hand_presenter.GetCardData(card_view);
        GameData.Instance.HandToFieldMove(card_data);
    
        m_hand_presenter.RemoveCard(card_view);
    }

    public void Dispose()
    {
        m_turn_manager.OnUpdatedThrowActionState -= m_controller.UpdateThrowState;
        m_turn_manager.OnUpdatedThrowCount -= m_controller.UpdateThrowCount;
    }
}