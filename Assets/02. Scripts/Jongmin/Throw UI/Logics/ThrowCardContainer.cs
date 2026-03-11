using System.Collections.Generic;
using System.Linq;

public class ThrowCardContainer
{
    private readonly List<IDiscardCardUI> m_card_list = new();
    private readonly Dictionary<IDiscardCardUI, ThrowCardPresenter> m_card_dict = new();

    public IReadOnlyList<IDiscardCardUI> Cards => m_card_list;
    public IReadOnlyDictionary<IDiscardCardUI, ThrowCardPresenter> Dict => m_card_dict;

    public void Add(IDiscardCardUI card_view, ThrowCardPresenter card_presenter)
    {
        m_card_list.Add(card_view);
        m_card_dict[card_view] = card_presenter;
    }

    public void Remove(IDiscardCardUI card_view)
    {
        m_card_list.Remove(card_view);
        m_card_dict.Remove(card_view);
    }

    public void Swap(IDiscardCardUI from_card_view, IDiscardCardUI to_card_view)
    {
        int from_index = m_card_list.IndexOf(from_card_view);
        int to_index   = m_card_list.IndexOf(to_card_view);
        
        if (from_index < 0 || to_index < 0 || from_index == to_index) 
            return;

        m_card_list.RemoveAt(from_index);
        m_card_list.Insert(to_index, from_card_view);
    }

    public bool IsPriority(IDiscardCardUI from_card_view, IDiscardCardUI to_card_view)
    {
        var from_index = m_card_list.IndexOf(from_card_view);
        var to_index = m_card_list.IndexOf(to_card_view);

        return from_index < to_index;
    }

    public bool IsExist(IDiscardCardUI target_card_view)
    {
        foreach(var card_view in m_card_list)
            if(card_view == target_card_view)
                return true;
        
        return false;
    }

    public int GetIndex(IDiscardCardUI card_view)
        => m_card_list.IndexOf(card_view);

    public void Clear()
    {
        m_card_list.Clear();
        m_card_dict.Clear();
    }

    public BattleCardData[] GetDatas()
        => m_card_list
                .Select(view => m_card_dict[view].BattleCardData)
                .ToArray();

    public IDiscardCardUI[] GetCardViews()
        => m_card_list.ToArray();

    public IDiscardCardUI GetCardView(BattleCardData card_data)
    {
        if (card_data == null) 
            return null;

        foreach(IDiscardCardUI card_view in m_card_list)
        {
            if(m_card_dict[card_view].CardData.id == card_data.data.id)
                return card_view;
        }

        return null;
    }

    public BattleCardData GetData(IDiscardCardUI card_view)
        => m_card_dict.TryGetValue(card_view, out var presenter) ? presenter.BattleCardData
                                                                 : null;
}
