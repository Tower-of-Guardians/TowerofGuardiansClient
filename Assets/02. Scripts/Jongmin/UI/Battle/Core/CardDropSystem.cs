using System;
using Jongmin;
using UnityEngine;

public class CardDropSystem
{
    private readonly ICardDropTarget<HandDomain> _handDropTarget;
    private readonly IATKCardDropTarget _atkFieldDropTarget;
    private readonly IDEFCardDropTarget _defFieldDropTarget;
    private readonly ICardDropTarget<DiscardDomain> _discardDropTarget;

    private readonly TurnManager _turnManager;
    private readonly INotifierUI _notifier;

    public CardDropSystem(ICardDropTarget<HandDomain> handDropTarget,
                          IATKCardDropTarget atkFieldDropTarget,
                          IDEFCardDropTarget defFieldDropTarget,
                          ICardDropTarget<DiscardDomain> discardDropTarget,
                          TurnManager turnManager,
                          INotifierUI notifier)
    {
        _handDropTarget = handDropTarget;
        _atkFieldDropTarget = atkFieldDropTarget;
        _defFieldDropTarget = defFieldDropTarget;
        _discardDropTarget = discardDropTarget;
        _turnManager = turnManager;
        _notifier = notifier;
    }

    /// <summary>
    /// 해당 필드 카드를 [공격/방어 필드]에서 [핸드 필드]로 내립니다.
    /// </summary>
    public void OnDroppedFieldToHand(Card card)
    {
        // bool isAtkFieldCard = _atkFieldDropTarget.IsExist(cardUI);
        // var sourceFieldDropTarget = isAtkFieldCard ? (ICardDropTarget<IFieldCardUI>)_atkFieldDropTarget
        //                                            : _defFieldDropTarget;
        //
        // if(!sourceFieldDropTarget.TryGetBattleCardData(cardUI, out BattleCardData battleCardData))
        // {
        //     return;
        // }
        //
        // if (isAtkFieldCard)
        // {
        //     GameData.Instance.attackField.Remove(battleCardData.data);
        // }
        // else
        // {
        //     GameData.Instance.defenseField.Remove(battleCardData.data);
        // }
        //
        // GameData.Instance.FieldToHandMove(battleCardData);
        //
        // if(isAtkFieldCard)
        // {
        //     _atkFieldDropTarget.RemoveCard(cardUI);
        // }
        // else
        // {
        //     _defFieldDropTarget.RemoveCard(cardUI);
        // }
        //
        // _handDropTarget.CreateCard(battleCardData);        
    }

    /// <summary>
    /// 해당 교체 카드를 [교체 필드]에서 [핸드 필드]로 내립니다. 
    /// </summary>
    public void OnDroppedDiscardToHand(Card card)
    {
        GameData.Instance.FieldToHandMove(card.BattleCardData);
        _handDropTarget.CreateCard(card.BattleCardData);
        _discardDropTarget.RemoveCard(card);
    }

    /// <summary>
    /// 해당 핸드 카드를 [핸드 필드]에서 [공격/방어 필드]로 올립니다.
    /// </summary>
    public void OnDroppedHandToField(Card card, bool isAtk)
    {
        // if(isAtk && !_atkFieldDropTarget.CanInteraction)
        // {
        //     return;
        // }
        //
        // if(!isAtk && !_defFieldDropTarget.CanInteraction)
        // {
        //     return;
        // }
        //
        // if(!_turnManager.CanAction)
        // {
        //     _notifier.Notify("<color=red>더 이상 행동할 수 없습니다.</color>");
        //     return;
        // }
        //
        // if(!_handDropTarget.TryGetBattleCardData(cardUI, out BattleCardData battleCardData))
        // {
        //     return;
        // }
        //
        // if(isAtk)
        // {
        //     _atkFieldDropTarget.CreateCard(battleCardData);
        // }
        // else
        // {
        //     _defFieldDropTarget.CreateCard(battleCardData);
        // }
        //
        // _handDropTarget.RemoveCard(cardUI);
        // GameData.Instance.HandToFieldMove(battleCardData);
    }

    /// <summary>
    /// 해당 핸드 카드를 [핸드 필드]에서 [교체 필드]로 올립니다.
    /// </summary>
    public void OnDroppedHandToDiscard(Card card)
    {
        if(!_turnManager.CanThrow)
        {
            _notifier.Notify("<color=red>더 이상 버릴 수 없습니다.</color>");
            return;
        }
        
        _discardDropTarget.CreateCard(card.BattleCardData);
        GameData.Instance.HandToFieldMove(card.BattleCardData);
        _handDropTarget.RemoveCard(card);
    }
}
