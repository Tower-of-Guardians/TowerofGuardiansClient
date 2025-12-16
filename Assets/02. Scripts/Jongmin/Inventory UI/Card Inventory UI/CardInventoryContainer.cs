using System.Collections.Generic;

public class CardInventoryContainer
{
    private readonly List<IInventoryCardView> m_card_list = new();
    private readonly Dictionary<IInventoryCardView, InventoryCardPresenter> m_card_dict = new();

    public IReadOnlyList<IInventoryCardView> Cards => m_card_list;
    public IReadOnlyDictionary<IInventoryCardView, InventoryCardPresenter> Dict => m_card_dict;    

    public void Add(IInventoryCardView card_view, InventoryCardPresenter card_presenter)
    {
        m_card_list.Add(card_view);
        m_card_dict[card_view] = card_presenter;
    }

    public void Remove(IInventoryCardView card_view)
    {
        m_card_list.Remove(card_view);
        m_card_dict.Remove(card_view);
    }   

    public void Clear()
    {
        m_card_list.Clear();
        m_card_dict.Clear();
    }
}
