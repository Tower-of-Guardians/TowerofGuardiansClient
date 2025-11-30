public class DefendFieldPresenter : FieldPresenter
{
    public DefendFieldPresenter(IFieldView view,
                                FieldCardContainer container,
                                IFieldCardFactory factory,
                                INotice notice,
                                FieldUIDesigner designer,
                                TurnManager turn_manager,
                                ThrowPresenter throw_presenter) : base(view, container, factory, notice, designer, turn_manager, throw_presenter, false) {}

    public override bool InstantiateCard(CardData card_data)
    {
        if(!m_service.CanAdd)
        {
            m_controller.Notify("<color=red>방어 필드가 이미 가득 차 있습니다.</color>");
            return false;
        }

        m_service.Add(card_data);
        return true;
    }
}