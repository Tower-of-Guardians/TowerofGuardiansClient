using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData : Singleton<GameData>
{
    public List<string> notuseDeck = new List<string>(); // 미사용덱
    public List<string> handDeck = new List<string>(); // 핸드덱
    public List<string> garbageDeck = new List<string>(); // 사용덱

    public event Action<DeckType, int> DeckChange;

    public List<string> attackField = new List<string>();
    public List<string> defenseField = new List<string>();
    public List<string> garbageField = new List<string>();

    // TODO: 이벤트 notuseDeck 개수 변경됐을때 
    // TODO: 이벤트 useDeck 개수 변경됐을때

    // TODO: 뽑을 카드 더미 프로퍼티 Getter 오픈
    // TODO: 버릴 카드 더미 프로퍼티 Getter 오픈
    private void Start()
    {
        FirstDeckSet();
        DeckChange?.Invoke(DeckType.Draw, notuseDeck.Count);
    }

    // TODO: 리스트에 추가하는? 인덱스도 
    // TODO: 카드 아이디 => 인덱스 리턴 

    // TODO: 공격, 방어 필드의 카드 스왑 기능 이야기기

    public void InvokeDeckCountChange(DeckType deck_type)
        => DeckChange?.Invoke(deck_type, deck_type == DeckType.Draw ? notuseDeck.Count
                                                                    : garbageDeck.Count);

    public void FirstDeckSet()
    {
        if (DataCenter.Instance.userDeck.Count <= 0)
        {
            Debug.Log("���� �� �����Ͱ� �����ϴ�");
            return; //SaveLoadManager.Instance.LoadGame();
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
}
