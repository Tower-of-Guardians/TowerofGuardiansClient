public class CraftmanInventoryPresenter : CardInventoryPresenter, ICardSelectionController
{
    private CraftmanDialogueBubblePresenter m_dialogue_bubble_presenter;
    private ReinforcementPresenter m_reinforcement_presenter;

    public CraftmanInventoryPresenter(ICardInventoryView view, 
                                      CardInventoryFactory factory,
                                      ICardBehavior behavior,
                                      INotice notice,
                                      CraftmanDialogueBubblePresenter dialogue_bubble_presenter,
                                      ReinforcementPresenter reinforcement_presenter) 
        : base(view, factory, behavior, notice)
    {
        m_dialogue_bubble_presenter = dialogue_bubble_presenter;
        m_reinforcement_presenter = reinforcement_presenter;

        m_view.Inject(this);
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
