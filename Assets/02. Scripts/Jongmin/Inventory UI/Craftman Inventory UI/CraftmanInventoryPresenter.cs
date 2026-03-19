public class CraftmanInventoryPresenter : DeckInvenPresenter, ICardSelectionController
{
    private CraftmanDialogueBubblePresenter m_dialogue_bubble_presenter;
    private ReinforcementPresenter m_reinforcement_presenter;

    public CraftmanInventoryPresenter(IDeckInvenUI view, 
                                      ICardFactory<IDeckInvenCardUI> factory,
                                      CardContainer<IDeckInvenCardUI, DeckInvenCardPresenter> cardContainer,
                                      ICardBehavior behavior,
                                      CraftmanDialogueBubblePresenter dialogue_bubble_presenter,
                                      ReinforcementPresenter reinforcement_presenter) 
        : base(view, factory, cardContainer, behavior)
    {
        m_dialogue_bubble_presenter = dialogue_bubble_presenter;
        m_reinforcement_presenter = reinforcement_presenter;

        _deckInvenUI.Construct(this);
    }

    public override void OpenUI()
    {
        m_dialogue_bubble_presenter.OpenUI(this);
        UpdateDefaultBubble();

        base.OpenUI();
    }

    public override void CloseUI()
    {
        m_dialogue_bubble_presenter.CloseUI(this);
        base.CloseUI();
    }

    public void FadeUpUI()
        => OpenUI();

    public void FadeDownUI()
        => base.CloseUI();

    public void Select(CardData card_data)
    {
        m_dialogue_bubble_presenter.UpdateSelectedBubble(card_data);
        m_reinforcement_presenter.OpenUI(card_data);
        FadeDownUI();
    }

    public void UpdateDefaultBubble()
        => m_dialogue_bubble_presenter.UpdateDefaultBubble();

    public void UpdateEnforcedBubble()
        => m_dialogue_bubble_presenter.UpdateEnforcedBubble();

    protected override ICardSelectionController CreateSelectionController()
        => this;
}
