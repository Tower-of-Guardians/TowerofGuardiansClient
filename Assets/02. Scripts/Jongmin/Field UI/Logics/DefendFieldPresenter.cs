using System.Collections.Generic;

public class DefendFieldPresenter : FieldPresenter
{
    public DefendFieldPresenter(IFieldView view,
                                FieldCardContainer container,
                                IFieldCardFactory factory,
                                FieldCardLayoutController layout_controller,
                                INotice notice,
                                FieldUIDesigner designer,
                                TurnManager turn_manager,
                                ThrowPresenter throw_presenter) : base(view, container, factory, layout_controller, notice, designer, turn_manager, throw_presenter, false) {}

    public override bool InstantiateCard(BattleCardData card_data)
    {
        if(!m_service.CanAdd)
        {
            m_controller.Notify("<color=red>방어 필드가 이미 가득 차 있습니다.</color>");
            return false;
        }

        m_service.Add(card_data);
        GameData.Instance.defenseField.Add(card_data.data);
        return true;
    }

    public override void RemoveAll()
    {
        base.RemoveAll();

        var defend_field_card_list = new List<CardData>(GameData.Instance.defenseField);
        foreach (var card_data in defend_field_card_list)
        {
            if (card_data != null)
                GameData.Instance.UseCard(card_data.id);
        }
        
        GameData.Instance.attackField.Clear();
    }
}