using System;
using System.Collections.Generic;

public class DeckStatusPresenter : IDisposable
{
    private readonly IDeckStatusView m_view;
    // TODO: GameData 의존
    // Because of 1. NoUseCard 및 UserCard 참조
    //            2. 카드의 수 변화 이벤트 연결

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
        // TODO: 카드의 수 변화 이벤트에 UpdateCardCount를 연결합니다.
    }

    public void OpenUI(DeckType deck_type)
    {
        var title_name = GetTitleName(deck_type);

        m_view.OpenUI();
        m_view.UpdateUI(title_name);

        var list = deck_type == DeckType.Draw ? GameData.Instance.GetDeckDatas(0)
                                              : GameData.Instance.GetDeckDatas(1);
        
        foreach(var elem in list)
            InstantiateCard(elem);
    }

    public void CloseUI()
    {
        m_card_list.Clear();
        m_card_dict.Clear();

        m_view.CloseUI();
    }

    /// <summary>
    /// GameData의 NotUseDeck, UseDeck 리스트를 이용하여 아래의 함수를 호출하면 자동적으로 카드가 생성됩니다. 
    /// </summary>
    public void InstantiateCard(BattleCardData card_data)
    {
        var card_view = m_view.InstantiateCardView();
        m_card_list.Add(card_view);

        var card_presenter = new DeckStatusCardPresenter(card_view,
                                                         card_data);
        m_card_dict.TryAdd(card_view, card_presenter);
    }

    /// <summary>
    /// UI에게 DeckType에 따라 Draw 또는 Throw 카드 덱의 수를 변경하여 표시하도록 지시합니다.
    /// </summary>
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
        => GameData.Instance.DeckChange += UpdateCardCount;
}