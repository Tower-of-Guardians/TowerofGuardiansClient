public class HandPresenter
{
    private readonly FieldPresenter m_attack_field_presenter;
    private readonly FieldPresenter m_defend_field_presenter;
    private readonly ThrowPresenter m_throw_presenter;

    private readonly HandCardContainer m_container;
    private readonly HandCardService m_service;
    private readonly HandCardViewController m_view_controller;

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
                         ThrowPresenter throw_presenter)
    {
        
        m_container = container;
        m_service = new HandCardService(factory, container, layout_controller);
        m_view_controller = new HandCardViewController(view);

        m_attack_field_presenter = attack_field_presenter;
        m_defend_field_presenter = defend_field_presenter;
        m_throw_presenter = throw_presenter;

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
        => m_service.RemoveAll();

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
}