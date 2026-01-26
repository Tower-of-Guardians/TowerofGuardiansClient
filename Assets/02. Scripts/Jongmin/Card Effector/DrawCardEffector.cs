public class DrawCardEffector : CardEffector
{
    private HandPresenter m_hand_presenter;

    public void Inject(HandPresenter hand_presenter)
        => m_hand_presenter = hand_presenter;

    protected override void OnTempCardAnimeEnd(BattleCardData card_data)
        => m_hand_presenter.InstantiateCard(card_data);

}
