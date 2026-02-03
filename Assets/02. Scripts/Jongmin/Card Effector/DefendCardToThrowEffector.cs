using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class DefendCardToThrowEffector : CardEffector
{
    [Header("카드 부모 트랜스폼")]
    [SerializeField] private Transform m_card_root;

    private DefendFieldPresenter m_defend_field_presenter;

    public void Inject(DefendFieldPresenter defend_field_presenter)
    {
        m_defend_field_presenter = defend_field_presenter;

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
        m_temp_card_anime_request.CardDatas = m_defend_field_presenter.GetCardDatas();

        List<Vector3> field_card_positions = new();
        foreach(IFieldCardView card_view in m_defend_field_presenter.GetCardViews())
            field_card_positions.Add((card_view as FieldCardView).transform.position);

        m_temp_card_anime_request.StartPositions = field_card_positions.ToArray(); 

        base.Execute();
    }

    protected override void OnTempCardAnimeStart(BattleCardData card_data)
        => m_defend_field_presenter.Remove(m_defend_field_presenter.GetCardView(card_data));

    protected override void OnTempCardAnimeEnd(BattleCardData card_data)
    {
        GameData.Instance.defenseField.Remove(card_data.data);
        GameData.Instance.UseCard(card_data.data.id);
        GameData.Instance.InvokeDeckCountChange(DeckType.Throw);    
    }
}
