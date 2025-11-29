public class HandCardPresenter : CardPresenter
{
    private readonly IHandCardView m_view;

    public HandCardPresenter(IHandCardView view,
                             CardData card_data)
    {
        m_view = view;
        m_card_data = card_data;

        m_view.InitUI(card_data);
    }

    public override void Return()
        => m_view.Return();
}
