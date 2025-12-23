using System.Collections.Generic;
using UnityEngine;

public class BattleShopPresenter
{
    private readonly IBattleShopView m_view;
    private readonly BattleShopCardFactory m_factory;

    private List<BattleCardData> m_card_datas;
    private int index = 0;

    private int m_current_cost;

    private readonly int MIN_REFRESH_COST = 5;
    private readonly int MAX_REFRESH_COST = 20;
    private readonly int INIT_REFRESH_COST = 5;
    private readonly int REFRESH_COST_INTERVAL = 5;

    public BattleShopPresenter(IBattleShopView view,
                               BattleShopCardFactory factory)
    {
        m_view = view;
        m_factory = factory;
        
        m_view.Inject(this);
    }

    public void OpenUI()
    {
        m_view.OpenUI();

        InitRefreshCost();
        SetRate();
    }

    public void CloseUI()
        => m_view.CloseUI();

    public void InstantiateCard()
    {
        var slot_view = m_factory.InstantiateCardView();

        var shop_slot_data = new ShopCardData(m_card_datas[index++]); 
        var slot_presenter = new BattleShopSlotPresenter(slot_view, shop_slot_data);
    }

    public void Refresh()
    {
        UpdateCandidateCards();
        UpdateRefreshCost();
    }

    public void RemoveCards()
        => m_factory.RemoveCards();

    private void SetRate()
    {
        var color_arr = new List<string>{ "828282", "4AA8D8", "FEFD48", "F06464" };
        var rate_list = GameData.Instance.GetResultPercent();

        var rate_string = string.Empty;
        for(int i = 0; i < rate_list.Count; i++)
        {
            rate_string += i < rate_list.Count - 1 ? $"<color=#{color_arr[i]}>{rate_list[i]}%</color>   "
                                                   : $"<color=#{color_arr[i]}>{rate_list[i]}%</color>";
        }
            
        m_view.UpdateRate(rate_string);
    }

    private void InitRefreshCost()
        => m_current_cost = INIT_REFRESH_COST - REFRESH_COST_INTERVAL;

    private void UpdateRefreshCost()
    {
        m_current_cost += REFRESH_COST_INTERVAL;
        m_current_cost = Mathf.Clamp(m_current_cost, MIN_REFRESH_COST, MAX_REFRESH_COST);

        // TODO: 플레이어 정보가 생긴다면 true를 플레이어의 현재 보유 골드와 비교하여 나타내야 함.
        m_view.UpdateRefresh($"<size=20>새로고침</size>\n<color=#99CCFF>${m_current_cost}</color>", true);
    }

    private void UpdateCandidateCards()
    {
        index = 0;
        m_card_datas = GameData.Instance.GetResultItems();
    }
}
