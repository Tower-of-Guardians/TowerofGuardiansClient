using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;
using VContainer;

public class ThrowCardToThrowEffector : CardEffector
{
    private DiscardPresenter _discardPresenter;
    private CardContainer<IDiscardCardUI, DiscardCardPresenter> _discardCardContainer;

    [Inject]
    public void Construct(DiscardPresenter discardPresenter,
                          CardContainer<IDiscardCardUI, DiscardCardPresenter> discardCardContainer)
    {
        _discardPresenter = discardPresenter;
        _discardCardContainer = discardCardContainer;

        m_temp_card_settings = new()
        {
            Duration = 0.5f,

            UseJump = true,
            JumpPower = -50f,
            MoveEase = Ease.InQuad,

            UseScale = true,
            Scale = Vector3.zero,
            ScaleEase = Ease.InQuad,

            UseRotation = false,

            UseOpacity = true,
            Opacity = 0.5f,
            OpacityEase = Ease.Unset,

            ForceStartScale = true,
            StartScale = Vector3.one * 0.44f,

            ForceStartRotation = false,
        };

        m_temp_card_anime_request = new()
        {
            EndPosition = m_end_transform == null ? Vector3.zero : m_end_transform.position,

            Interval = 0.05f,

            Settings = m_temp_card_settings,
        };
    }

    [Obsolete]
    public void Inject(DiscardPresenter discardPresenter,
                       CardContainer<IDiscardCardUI, DiscardCardPresenter> discardCardContainer)
        => Construct(discardPresenter, discardCardContainer);

    public override void Execute()
    {
        m_temp_card_anime_request.CardDatas = _discardCardContainer.GetAllDatas();

        List<Vector3> discardCardPositionList = new();
        if(!_discardCardContainer.TryGetAllUIs(out IDiscardCardUI[] discardCardUIArray))
        {
            return;
        }

        foreach(IDiscardCardUI cardUI in discardCardUIArray)
        {
            DiscardCardUI concreteCardUI = cardUI as DiscardCardUI;
            discardCardPositionList.Add(concreteCardUI.transform.position);
        }
        m_temp_card_anime_request.StartPositions = discardCardPositionList.ToArray(); 

        base.Execute();
    }

    protected override void OnTempCardAnimeStart(BattleCardData battleCardData)
    {
        if(!_discardCardContainer.TryGetUI(battleCardData, out IDiscardCardUI cardUI))
        {
            return;
        }

        _discardPresenter.RemoveCard(cardUI);
    }

    protected override void OnTempCardAnimeEnd(BattleCardData battleCardData)
    {
        GameData.Instance.UseCard(battleCardData.data.id);
        GameData.Instance.InvokeDeckCountChange(DeckType.Throw);
    }
}
