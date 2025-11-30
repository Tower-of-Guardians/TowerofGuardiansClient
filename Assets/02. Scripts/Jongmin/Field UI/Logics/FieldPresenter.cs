using System;

public abstract class FieldPresenter : IDisposable
{
    protected readonly IFieldView m_view;
    protected readonly FieldCardContainer m_container;
    protected readonly FieldViewController m_controller;
    protected readonly FieldCardService m_service;

    protected readonly TurnManager m_turn_manager;
    private HandPresenter m_hand_presenter;
    private readonly ThrowPresenter m_throw_presenter;

    protected bool Active { get; private set; } = true;

    public FieldPresenter(IFieldView view,
                          FieldCardContainer container,
                          IFieldCardFactory factory,
                          INotice notice,
                          FieldUIDesigner designer,
                          TurnManager turn_manager,
                          ThrowPresenter throw_presenter,
                          bool is_atk)
    {
        m_view = view;
        m_container = container;
        m_controller = new FieldViewController(view, notice);
        m_service = new FieldCardService(factory, container, turn_manager, designer, is_atk);
        m_turn_manager = turn_manager;
        m_throw_presenter = throw_presenter;

        m_throw_presenter.OnUpdatedToggleUI += ToggleActive;

        m_view.Inject(this);
    }

    public void Inject(HandPresenter hand_presenter)
        => m_hand_presenter = hand_presenter;

    public abstract bool InstantiateCard(CardData card_data);

    public void ToggleManual(bool active)
    {
        if(!Active)
            return;

        if(active && m_service.CanAdd)
            m_controller.ToggleManual(true);
        else if(!active)
            m_controller.ToggleManual(false);
    }

    public void Return()
        => m_service.RemoveAll();

    public CardData[] GetCardDatas()
        => m_container.GetDatas();

    public CardData GetCardData(IFieldCardView card_view)
        => m_container.GetData(card_view);

    public void OnDroped(IHandCardView card_view)
    {
        if(!Active)
            return;

        if(!m_turn_manager.CanAction())
        {
            m_controller.Notify("<color=red>더 이상 행동할 수 없습니다.</color>");
            return;
        }

        var card_data = m_hand_presenter.GetCardData(card_view);
        if(InstantiateCard(card_data))
        {
            m_hand_presenter.RemoveCard(card_view);
            m_turn_manager.UpdateActionCount(1);
        }
    }

    public void Dispose()
        => m_throw_presenter.OnUpdatedToggleUI -= ToggleActive;

    protected void ToggleActive(bool active)
        => Active = !active;
}
