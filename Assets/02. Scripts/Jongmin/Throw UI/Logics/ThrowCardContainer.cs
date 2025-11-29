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

    public void Clear()
    {
        m_card_list.Clear();
        m_card_dict.Clear();
    }

    public CardData[] GetDatas()
        => m_card_dict.Values.Select(x => x.CardData)
                             .ToArray();

    public CardData GetData(IThrowCardView card_view)
        => m_card_dict.TryGetValue(card_view, out var presenter) ? presenter.CardData
                                                                 : null;
}
