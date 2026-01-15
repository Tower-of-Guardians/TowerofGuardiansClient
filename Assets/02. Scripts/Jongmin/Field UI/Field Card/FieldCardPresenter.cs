public class FieldCardPresenter : CardPresenter
{
    private readonly IFieldCardView m_view;

    public FieldCardPresenter(IFieldCardView view,
                              BattleCardData card_data,
                              bool is_atk)
    {
        m_view = view;
        m_card_data = card_data;

        m_view.InitUI(m_card_data.data, is_atk);
    }

    public override void Return()
        => m_view.Return();

    public void ToggleLock()
        => m_view.ToggleLock();
}
