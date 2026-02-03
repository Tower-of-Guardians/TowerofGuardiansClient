using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class HandCardToThrowEffector : CardEffector
{
    [Header("카드 부모 트랜스폼")]
    [SerializeField] private Transform m_card_root;
    private HandPresenter m_hand_presenter;

    public void Inject(HandPresenter hand_presenter)
    {
        m_hand_presenter = hand_presenter;

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

            ForceStartScale = true,
            StartScale = Vector3.one * 0.66f,

            ForceStartRotation = true,
        };

        m_temp_card_anime_request = new()
        {
            TargetRoot = m_card_root,
            EndPosition = m_end_transform == null ? Vector3.zero : m_end_transform.position,

            Interval = 0.1f,

            Settings = m_temp_card_settings,
        };
    }

    public override void Execute()
    {
        m_temp_card_anime_request.CardDatas = m_hand_presenter.GetCardDatas();
        foreach(BattleCardData card_data in m_temp_card_anime_request.CardDatas)
            Debug.Log(card_data.data.id);
        List<Vector3> hand_card_positions = new();
        foreach(IHandCardView card_view in m_hand_presenter.GetCardViews())
            hand_card_positions.Add((card_view as HandCardView).transform.position);

        List<Vector3> hand_card_rotations = new();
        foreach(IHandCardView card_view in m_hand_presenter.GetCardViews())
            hand_card_rotations.Add((card_view as HandCardView).transform.eulerAngles);

        m_temp_card_anime_request.StartPositions = hand_card_positions.ToArray(); 
        m_temp_card_anime_request.StartRotations = hand_card_rotations.ToArray();

        base.Execute();
    }


    protected override void OnTempCardAnimeStart(BattleCardData card_data)
        => m_hand_presenter.RemoveCard(m_hand_presenter.GetCardView(card_data), false);

    protected override void OnTempCardAnimeEnd(BattleCardData card_data)
    {
        GameData.Instance.handDeck.Remove(card_data.data.id);
        GameData.Instance.UseCard(card_data.data.id);
        GameData.Instance.InvokeDeckCountChange(DeckType.Throw);        
    }
}
