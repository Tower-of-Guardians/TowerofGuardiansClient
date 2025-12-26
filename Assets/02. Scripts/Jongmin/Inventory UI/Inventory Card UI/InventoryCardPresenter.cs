public class InventoryCardPresenter : CardPresenter
{    
    protected readonly IInventoryCardView m_view;
    protected readonly new CardData m_card_data;
    protected readonly ICardBehavior m_behavior;
    private readonly ICardSelectionRequester m_selection_requester;
    private readonly ICardSelectionController m_selection_controller;

    public new CardData CardData => m_card_data;

    private bool m_is_selected;

    public InventoryCardPresenter(IInventoryCardView view,
                                  CardData card_data,
                                  ICardBehavior behavior,
                                  ICardSelectionRequester selection_requester,
                                  ICardSelectionController selection_controller)
    {
        m_view = view;
        m_card_data = card_data;
        m_behavior = behavior;
        m_selection_requester = selection_requester;
        m_selection_controller = selection_controller;

        m_view.Inject(this);
        m_view.InitUI(card_data);
    }

    public virtual void OnClick()
        => m_behavior?.OnClick(this);

    public virtual void OnPointerEnter()
        => m_behavior?.OnPointerEnter(this);

    public virtual void OnPointerExit()
        => m_behavior?.OnPointerExit(this);

    public void ToggleSelectRequest()
    {
        if(m_is_selected)
            m_selection_requester?.RequestDeselect(this);
        else
            m_selection_requester?.RequestSelect(this);
    }

    public void SetSelected(bool is_selected)
    {
        m_is_selected = is_selected;
        m_view.ShowHighlight(m_is_selected);
    }

    public void SelectCard()
        => m_selection_controller.Select(CardData);

    public void DeselectRequest()
        => m_selection_requester?.RequestDeselect(this);

    public override void Return()
        => m_view.Return();
}
