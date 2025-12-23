using System;
using System.Collections.Generic;
using UnityEngine;

public class MerchantShopDispenser : MonoBehaviour
{
    private List<ShopCardPresenter> m_shop_card_presenter_list = new();

    public event Action OnPurchasedAnyItem;

    public void Inject(List<ShopCardPresenter> shop_card_presenter_list)
    {
        m_shop_card_presenter_list = shop_card_presenter_list;
    }

    public void Initialize()
    {
        // TODO: GameData로부터 카드 5장을 받음.
        var card_data_list = GetRandomCards();

        for(int i = 0; i < m_shop_card_presenter_list.Count; i++)
            m_shop_card_presenter_list[i].Inject(new ShopCardData(card_data_list[i]));
    }

    public void Alert()
        => OnPurchasedAnyItem?.Invoke();

    private List<BattleCardData> GetRandomCards()
    {
        ResultPercentData resultPercent = ScriptableObject.CreateInstance<ResultPercentData>();
        DataCenter.Instance.GetResultPercentData(DataCenter.Instance.playerstate.level + 2, (data) =>
        {
            resultPercent = Instantiate(data);
        });
        List<BattleCardData> results = new List<BattleCardData>();

        for (int i = 0; i < 5; i++)
        {
            float roll = UnityEngine.Random.Range(0, 100);
            float accumulatedChance = 0;

            for (int n = 0; n < resultPercent.percent.Count; n++)
            {
                accumulatedChance += resultPercent.percent[n];
                // 추첨 값이 누적 확률 범위 내에 있으면 해당 등급을 반환
                if (roll <= accumulatedChance)
                {
                    results.Add(GetRandomCardData(n+1));
                    break;
                }
            }
        }

        return results;
    }

    private BattleCardData GetRandomCardData(int cut)
    {
        BattleCardData cardData = new BattleCardData();
        cardData.data = null;
        while (cardData.data == null)
        {
            string radom_id = DataCenter.random_card_datas[UnityEngine.Random.Range(0, DataCenter.random_card_datas.Count - 1)].ToString();
            DataCenter.Instance.GetCardData(radom_id, (data) =>
            {
                cardData.data = Instantiate(data);
            });
            if (cardData.data.grade != cut)
            {
                cardData.data = null;
            }
        }

        return cardData;
    }
}
