[System.Serializable]
public class BattleShopSlotData
{
    private readonly CardData m_card_data;
    public CardData Card => m_card_data;

    private readonly int m_card_cost;
    public int Cost => m_card_cost;

    public BattleShopSlotData(CardData card_data,
                              int cost)
    {
        m_card_data = card_data;
        m_card_cost = cost;
    }
}