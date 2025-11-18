public class HandCardPresenter
{
    private readonly IHandCardView m_view;
    private readonly CardData m_card_data;

    public CardData CardData => m_card_data;

    public HandCardPresenter(IHandCardView view,
                             CardData card_data)
    {
        m_view = view;
        m_card_data = card_data;

        m_view.Inject(this);
        m_view.InitUI(card_data);
    }

    public void Return()
        => m_view.Return();
}
