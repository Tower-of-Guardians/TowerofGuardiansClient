public class HandCardPresenter : CardPresenter
{
    private readonly IHandCardView m_view;

    public HandCardPresenter(IHandCardView view,
                             BattleCardData card_data)
    {
        m_view = view;
        m_card_data = card_data;

        m_view.InitUI(card_data.data);
    }

    public override void Return()
        => m_view.Return();
}
