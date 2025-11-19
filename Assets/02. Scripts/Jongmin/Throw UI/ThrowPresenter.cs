using System;
using System.Collections.Generic;

public class ThrowPresenter
{
    private readonly IThrowView m_view;
    private readonly TurnManager m_turn_manager;
    private HandPresenter m_hand_presenter;

    private readonly List<IThrowCardView> m_card_list;
    private readonly Dictionary<IThrowCardView, ThrowCardPresenter> m_card_dict;

    private bool m_is_throwed;

    public event Action<bool> OnUpdatedToggleUI;

    private bool Active { get; set; }

    public ThrowPresenter(IThrowView view,
                          TurnManager turn_manager)
    {
        m_card_list = new();
        m_card_dict = new();

        m_view = view;
        m_turn_manager = turn_manager;

        m_view.Inject(this);
    }

    public void Inject(HandPresenter hand_presenter)
        => m_hand_presenter = hand_presenter;

    public void OnClickedOpenUI()
    {
        Active = true;
        m_is_throwed = false;

        m_view.OpenUI();

        OnUpdatedToggleUI?.Invoke(true);
    }

    public void OnClickedCloseUI()
    {
        Active = false;

        m_view.CloseUI();

        // TODO: 카드가 다시 패로 이동

        m_turn_manager.UpdateActionCount(-m_card_list.Count);
        Return();

        OnUpdatedToggleUI?.Invoke(false);
    }

    public void Return()
    {
        m_card_list.Clear();
        m_card_dict.Clear();

        m_view.ReturnCards();
    }

    public void OnClickedThrowCards()
    {
        Active = false;
        m_is_throwed = true;
        
        m_view.ThrowUI();

        Return();

        OnUpdatedToggleUI?.Invoke(false);
    }

    public bool InstantiateCard()
    {
        var card_view = m_view.InstantiateCardView();
        m_card_list.Add(card_view);

        var card_presenter = new ThrowCardPresenter(card_view,
                                                    this);
        m_card_dict.TryAdd(card_view, card_presenter);

        return true;
    }

    public void ToggleManual(bool active)
    {
        if(!Active)
            return;

        m_view.ToggleManual(active);
    }

    public CardData[] GetCardDatas()
    {
        var card_data_list = new List<CardData>();

        foreach(var card_view in m_card_list)
            if(m_card_dict.TryGetValue(card_view, out var card_presenter))
                card_data_list.Add(card_presenter.Data);

        return card_data_list.ToArray();
    }

    public void OnTempCardAnimeEnd(CardData card_data)
    {
        if(!m_is_throwed)
            m_hand_presenter.InstantiateCard(card_data);
    }

    public void OnDroped(IHandCardView card_view)
    {
        if(!m_turn_manager.CanAction())
        {
            m_view.PrintNotice("<color=red>더 이상 행동할 수 없습니다.</color>");
            return;
        }

        InstantiateCard();
        m_hand_presenter.RemoveCard(card_view);
        m_turn_manager.UpdateActionCount(1);
    }

    public void OnClickedCard(IThrowCardView card_view)
    {
        if(m_card_dict.TryGetValue(card_view, out var card_presenter))
        {
            m_card_list.Remove(card_view);
            m_card_dict.Remove(card_view);

            m_view.ReturnCard(card_view, card_presenter.Data);
            m_turn_manager.UpdateActionCount(-1);
        }
    }
}
