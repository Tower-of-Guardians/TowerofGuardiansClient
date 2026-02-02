using System;

public class HandPresenter : IDisposable
{
    private readonly FieldPresenter m_attack_field_presenter;
    private readonly FieldPresenter m_defend_field_presenter;
    private readonly ThrowPresenter m_throw_presenter;

    private readonly HandCardContainer m_container;
    private readonly HandCardService m_service;
    private readonly HandCardViewController m_view_controller;
    private readonly TurnManager m_turn_manager;

    private IHandCardView m_hover_card;

    public IHandCardView HoverCard
    {
        get => m_hover_card;
        set => m_hover_card = value;
    }

    public HandPresenter(IHandView view,
                         HandCardContainer container,
                         IHandCardFactory factory,
                         HandCardLayoutController layout_controller,
                         FieldPresenter attack_field_presenter,
                         FieldPresenter defend_field_presenter,
                         ThrowPresenter throw_presenter,
                         TurnManager turn_manager)
    {
        
        m_container = container;
        m_service = new HandCardService(factory, container, layout_controller);
        m_view_controller = new HandCardViewController(view);

        m_attack_field_presenter = attack_field_presenter;
        m_defend_field_presenter = defend_field_presenter;
        m_throw_presenter = throw_presenter;
        m_turn_manager = turn_manager;

        m_turn_manager.EndCurrentTurn += ClearAllCards;

        view.Inject(this);
        m_attack_field_presenter.Inject(this);
        m_defend_field_presenter.Inject(this);
        throw_presenter.Inject(this);
    }

    public void InstantiateCard(BattleCardData card_data)
        => m_service.Add(card_data);

    public void RemoveCard(IHandCardView card_view)
        => m_service.Remove(card_view);

    public void ClearAllCards()
    {
        var card_data_list = GetCardDatas();
        if (card_data_list != null && card_data_list.Length > 0)
        {
            foreach (var card_data in card_data_list)
            {
                if (card_data != null && card_data.data != null)
                    GameData.Instance.UseCard(card_data.data.id);
            }
        }

        m_service.RemoveAll();
        GameData.Instance.handDeck.Clear();
    }

    public void OpenUI()
        => m_view_controller.OpenUI();

    public void CloseUI()
        => m_view_controller.CloseUI();

    public void ToggleFieldPreview(bool active)
    {
        m_attack_field_presenter.ToggleManual(active);
        m_defend_field_presenter.ToggleManual(active);
        m_throw_presenter.ToggleManual(active);
    }

    public BattleCardData GetCardData(IHandCardView card_view)
        => m_container.GetData(card_view);

    public BattleCardData[] GetCardDatas()
        => m_container.GetDatas();

    public void OnDroped(IThrowCardView card_view)
    {
        var card_data = m_throw_presenter.GetCardData(card_view);
        GameData.Instance.FieldToHandMove(card_data);

        m_throw_presenter.RemoveCard(card_view);
        InstantiateCard(card_data);
    }

    public void OnDroped(IFieldCardView card_view)
    {
        var card_data = m_attack_field_presenter.GetCardData(card_view);
        card_data ??= m_defend_field_presenter.GetCardData(card_view);
        
        GameData.Instance.FieldToHandMove(card_data);

        if(m_attack_field_presenter.IsExist(card_view))
            m_attack_field_presenter.Remove(card_view);
        else
            m_defend_field_presenter.Remove(card_view);

        InstantiateCard(card_data);
    }

    public void Dispose()
    {
        if(m_turn_manager != null)
            m_turn_manager.EndCurrentTurn -= ClearAllCards;
    }
}