using System.Collections.Generic;
using System.Linq;

public class ThrowCardContainer
{
    private readonly List<IThrowCardView> m_card_list = new();
    private readonly Dictionary<IThrowCardView, ThrowCardPresenter> m_card_dict = new();

    public IReadOnlyList<IThrowCardView> Cards => m_card_list;
    public IReadOnlyDictionary<IThrowCardView, ThrowCardPresenter> Dict => m_card_dict;

    public void Add(IThrowCardView card_view, ThrowCardPresenter card_presenter)
    {
        m_card_list.Add(card_view);
        m_card_dict[card_view] = card_presenter;
    }

    public void Remove(IThrowCardView card_view)
    {
        m_card_list.Remove(card_view);
        m_card_dict.Remove(card_view);
    }

    public void Swap(IThrowCardView from_card_view, IThrowCardView to_card_view)
    {
        int from_index = m_card_list.IndexOf(from_card_view);
        int to_index   = m_card_list.IndexOf(to_card_view);
        
        if (from_index < 0 || to_index < 0 || from_index == to_index) 
            return;

        m_card_list.RemoveAt(from_index);
        m_card_list.Insert(to_index, from_card_view);
    }

    public bool IsPriority(IThrowCardView from_card_view, IThrowCardView to_card_view)
    {
        var from_index = m_card_list.IndexOf(from_card_view);
        var to_index = m_card_list.IndexOf(to_card_view);

        return from_index < to_index;
    }

    public bool IsExist(IThrowCardView target_card_view)
    {
        foreach(var card_view in m_card_list)
            if(card_view == target_card_view)
                return true;
        
        return false;
    }

    public int GetIndex(IThrowCardView card_view)
        => m_card_list.IndexOf(card_view);

    public void Clear()
    {
        m_card_list.Clear();
        m_card_dict.Clear();
    }

    public BattleCardData[] GetDatas()
        => m_card_list
                .Select(view => m_card_dict[view].CardData)
                .ToArray();

    public IThrowCardView[] GetCardViews()
        => m_card_list.ToArray();

    public IThrowCardView GetCardView(BattleCardData card_data)
    {
        if (card_data == null) 
            return null;

        foreach(IThrowCardView card_view in m_card_list)
        {
            if(m_card_dict[card_view].CardData.data.id == card_data.data.id)
                return card_view;
        }

        return null;
    }

    public BattleCardData GetData(IThrowCardView card_view)
        => m_card_dict.TryGetValue(card_view, out var presenter) ? presenter.CardData
                                                                 : null;
}
