public class FieldCardPresenter : CardPresenter
{
    private readonly IFieldCardView m_view;

    public CardData CardData => BattleCardData.data;

    public FieldCardPresenter(IFieldCardView view,
                              BattleCardData card_data,
                              bool is_atk)
    {
        m_view = view;
        BattleCardData = card_data;

        m_view.InitUI(card_data.data, is_atk);
    }

    public void ToggleLock()
        => m_view.ToggleLock();
}
