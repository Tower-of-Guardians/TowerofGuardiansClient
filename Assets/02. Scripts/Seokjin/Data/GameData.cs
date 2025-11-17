using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class GameData : MonoBehaviour
{
    [SerializeField] List<CardData> notuseDeck = new List<CardData>();
    [SerializeField] List<CardData> handDeck = new List<CardData>();
    [SerializeField] List<CardData> useDeck = new List<CardData>();
    [SerializeField] Button firstDeckButton;
    [SerializeField] Button nextDeckButton;

    [SerializeField] List<Button> handCardButton;

    int handcardcount = 5;
    int maxhand = 10;
    int nextdrawcount = 3;

    private void Start()
    {
        firstDeckButton.onClick.AddListener(() => { FirstDeckSet(); });
        nextDeckButton.onClick.AddListener(() => { NextDeckSet(nextdrawcount); });

        int n = 0;
        foreach (Button btn in handCardButton)
        {
            int nn = n;
            btn.onClick.AddListener(() => { UseCard(nn); });
            btn.gameObject.SetActive(false);
            n++;
        }
    }

    public void FirstDeckSet()
    {
        foreach (string id in DataCenter.Instance.userDeck)
        {
            foreach (CardData data in DataCenter.Instance.cardDatas)
            {
                if (id == data.Id)
                {
                    notuseDeck.Add(new CardData(data.Id, data.Name, data.Description));
                }
            }
        }

        Shuffle(notuseDeck);
        NextDeckSet(5);
    }

    public void NextDeckSet(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (handDeck.Count >= maxhand)
            {
                break;
            }
            if (notuseDeck.Count <= 0)
            {
                notuseDeck = new List<CardData>(useDeck);
                Shuffle(notuseDeck);
                useDeck.Clear();
                i--;
            }
            handDeck.Add(notuseDeck[0]);
            notuseDeck.RemoveAt(0);
        }
        ShowCard();
    }

    public void Shuffle(List<CardData> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            CardData temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    /////////////////Test¿ë//////////////////////////
    void ShowCard()
    {
        foreach (Button btn in handCardButton)
        {
            btn.gameObject.SetActive(false);
        }

        for (int i = 0; i < handDeck.Count; i++)
        {
            handCardButton[i].gameObject.SetActive(true);
            handCardButton[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                string.Format("Name : {0}\nID : {1}\nDescription : {2}", handDeck[i].Name, handDeck[i].Id, handDeck[i].Description);
        }
    }
    public void UseCard(int number)
    {
        handCardButton[handDeck.Count - 1].gameObject.SetActive(false);
        useDeck.Add(handDeck[number]);
        handDeck.RemoveAt(number);
        ShowCard();
    }
}
