using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;
using static UnityEngine.Rendering.VolumeComponent;

public class GameData : Singleton<GameData>
{
    public List<string> notuseDeck = new List<string>(); // 미사용덱
    public List<string> handDeck = new List<string>(); // 핸드덱
    public List<string> garbageDeck = new List<string>(); // 사용덱

    public event Action<DeckType, int> DeckChange;

    public List<CardData> attackField = new List<CardData>();
    public List<CardData> defenseField = new List<CardData>();

    public int player_lever = 1;
    private void Start()
    {
        FirstDeckSet();
        DeckChange?.Invoke(DeckType.Draw, notuseDeck.Count);
    }

    /// <summary>
    /// 덱 정보 변경시 카운트 정보 이벤트
    /// </summary>
    /// <param name="deck_type"></param>
    public void InvokeDeckCountChange(DeckType deck_type)
        => DeckChange?.Invoke(deck_type, deck_type == DeckType.Draw ? notuseDeck.Count
                                                                    : garbageDeck.Count);

    /// <summary>
    /// 처음 시작시 덱 정보 불러오고 섞기
    /// </summary>
    public void FirstDeckSet()
    {
        if (DataCenter.Instance.userDeck.Count <= 0)
        {
            Debug.Log("���� �� �����Ͱ� �����ϴ�");
            return;
        }
        foreach (string id in DataCenter.Instance.userDeck)
        {
            notuseDeck.Add(id);
        }

        Shuffle();
    }

    /// <summary>
    /// 카드 뽑을때 데이터 받아올수 있게
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public BattleCardData NextDeckSet(int count)
    {
        BattleCardData getdata = new BattleCardData();
        for (int i = 0; i < count; i++)
        {
            if (notuseDeck.Count <= 0)
            {
                notuseDeck = new List<string>(garbageDeck);
                Shuffle();
                garbageDeck.Clear();
            }

            DataCenter.Instance.GetCardData(notuseDeck[0], (data) =>
            {
                getdata.data = data;
            });
            handDeck.Add(notuseDeck[0]);
            notuseDeck.RemoveAt(0);
        }

        getdata.index = handDeck.Count - 1;

        Debug.LogFormat("ID : {0} , [{1}] , index : {2} .", getdata.data.id, getdata.data.itemName, getdata.index);

        InvokeDeckCountChange(DeckType.Draw);
        InvokeDeckCountChange(DeckType.Throw);
        return getdata;
    }

    /// <summary>
    /// 핸드에서 필드에 넣을때
    /// </summary>
    /// <param name="bc_data"></param>
    public void HandToFieldMove(BattleCardData bc_data)
    {
        handDeck.Remove(bc_data.data.id);
    }
    /// <summary>
    /// 필드에서 핸드에 넣을때
    /// </summary>
    /// <param name="bc_data"></param>
    public void FieldToHandMove(BattleCardData bc_data)
    {
        handDeck.Add(bc_data.data.id);
    }

    /// <summary>
    /// 사용하지 않은 카드 섞기
    /// </summary>
    public void Shuffle()
    {
        for (int i = notuseDeck.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);

            string temp = notuseDeck[i];
            notuseDeck[i] = notuseDeck[j];
            notuseDeck[j] = temp;
        }
    }

    public float AttackField()
    {
        float damege = 0f;
        foreach (CardData data in attackField)
        {
            damege += data.ATK;
        }
        return damege;
    }

    public float DefenseField()
    {
        float shield = 0f;
        foreach (CardData data in defenseField)
        {
            shield += data.DEF;
        }
        return shield;
    }

    /// <summary>
    /// 사용한 카드 리스트에 추가
    /// </summary>
    /// <param name="index"></param>
    public void UseCard(string index)
    {
        garbageDeck.Add(index);
    }

    /// <summary>
    /// 덱 가지고 있는거 표기해주기 위한 리스트 요청
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public List<BattleCardData> GetDeckDatas(DeckType type)
    {
        List<BattleCardData> deck_data = new List<BattleCardData>();

        List<string> cards = new List<string>();

        switch (type)
        {
            case DeckType.Draw:
                cards = notuseDeck;
                break;
            case DeckType.Throw:
                cards = garbageDeck;
                break;
        }

        int index = 0;
        foreach (string card in cards)
        {
            DataCenter.Instance.GetCardData(card, (data) =>
            {
                BattleCardData battleCardData = new BattleCardData();
                battleCardData.index = index;
                battleCardData.data = data;
                deck_data.Add(battleCardData);
            });
        }

        int curIndex;
        int preIndex;
        BattleCardData value;

        for (curIndex = 1; curIndex < deck_data.Count; curIndex++)
        {
            value = deck_data[curIndex];
            preIndex = curIndex - 1;

            while (preIndex >= 0 && int.Parse(value.data.id) < int.Parse(deck_data[preIndex].data.id))
            {
                deck_data[preIndex + 1] = deck_data[preIndex];
                preIndex--;
            }
            deck_data[preIndex + 1] = value;
        }

        return deck_data;
    }
    /// <summary>
    /// 스테이지 종료후 랜덤 아이템 상점
    /// </summary>
    /// <returns></returns>
    public List<BattleCardData> GetResultItems()
    {
        ResultPercentData resultPercent = ScriptableObject.CreateInstance<ResultPercentData>();
        DataCenter.Instance.GetResultPercentData(player_lever, (data) =>
        {
            resultPercent = data;
        });
        List<BattleCardData> results = new List<BattleCardData>();

        for (int i = 0; i < 3; i++)
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
    /// <summary>
    /// 확률(등급)에 따른 아이템 뽑기
    /// </summary>
    /// <param name="cut"></param>
    /// <returns></returns>
    public BattleCardData GetRandomCardData(int cut)
    {
        BattleCardData cardData = new BattleCardData();
        cardData.data = null;
        while (cardData.data == null)
        {
            string radom_id = DataCenter.random_card_datas[UnityEngine.Random.Range(0, DataCenter.random_card_datas.Count - 1)].ToString();
            DataCenter.Instance.GetCardData(radom_id, (data) =>
            {
                cardData.data = data;
            });
            if (cardData.data.grade != cut)
            {
                cardData.data = null;
            }
        }
        //Debug.Log($"이름: {cardData.data.itemName}, ID: {cardData.data.id}, 공격력: {cardData.data.ATK}, 방어력: {cardData.data.DEF}, cut : {cut} , grade : {cardData.data.grade}");
        Debug.Log($"grade : {cardData.data.grade}");
        return cardData;
    }
    /// <summary>
    /// 플레이어 레벨에 따른 퍼센트값 리턴(Normal, Rare, Unique, Epic)
    /// </summary>
    /// <returns></returns>
    public List<float> GetResultPercent()
    {
        ResultPercentData resultPercent = ScriptableObject.CreateInstance<ResultPercentData>();
        DataCenter.Instance.GetResultPercentData(player_lever, (data) =>
        {
            resultPercent = data;
        });
        return resultPercent.percent;
    }
}
