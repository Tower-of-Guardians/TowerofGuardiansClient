[System.Serializable]
public class ShopCardData
{
    private readonly BattleCardData m_card_data;

    public BattleCardData Card => m_card_data;
    public float Cost => Card.data.price;

    public ShopCardData(BattleCardData card_data)
        => m_card_data = card_data;
}