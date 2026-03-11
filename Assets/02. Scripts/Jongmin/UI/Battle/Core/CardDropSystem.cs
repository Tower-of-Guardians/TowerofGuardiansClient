using System;

public class CardDropSystem
{
    private readonly ICardDropTarget<IHandCardUI> _handDropTarget;
    private readonly IATKCardDropTarget _atkFieldDropTarget;
    private readonly IDEFCardDropTarget _defFieldDropTarget;
    private readonly ICardDropTarget<IThrowCardView> _discardDropTarget;

    private TurnManager _turnManager;
    private INotice _notifier;

    public CardDropSystem(ICardDropTarget<IHandCardUI> handDropTarget,
                          IATKCardDropTarget atkfieldDropTarget,
                          IDEFCardDropTarget defFieldDropTarget,
                          ICardDropTarget<IThrowCardView> discardDropTarget,
                          TurnManager turnManager,
                          INotice notifier)
    {
        _handDropTarget = handDropTarget;
        _atkFieldDropTarget = atkfieldDropTarget;
        _defFieldDropTarget = defFieldDropTarget;
        _discardDropTarget = discardDropTarget;
        _turnManager = turnManager;
        _notifier = notifier;
    }

    /// <summary>
    /// 해당 필드 카드를 [공격/방어 필드]에서 [핸드 필드]로 내립니다.
    /// </summary>
    public void OnDropedFieldToHand(IFieldCardUI cardUI)
    {
        bool isAtkFieldCard = _atkFieldDropTarget.IsExist(cardUI);
        var sourceFieldDropTarget = isAtkFieldCard ? (ICardDropTarget<IFieldCardUI>)_atkFieldDropTarget
                                                   : _defFieldDropTarget;

        if(!sourceFieldDropTarget.TryGetBattleCardData(cardUI, out BattleCardData battleCardData))
        {
            return;
        }
        
        GameData.Instance.FieldToHandMove(battleCardData);

        if(isAtkFieldCard)
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

    /// <summary>
    /// 해당 핸드 카드를 [핸드 필드]에서 [공격/방어 필드]로 올립니다.
    /// </summary>
    public void OnDropedHandToField(IHandCardUI cardUI, bool isAtk)
    {
        if(!_turnManager.CanAction())
        {
            _notifier.Notify("<color=red>더 이상 행동할 수 없습니다.</color>");
            return;
        }

        if(!_handDropTarget.TryGetBattleCardData(cardUI, out BattleCardData battleCardData))
        {
            return;
        }

        if(isAtk)
        {
            _atkFieldDropTarget.CreateCard(battleCardData);
        }
        else
        {
            _defFieldDropTarget.CreateCard(battleCardData);
        }

        GameData.Instance.HandToFieldMove(battleCardData);
        _handDropTarget.RemoveCard(cardUI);
    }
}
