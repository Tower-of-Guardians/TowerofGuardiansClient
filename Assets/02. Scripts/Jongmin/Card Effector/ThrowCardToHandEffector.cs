using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class ThrowCardToHandEffector : CardEffector
{
    private ThrowPresenter m_throw_presenter;
    private HandPresenter m_hand_presenter;

    public void Inject(ThrowPresenter throw_presenter,
                       HandPresenter hand_presenter)
    {
        m_throw_presenter = throw_presenter;
        m_hand_presenter = hand_presenter;

        m_temp_card_settings = new()
        {
            Duration = 0.35f,

            UseJump = true,
            JumpPower = 0f,
            MoveEase = Ease.InQuad,

            UseScale = true,
            Scale = Vector3.zero,
            ScaleEase = Ease.InQuad,

            UseRotation = false,

            ForceStartScale = true,
            StartScale = Vector3.one * 0.66f,

            ForceStartRotation = false,
        };

        m_temp_card_anime_request = new()
        {
            EndPosition = m_end_transform == null ? Vector3.zero : m_end_transform.position,

            Interval = 0f,

            Settings = m_temp_card_settings,
        };
    }

    public override void Execute()
    {
        m_temp_card_anime_request.CardDatas = m_throw_presenter.GetCardDatas();

        List<Vector3> throw_card_positions = new();
        foreach(IThrowCardView card_view in m_throw_presenter.GetCardViews())
            throw_card_positions.Add((card_view as ThrowCardView).transform.position);

        m_temp_card_anime_request.StartPositions = throw_card_positions.ToArray(); 

        base.Execute();
    }

    protected override void OnTempCardAnimeStart(BattleCardData card_data)
        => m_throw_presenter.RemoveCard(m_throw_presenter.GetCardView(card_data));

    protected override void OnTempCardAnimeEnd(BattleCardData card_data)
        => m_hand_presenter.InstantiateCard(card_data);
}
