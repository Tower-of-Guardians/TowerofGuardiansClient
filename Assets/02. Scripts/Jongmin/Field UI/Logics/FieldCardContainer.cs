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

    public void Clear()
    {
        m_card_list.Clear();
        m_card_dict.Clear();
    }

    public CardData[] GetDatas()
        => m_card_dict.Values.Select(x => x.CardData)
                             .ToArray();

    public CardData GetData(IFieldCardView card_view)
        => m_card_dict.TryGetValue(card_view, out var presenter) ? presenter.CardData
                                                                 : null;
}
