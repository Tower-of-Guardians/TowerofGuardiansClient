using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class DefendCardToThrowEffector : CardEffector
{
    [Header("전투 비활성화 패널")]
    [SerializeField] private UILocker _battleLocker;

    [Header("공격 필드 비활성화 패널")]
    [SerializeField] private UILocker _atkFieldLocker;

    [Header("방어 필드 비활성화 패널")]
    [SerializeField] private UILocker _defFieldLocker;

    private DefendFieldPresenter _defFieldPresenter;
    private CardContainer<IFieldCardUI, FieldCardPresenter> _defFieldCardContainer;

    public void Inject(DefendFieldPresenter defendFieldPresenter,
                       CardContainer<IFieldCardUI, FieldCardPresenter> defFieldCardContainer)
    {
        _defFieldPresenter = defendFieldPresenter;
        _defFieldCardContainer = defFieldCardContainer;

        m_temp_card_settings = new()
        {
            Duration = 0.5f,

            UseJump = true,
            JumpPower = 150f,
            MoveEase = Ease.InOutQuad,

            UseScale = true,
            Scale = Vector3.one * 0.11f,
            ScaleEase = Ease.InQuad,

            UseRotation = true,
            TargetEuler = new Vector3(0f, 0f, -180f),
            RotateMode = RotateMode.LocalAxisAdd,
            RotateEase = Ease.InOutQuad,

            UseOpacity = true,
            Opacity = 0.5f,
            OpacityEase = Ease.Unset,

            ForceStartScale = true,
            StartScale = Vector3.one * 0.66f,

            ForceStartRotation = true,
        };

        m_temp_card_anime_request = new()
        {
            //TargetRoot = m_card_root,

            EndPosition = m_end_transform == null ? Vector3.zero : m_end_transform.position,

            StartRotation = Vector3.zero,

            Interval = 0.1f,

            Settings = m_temp_card_settings,
        };
    }

    public override void Execute()
    {
        m_temp_card_anime_request.CardDatas = _defFieldCardContainer.GetAllDatas();

        List<Vector3> fieldCardPositionList = new();
        if(!_defFieldCardContainer.TryGetAllUIs(out IFieldCardUI[] fieldUIArray))
        {
            return;
        }

        foreach(IFieldCardUI cardUI in fieldUIArray)
        {
            FieldCardUI concreteCardUI = cardUI as FieldCardUI;
            fieldCardPositionList.Add(concreteCardUI.transform.position);
        }

        m_temp_card_anime_request.StartPositions = fieldCardPositionList.ToArray();  

        base.Execute();
    }

    protected override void OnTempCardAnimeStart(BattleCardData card_data)
    {
        if(!_defFieldCardContainer.TryGetUI(card_data, out IFieldCardUI cardUI))
        {
            return;
        }

        _defFieldPresenter.RemoveCard(cardUI);
    }

    protected override void OnTempCardAnimeEnd(BattleCardData card_data)
    {
        GameData.Instance.defenseField.Remove(card_data.data);
        GameData.Instance.UseCard(card_data.data.id);
        GameData.Instance.InvokeDeckCountChange(DeckType.Throw);    
    }

    protected override void OnFinalAnimeEnd()
    {
        _battleLocker.Lock(false);
        _atkFieldLocker.Lock(false);
        _defFieldLocker.Lock(false);
    }
}
