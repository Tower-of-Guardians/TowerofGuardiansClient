using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class GameData : Singleton<GameData>
{
    [SerializeField] List<string> notuseDeck = new List<string>();
    [SerializeField] List<string> useDeck = new List<string>();
    // TODO: 손 덱
    // TOOD: 공격필드
    // TODO: 방어필드
    // TODO: 쓰레기 필드

    // TODO: 이벤트 notuseDeck 개수 변경됐을때 
    // TODO: 이벤트 useDeck 개수 변경됐을때

    // TODO: 뽑을 카드 더미 프로퍼티 Getter 오픈
    // TODO: 버릴 카드 더미 프로퍼티 Getter 오픈
    private void Start()
    {
        FirstDeckSet();
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

    public CardData NextDeckSet(int count)
    {
        CardData getdata = null;
        for (int i = 0; i < count; i++)
        {
            if (notuseDeck.Count <= 0)
            {
                notuseDeck = new List<string>(useDeck);
                Shuffle();
                useDeck.Clear();
            }

            DataCenter.Instance.GetCardData(notuseDeck[0], (data) =>
            {
                getdata = data;
            });
            useDeck.Add(notuseDeck[0]);
            notuseDeck.RemoveAt(0);
        }

        Debug.LogFormat("ID : {0} , [{1}] �ε� ����.",getdata.id,getdata.itemName);
        return getdata;
    }

    public void Shuffle()
    {
        for (int i = notuseDeck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            string temp = notuseDeck[i];
            notuseDeck[i] = notuseDeck[j];
            notuseDeck[j] = temp;
        }
    }
}
