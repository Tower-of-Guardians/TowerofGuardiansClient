using System.Collections.Generic;
using System.Linq;

public class FieldCardContainer
{
    private readonly List<IFieldCardUI> m_card_list = new();
    private readonly Dictionary<IFieldCardUI, FieldCardPresenter> m_card_dict = new();

    public IReadOnlyList<IFieldCardUI> Cards => m_card_list;
    public IReadOnlyDictionary<IFieldCardUI, FieldCardPresenter> Dict => m_card_dict;

    public void Add(IFieldCardUI card_view, FieldCardPresenter card_presenter)
    {
        m_card_list.Add(card_view);
        m_card_dict[card_view] = card_presenter;
    }

    public void Remove(IFieldCardUI card_view)
    {
        m_card_list.Remove(card_view);
        m_card_dict.Remove(card_view);
    }

    public FieldCardPresenter GetPresenter(IFieldCardUI card_view)
        => m_card_dict.TryGetValue(card_view, out var presenter) ? presenter : null;

    public void Swap(IFieldCardUI from_card_view, IFieldCardUI to_card_view)
    {
        var index = m_card_list.IndexOf(to_card_view);
        
        m_card_list.Remove(from_card_view);
        m_card_list.Insert(index, from_card_view);
    }

    public bool IsPriority(IFieldCardUI from_card_view, IFieldCardUI to_card_view)
    {
        var from_index = m_card_list.IndexOf(from_card_view);
        var to_index = m_card_list.IndexOf(to_card_view);

        return from_index < to_index;
    }

    public bool IsExist(IFieldCardUI target_card_view)
    {
        foreach(var card_view in m_card_list)
            if(card_view == target_card_view)
                return true;
        
        return false;
    }

    public int GetIndex(IFieldCardUI card_view)
        => m_card_list.IndexOf(card_view);

    public void Clear()
    {
        m_card_list.Clear();
        m_card_dict.Clear();
    }

    public IFieldCardUI[] GetCardViews()
        => m_card_list.ToArray();

    public IFieldCardUI GetCardView(BattleCardData card_data)
    {
        if (card_data == null) 
            return null;

        foreach(IFieldCardUI card_view in m_card_list)
        {
            if(m_card_dict[card_view].CardData.id == card_data.data.id)
                return card_view;
        }

        return null;
    }

    public BattleCardData[] GetDatas()
        => m_card_list
                .Select(view => m_card_dict[view].BattleCardData)
                .ToArray();

    public BattleCardData GetData(IFieldCardUI card_view)
        => m_card_dict.TryGetValue(card_view, out var presenter) ? presenter.BattleCardData
                                                                 : null;
}
