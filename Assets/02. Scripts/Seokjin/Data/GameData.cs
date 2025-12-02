using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;

public class GameData : Singleton<GameData>
{
    public List<string> notuseDeck = new List<string>(); // 미사용덱
    public List<string> handDeck = new List<string>(); // 핸드덱
    public List<string> garbageDeck = new List<string>(); // 사용덱

    public event Action<DeckType, int> DeckChange;

    public List<string> attackField = new List<string>();
    public List<string> defenseField = new List<string>();

    private void Start()
    {
        FirstDeckSet();
        DeckChange?.Invoke(DeckType.Draw, notuseDeck.Count);
    }

    public void InvokeDeckCountChange(DeckType deck_type)
        => DeckChange?.Invoke(deck_type, deck_type == DeckType.Draw ? notuseDeck.Count
                                                                    : garbageDeck.Count);

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
        return getdata;
    }

    public void HandToFieldMove(BattleCardData bc_data)
    {
        handDeck.Remove(bc_data.data.id);
    }

    public void FieldToHandMove(BattleCardData bc_data)
    {
        handDeck.Add(bc_data.data.id);
    }

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

    public void UseCard(string index)
    {
        garbageDeck.Add(index);
    }

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
        foreach(string card in cards)
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
}
