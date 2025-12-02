using System.Collections.Generic;
using System.Linq;

public class FieldCardContainer
{
    private readonly List<IFieldCardView> m_card_list = new();
    private readonly Dictionary<IFieldCardView, FieldCardPresenter> m_card_dict = new();

    public IReadOnlyList<IFieldCardView> Cards => m_card_list;
    public IReadOnlyDictionary<IFieldCardView, FieldCardPresenter> Dict => m_card_dict;

    public void Add(IFieldCardView card_view, FieldCardPresenter card_presenter)
    {
        m_card_list.Add(card_view);
        m_card_dict[card_view] = card_presenter;
    }

    public void Remove(IFieldCardView card_view)
    {
        m_card_list.Remove(card_view);
        m_card_dict.Remove(card_view);
    }

    public void Swap(IFieldCardView from_card_view, IFieldCardView to_card_view)
    {
        var index = m_card_list.IndexOf(to_card_view);
        
        m_card_list.Remove(from_card_view);
        m_card_list.Insert(index, from_card_view);
    }

    public bool IsPriority(IFieldCardView from_card_view, IFieldCardView to_card_view)
    {
        var from_index = m_card_list.IndexOf(from_card_view);
        var to_index = m_card_list.IndexOf(to_card_view);

        return from_index < to_index;
    }

    public bool IsExist(IFieldCardView target_card_view)
    {
        foreach(var card_view in m_card_list)
            if(card_view == target_card_view)
                return true;
        
        return false;
    }

    public int GetIndex(IFieldCardView card_view)
        => m_card_list.IndexOf(card_view);

    public void Clear()
    {
        m_card_list.Clear();
        m_card_dict.Clear();
    }

    public BattleCardData[] GetDatas()
        => m_card_dict.Values.Select(x => x.CardData)
                             .ToArray();

    public BattleCardData GetData(IFieldCardView card_view)
        => m_card_dict.TryGetValue(card_view, out var presenter) ? presenter.CardData
                                                                 : null;
}
