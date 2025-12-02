public class TemporaryCardPresenter : CardPresenter
{
    private readonly ICardView m_view;

    public TemporaryCardPresenter(ICardView view,
                                  BattleCardData card_data)
    {
        m_view = view;
        m_card_data = card_data;

        m_view.InitUI(m_card_data.data);
    }

    public override void Return()
        => m_view.Return();
}
