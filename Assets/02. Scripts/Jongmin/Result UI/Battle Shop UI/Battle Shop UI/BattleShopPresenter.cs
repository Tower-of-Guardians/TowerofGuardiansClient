using System.Collections.Generic;

public class BattleShopPresenter
{
    private readonly IBattleShopView m_view;
    private readonly BattleShopCardFactory m_factory;

    private List<BattleCardData> m_card_datas;
    private int index = 0;

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
        SetRate();
    }

    public void CloseUI()
        => m_view.CloseUI();

    public void InstantiateCard()
    {
        var slot_view = m_factory.InstantiateCardView();

        var shop_slot_data = new BattleShopSlotData(m_card_datas[index++]); 
        var slot_presenter = new BattleShopSlotPresenter(slot_view, shop_slot_data);
    }

    public void Refresh()
    {
        index = 0;
        m_card_datas = GameData.Instance.GetResultItems();
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
}
