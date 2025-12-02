using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameData : Singleton<GameData>
{
    public List<string> notuseDeck = new List<string>(); // 미사용덱
    public List<string> handDeck = new List<string>(); // 핸드덱
    public List<string> garbageDeck = new List<string>(); // 사용덱

    public event EventHandler Deck_Change;

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

        Deck_Change += UseDeckChange;
        Deck_Change += HandDeckChange;
        Deck_Change += GarbageDeckChange;
    }

    private void UseDeckChange(object sender, EventArgs e)
    {
        Debug.Log("GameData UseDeckChange");
    }
    private void HandDeckChange(object sender, EventArgs e)
    {
        Debug.Log("GameData HandDeckChange");
    }
    private void GarbageDeckChange(object sender, EventArgs e)
    {
        Debug.Log("GameData GarbageDeckChange");
    }

    // TODO: 리스트에 추가하는? 인덱스도 
    // TODO: 카드 아이디 => 인덱스 리턴 

    // TODO: 손 덱에서 카드 제거하는 기능
    // ex) public void RemoveCardFromDeck(CardID id);

    // TODO: 손 덱에서 카드 삽입하는 기능
    // ex) public void AddCardFromDeck(CardID id);

    // TODO: 공격, 방어 필드의 카드 스왑 기능 이야기기

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

        getdata.index = handDeck.Count-1;

        Debug.LogFormat("ID : {0} , [{1}] , index : {2} .",getdata.data.id,getdata.data.itemName, getdata.index);

        Deck_Change?.Invoke(this, EventArgs.Empty);
        return getdata;
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
