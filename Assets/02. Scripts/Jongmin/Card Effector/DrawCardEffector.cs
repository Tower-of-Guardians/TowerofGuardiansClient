using UnityEngine;
using DG.Tweening;

public class DrawCardEffector : CardEffector
{
    private HandPresenter m_hand_presenter;
    private TurnManager m_turn_manager;

    public void Inject(HandPresenter hand_presenter,
                       TurnManager turn_manager)
    {
        m_hand_presenter = hand_presenter;
        m_turn_manager = turn_manager;

        m_temp_card_settings = new()
        {
            Duration = 0.25f,

            UseJump = true,
            JumpPower = 0f,
            MoveEase = Ease.InQuad,

            UseScale = true,
            Scale = Vector3.one * 0.4f,
            ScaleEase = Ease.Unset,

            UseRotation = false,

            ForceStartScale = true,
            StartScale = Vector3.one * 0.2f,

            ForceStartRotation = false,
        };

        m_temp_card_anime_request = new()
        {
            StartPositions = null,
            StartPosition = m_start_transform != null ? m_start_transform.position : Vector3.zero,
            EndPosition = m_end_transform != null ? m_end_transform.position : Vector3.zero,

            Interval = 0.075f,
            Settings = m_temp_card_settings,
        };
    }

    public override void Execute()
    {
        m_temp_card_anime_request.CardDatas = GameData.Instance.NextDeckSet(m_turn_manager.MaxHandCount).ToArray();

        base.Execute();
    }

    protected override void OnTempCardAnimeEnd(BattleCardData card_data)
        => m_hand_presenter.InstantiateCard(card_data);
}
