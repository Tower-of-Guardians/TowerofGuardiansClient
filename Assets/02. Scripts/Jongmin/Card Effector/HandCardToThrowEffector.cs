using System.Collections.Generic;
using DG.Tweening;
using JxModule;
using UnityEngine;
using VContainer;

public class HandCardToThrowEffector : CardEffector
{
    [SerializeField] private Transform _handCardRoot;
    [SerializeField] private UILocker _battleLocker;

    private IHandCardRemovePort _handCardRemovePort;
    private Jongmin.CardContainer _handCardContainer;

    [Inject]
    private void Construct(IHandCardRemovePort handCardRemovePort,
                           Jongmin.CardContainer handCardContainer)
    {
        _handCardRemovePort = handCardRemovePort;
        _handCardContainer = handCardContainer;

        _tempCardSettings = new()
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

        _tempCardAnimeRequest = new()
        {
            TargetRoot = _handCardRoot,
            EndPosition = _endTransform == null ? Vector3.zero : _endTransform.position,

            Interval = 0.1f,

            Settings = _tempCardSettings,
        };
    }

    public override void Execute()
    {
        _battleLocker.Lock(true);

        var cardDataList = new List<BattleCardData>();
        var handCardPositionList = new List<Vector3>();
        var handCardRotationList = new List<Vector3>();

        foreach (var card in _handCardContainer.Cards)
        {
            if (card == null || card.BattleCardData == null)
            {
                continue;
            }

            cardDataList.Add(card.BattleCardData);
            handCardPositionList.Add(card.transform.position);
            handCardRotationList.Add(card.transform.eulerAngles);
        }

        _tempCardAnimeRequest.CardDatas = cardDataList.ToArray();
        _tempCardAnimeRequest.StartPositions = handCardPositionList.ToArray();
        _tempCardAnimeRequest.StartRotations = handCardRotationList.ToArray();

        base.Execute();
    }

    protected override void OnTempCardAnimeStart(BattleCardData battleCardData)
        => _handCardRemovePort.TryRemoveCard(battleCardData);

    protected override void OnTempCardAnimeEnd(BattleCardData battleCardData)
    {
        GameData.Instance.handDeck.Remove(battleCardData.data.id);
        GameData.Instance.UseCard(battleCardData.data.id);
        GameData.Instance.InvokeDeckCountChange(DeckType.Throw);
    }
}
