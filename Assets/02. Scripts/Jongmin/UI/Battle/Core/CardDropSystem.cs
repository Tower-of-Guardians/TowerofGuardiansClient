using System;

public class CardDropSystem
{
    private readonly ICardDropTarget<IHandCardUI> _handDropTarget;
    private readonly IATKCardDropTarget _atkFieldDropTarget;
    private readonly IDEFCardDropTarget _defFieldDropTarget;
    private readonly ICardDropTarget<IThrowCardView> _discardDropTarget;

    public CardDropSystem(ICardDropTarget<IHandCardUI> handDropTarget,
                          IATKCardDropTarget atkfieldDropTarget,
                          IDEFCardDropTarget defFieldDropTarget,
                          ICardDropTarget<IThrowCardView> discardDropTarget)
    {
        _handDropTarget = handDropTarget;
        _atkFieldDropTarget = atkfieldDropTarget;
        _defFieldDropTarget = defFieldDropTarget;
        _discardDropTarget = discardDropTarget;
    }

    /// <summary>
    /// 해당 필드 카드를 [공격/방어 필드]에서 [핸드 필드]로 내립니다.
    /// </summary>
    public void OnDropedFieldToHand(IFieldCardUI cardUI)
    {
        if(!_atkFieldDropTarget.TryGetBattleCardData(cardUI, out BattleCardData battleCardData))
        {
            _defFieldDropTarget.TryGetBattleCardData(cardUI, out battleCardData);
        }
        
        GameData.Instance.FieldToHandMove(battleCardData);

        if(_atkFieldDropTarget.IsExist(cardUI))
        {
            _atkFieldDropTarget.RemoveCard(cardUI);
        }
        else
        {
            _defFieldDropTarget.RemoveCard(cardUI);
        }

        _handDropTarget.CreateCard(battleCardData);        
    }

    /// <summary>
    /// 해당 교체 카드를 [교체 필드]에서 [핸드 필드]로 내립니다. 
    /// </summary>
    public void OnDropedDiscardToHand(IThrowCardView cardUI)
    {
        if(!_discardDropTarget.TryGetBattleCardData(cardUI, out BattleCardData battleCardData))
        {
            return;
        }

        GameData.Instance.FieldToHandMove(battleCardData);

        _discardDropTarget.RemoveCard(cardUI);
        _handDropTarget.CreateCard(battleCardData);
    }
}