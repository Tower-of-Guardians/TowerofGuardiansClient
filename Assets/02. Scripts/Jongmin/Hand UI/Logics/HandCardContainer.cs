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
        int from_index = m_card_list.IndexOf(from_card_view);
        int to_index   = m_card_list.IndexOf(to_card_view);
        
        if (from_index < 0 || to_index < 0 || from_index == to_index) 
            return;

        m_card_list.RemoveAt(from_index);
        m_card_list.Insert(to_index, from_card_view);
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

    public IHandCardView GetHandCardView(BattleCardData battle_card_data)
    {
        if(battle_card_data == null)
            return null;

        foreach(IHandCardView card_view in m_card_list)
        {
            if(m_card_dict[card_view].CardData.data.id == battle_card_data.data.id)
                return card_view;
        }

        return null;
    }

    public IHandCardView[] GetHandCardViews()
        => m_card_list.ToArray();

    public BattleCardData[] GetDatas()
        => m_card_list
                .Select(view => m_card_dict[view].CardData)
                .ToArray();

    public BattleCardData GetData(IHandCardView card_view)
        => m_card_dict.TryGetValue(card_view, out var presenter) ? presenter.CardData
                                                                 : null;
}
