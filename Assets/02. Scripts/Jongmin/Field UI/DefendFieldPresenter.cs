public class DefendFieldPresenter : FieldPresenter
{
    public DefendFieldPresenter(IFieldView view,
                                FieldUIDesigner designer,
                                TurnManager turn_manager,
                                ThrowPresenter throw_presenter) : base(view, designer, turn_manager, throw_presenter) {}

    public override bool InstantiateCard(CardData card_data)
    {
        if(m_card_list.Count >= m_designer.DEFLimit)
        {
            m_view.PrintNotice("<color=red>방어 필드가 이미 가득 차 있습니다.</color>");
            return false;
        }

        var card_view = m_view.InstantiateCardView();
        m_card_list.Add(card_view);

        var card_presenter = new FieldCardPresenter(card_view,
                                                    card_data);
        m_card_dict.TryAdd(card_view, card_presenter);

        return true;
    }

    public override void ToggleManual(bool active)
    {
        if(!m_is_active)
            return;
            
        if(active && m_card_list.Count < m_designer.DEFLimit)
            m_view.ToggleManual(true);
        else if(!active)
            m_view.ToggleManual(false);
    }
}