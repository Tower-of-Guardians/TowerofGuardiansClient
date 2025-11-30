public class FieldCardPresenter : CardPresenter
{
    private readonly IFieldCardView m_view;

    public FieldCardPresenter(IFieldCardView view,
                              CardData card_data)
    {
        m_view = view;
        m_card_data = card_data;

        m_view.InitUI(m_card_data);
    }

    public override void Return()
        => m_view.Return();
}
