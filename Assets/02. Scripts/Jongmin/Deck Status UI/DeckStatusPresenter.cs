using System;
using System.Collections.Generic;

public class DeckStatusPresenter : IDisposable
{
    private readonly IDeckStatusView m_view;

    private List<IDeckStatusCardView> m_card_list;
    private Dictionary<IDeckStatusCardView, DeckStatusCardPresenter> m_card_dict;

    public DeckStatusPresenter(IDeckStatusView view)
    {
        m_card_list = new();
        m_card_dict = new();

        m_view = view;
        m_view.Inject(this);

        GameData.Instance.DeckChange += UpdateCardCount;
        GameData.Instance.InvokeDeckCountChange(DeckType.Draw);
        GameData.Instance.InvokeDeckCountChange(DeckType.Throw);
    }

    public void OpenUI(DeckType deck_type)
    {
        var title_name = GetTitleName(deck_type);

        m_view.OpenUI();
        m_view.UpdateUI(title_name);

        var card_data_list = GameData.Instance.GetDeckDatas(deck_type);

        foreach (var card_data in card_data_list)
            InstantiateCard(card_data);
    }

    public void CloseUI()
    {
        m_card_list.Clear();
        m_card_dict.Clear();

        m_view.CloseUI();
    }

    public void InstantiateCard(BattleCardData card_data)
    {
        var card_view = m_view.InstantiateCardView();
        m_card_list.Add(card_view);

        var card_presenter = new DeckStatusCardPresenter(card_view,
                                                         card_data);
        m_card_dict.TryAdd(card_view, card_presenter);
    }

    private void UpdateCardCount(DeckType deck_type, int count)
    {
        switch(deck_type)
        {
            case DeckType.Draw:
                m_view.UpdateDrawCardCount(count);
                break;
            
            case DeckType.Throw:
                m_view.UpdateThrowCardCount(count);
                break;
        }
    }

    private string GetTitleName(DeckType deck_type)
    {
        return deck_type switch
        {
            DeckType.Draw   => "뽑을 카드 더미",
            DeckType.Throw  => "버릴 카드 더미",
            _               => string.Empty
        };
    }

    public void Dispose()
        => GameData.Instance.DeckChange -= UpdateCardCount;
}