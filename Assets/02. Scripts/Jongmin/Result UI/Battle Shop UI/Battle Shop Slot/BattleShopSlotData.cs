[System.Serializable]
public class BattleShopSlotData
{
    private readonly BattleCardData m_card_data;

    public BattleCardData Card => m_card_data;
    public float Cost => Card.data.price;

    public BattleShopSlotData(BattleCardData card_data)
    {
        m_card_data = card_data;
    }
}