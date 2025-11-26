using System;
using System.Collections.Generic;

public class ThrowPresenter : IDisposable
{
    private readonly IThrowView m_view;
    private readonly TurnManager m_turn_manager;
    private HandPresenter m_hand_presenter;

    private List<IThrowCardView> m_card_list;
    private Dictionary<IThrowCardView, ThrowCardPresenter> m_card_dict;

    private bool m_is_throwed;
    private IThrowCardView m_hover_card;

    public event Action<bool> OnUpdatedToggleUI;

    private bool Active { get; set; }

    public List<IThrowCardView> Cards
    {
        get => m_card_list;
        set => m_card_list = value;
    }

    public IThrowCardView HoverCard
    {
        get => m_hover_card;
        set => m_hover_card = value;
    }

    public ThrowPresenter(IThrowView view,
                          TurnManager turn_manager)
    {
        m_card_list = new();
        m_card_dict = new();

        m_view = view;
        m_turn_manager = turn_manager;

        m_turn_manager.OnUpdatedThrowActionState += UpdateThrowState;
        m_turn_manager.OnUpdatedThrowCount += UpdateThrowCount;

        m_view.Inject(this);
        m_turn_manager.Initialize();
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

        m_turn_manager.UpdateThrowCount(-m_card_list.Count);
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
        m_turn_manager.UpdateThrowAction(false);
        
        m_view.ThrowUI();

        Return();

        OnUpdatedToggleUI?.Invoke(false);
    }

    public bool InstantiateCard()
    {
        var card_view = m_view.InstantiateCardView();
        m_card_list.Add(card_view);

        var card_presenter = new ThrowCardPresenter(card_view);
        m_card_dict.TryAdd(card_view, card_presenter);

        return true;
    }

    public void ToggleManual(bool active)
    {
        if(!Active)
            return;

        m_view.ToggleManual(active);
    }

    public void UpdateThrowState(bool active)
    {
        m_view.UpdateUI(active, active);
    }

    public void UpdateThrowCount(ActionData throw_data)
    {
        if(throw_data.Current <= 0)
            m_view.UpdateUI(true, false);
        else
            m_view.UpdateUI(true, true);
    }

    public CardData[] GetCardDatas()
    {
        var card_data_list = new List<CardData>();

        foreach(var card_view in m_card_list)
            if(m_card_dict.TryGetValue(card_view, out var card_presenter))
                card_data_list.Add(card_presenter.Data);

        return card_data_list.ToArray();
    }

    public CardData GetCardData(IThrowCardView card_view)
    {
        if(m_card_dict.TryGetValue(card_view, out var card_presenter))
            return card_presenter.Data;
        
        return null;
    }

    public void OnTempCardAnimeEnd(CardData card_data)
    {
        if(!m_is_throwed)
            m_hand_presenter.InstantiateCard(card_data);
    }

    public void OnDroped(IHandCardView card_view)
    {
        if(!m_turn_manager.CanThrow())
        {
            m_view.PrintNotice("<color=red>더 이상 버릴 수 없습니다.</color>");
            return;
        }

        InstantiateCard();
        m_hand_presenter.RemoveCard(card_view);
        m_turn_manager.UpdateThrowCount(1);
    }

    public void RemoveCard(IThrowCardView card_view)
    {
        if(m_card_dict.TryGetValue(card_view, out var card_presenter))
        {
            m_card_list.Remove(card_view);
            m_card_dict.Remove(card_view);

            m_view.ReturnCard(card_view, card_presenter.Data);
            m_turn_manager.UpdateThrowCount(-1);
        }
    }

    public void Dispose()
    {
        m_turn_manager.OnUpdatedThrowActionState -= UpdateThrowState;
    }
}
