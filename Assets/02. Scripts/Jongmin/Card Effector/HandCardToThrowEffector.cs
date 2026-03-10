using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;
using VContainer;

public class HandCardToThrowEffector : CardEffector
{
    [SerializeField] private Transform _handCardRoot;
    [SerializeField] private UILocker _battleLocker;

    private HandPresenter _handPresenter;
    private CardContainer<IHandCardUI, HandCardPresenter> _handCardContainer;

    [Inject]
    private void Construct(HandPresenter handPresenter,
                           CardContainer<IHandCardUI, HandCardPresenter> handCardContainer)
    {
        _handPresenter = handPresenter;
        _handCardContainer = handCardContainer;

        m_temp_card_settings = new()
        {
            Duration = 0.5f,

            UseJump = true,
            JumpPower = 50f,
            MoveEase = Ease.Unset,

            UseScale = true,
            Scale = Vector3.one * 0.11f,
            ScaleEase = Ease.InQuad,

            UseRotation = true,
            TargetEuler = new Vector3(0f, 0f, -180f),
            RotateMode = RotateMode.LocalAxisAdd,
            RotateEase = Ease.Unset,

            UseOpacity = true,
            Opacity = 0.5f,
            OpacityEase = Ease.Unset,

            ForceStartScale = true,
            StartScale = Vector3.one * 0.66f,

            ForceStartRotation = true,

            ForceStartOpacity = false,
        };

        m_temp_card_anime_request = new()
        {
            TargetRoot = _handCardRoot,
            EndPosition = m_end_transform == null ? Vector3.zero : m_end_transform.position,

            Interval = 0.1f,

            Settings = m_temp_card_settings,
        };        
    }

    [Obsolete]
    public void Inject(HandPresenter handPresenter)
    {
        _handPresenter = handPresenter;

        m_temp_card_settings = new()
        {
            Duration = 0.5f,

            UseJump = true,
            JumpPower = 50f,
            MoveEase = Ease.Unset,

            UseScale = true,
            Scale = Vector3.one * 0.11f,
            ScaleEase = Ease.InQuad,

            UseRotation = true,
            TargetEuler = new Vector3(0f, 0f, -180f),
            RotateMode = RotateMode.LocalAxisAdd,
            RotateEase = Ease.Unset,

            UseOpacity = true,
            Opacity = 0.5f,
            OpacityEase = Ease.Unset,

            ForceStartScale = true,
            StartScale = Vector3.one * 0.66f,

            ForceStartRotation = true,

            ForceStartOpacity = false,
        };

        m_temp_card_anime_request = new()
        {
            TargetRoot = _handCardRoot,
            EndPosition = m_end_transform == null ? Vector3.zero : m_end_transform.position,

            Interval = 0.1f,

            Settings = m_temp_card_settings,
        };
    }

    public override void Execute()
    {
        _battleLocker.Lock(true);

        m_temp_card_anime_request.CardDatas = _handCardContainer.GetAllDatas();

        List<Vector3> handCardPositionList = new();
        if(!_handCardContainer.TryGetAllUIs(out IHandCardUI[] handCardArray))
        {
            return;
        }

        foreach(IHandCardUI cardUI in handCardArray)
        {
            HandCardUI concreteCardUI = cardUI as HandCardUI;
            handCardPositionList.Add(concreteCardUI.transform.position);
        }

        List<Vector3> handCardRotationList = new();
        if(!_handCardContainer.TryGetAllUIs(out handCardArray))
        {
            return;
        }

        foreach(IHandCardUI card_view in handCardArray)
        {
            HandCardUI concreteCardUI = card_view as HandCardUI;
            handCardRotationList.Add(concreteCardUI.transform.eulerAngles);
        }

        m_temp_card_anime_request.StartPositions = handCardPositionList.ToArray(); 
        m_temp_card_anime_request.StartRotations = handCardRotationList.ToArray();

        base.Execute();
    }


    protected override void OnTempCardAnimeStart(BattleCardData card_data)
    {
        if(!_handCardContainer.TryGetUI(card_data, out IHandCardUI cardUI))
        {
            return;
        }

        _handPresenter.RemoveCard(cardUI);
    }

    protected override void OnTempCardAnimeEnd(BattleCardData card_data)
    {
        GameData.Instance.handDeck.Remove(card_data.data.id);
        GameData.Instance.UseCard(card_data.data.id);
        GameData.Instance.InvokeDeckCountChange(DeckType.Throw);        
    }
}
