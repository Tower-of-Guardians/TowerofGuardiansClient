public class ThrowCardPresenter : CardPresenter
{
    private readonly IDiscardCardUI _cardUI;

    public CardData CardData => BattleCardData.data;

    public ThrowCardPresenter(IDiscardCardUI cardUI, 
                              BattleCardData battleCardData)
    {
        _cardUI = cardUI;
        BattleCardData = battleCardData;

        _cardUI.UpdateUI(CardData);
    }
}