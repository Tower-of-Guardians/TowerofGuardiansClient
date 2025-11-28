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

    private void Start()
    {
        FirstDeckSet();
    }

    public void FirstDeckSet()
    {
        if (DataCenter.Instance.userDeck.Count <= 0)
        {
            Debug.Log("유져 덱 데이터가 없습니다");
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

        Debug.LogFormat("ID : {0} , [{1}] 로드 성공.",getdata.id,getdata.itemName);
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
