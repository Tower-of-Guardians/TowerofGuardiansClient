using System.Collections.Generic;
using System.Linq;

public class HandCardContainer
{
    private List<IHandCardView> m_card_list = new();
    private Dictionary<IHandCardView, HandCardPresenter> m_card_dict = new();

    public IReadOnlyList<IHandCardView> Cards => m_card_list;
    public IReadOnlyDictionary<IHandCardView, HandCardPresenter> Dict => m_card_dict;

    public void Add(IHandCardView card_view, HandCardPresenter card_presenter)
    {
        m_card_list.Add(card_view);
        m_card_dict[card_view] = card_presenter;
    }

    public void Remove(IHandCardView card_view)
    {
        m_card_list.Remove(card_view);
        m_card_dict.Remove(card_view);
    }

    public HandCardPresenter GetPresenter(IHandCardView card_view)
        => m_card_dict.TryGetValue(card_view, out var presenter) ? presenter : null;

    public void Swap(IHandCardView from_card_view, IHandCardView to_card_view)
    {
        var index = m_card_list.IndexOf(to_card_view);
        
        m_card_list.Remove(from_card_view);
        m_card_list.Insert(index, from_card_view);
    }

    public bool IsPriority(IHandCardView from_card_view, IHandCardView to_card_view)
    {
        var from_index = m_card_list.IndexOf(from_card_view);
        var to_index = m_card_list.IndexOf(to_card_view);

        return from_index < to_index;
    }

    public bool IsExist(IHandCardView target_card_view)
    {
        foreach(var card_view in m_card_list)
            if(card_view == target_card_view)
                return true;
        
        return false;
    }

    public int GetIndex(IHandCardView card_view)
        => m_card_list.IndexOf(card_view);

    public void Clear()
    {
        m_card_list.Clear();
        m_card_dict.Clear();
    }

    public BattleCardData[] GetDatas()
        => m_card_dict.Values.Select(x => x.CardData)
                             .ToArray();

    public BattleCardData GetData(IHandCardView card_view)
        => m_card_dict.TryGetValue(card_view, out var presenter) ? presenter.CardData
                                                                 : null;
}
