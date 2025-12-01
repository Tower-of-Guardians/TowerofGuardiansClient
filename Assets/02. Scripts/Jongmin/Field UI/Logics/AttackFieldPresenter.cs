public class AttackFieldPresenter : FieldPresenter
{
    public AttackFieldPresenter(IFieldView view,
                                FieldCardContainer container,
                                IFieldCardFactory factory,
                                FieldCardLayoutController layout_controller,
                                INotice notice,
                                FieldUIDesigner designer,
                                TurnManager turn_manager,
                                ThrowPresenter throw_presenter) : base(view, container, factory, layout_controller, notice, designer, turn_manager, throw_presenter, true) {}

    public override bool InstantiateCard(CardData card_data)
    {
        if(!m_service.CanAdd)
        {
            m_controller.Notify("<color=red>공격 필드가 이미 가득 차 있습니다.</color>");
            return false;
        }

        m_service.Add(card_data);
        return true;
    }
}
