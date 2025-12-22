using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEventSetting : MonoBehaviour
{
    [SerializeField] Button testbutton1, textbutton2, textbutton3;
    public CardData card;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        testbutton1.onClick.AddListener(() =>
        {
            DataCenter.Instance.SortUserCards(SortType.Attack);
        });
        textbutton2.onClick.AddListener(() =>
        {
            DataCenter.Instance.SortUserCards(SortType.Defense);
        });

        textbutton3.onClick.AddListener(() =>
        {
            DataCenter.Instance.SortUserCards(SortType.Grade);
        });

        StartCoroutine("skil_1234","1234");
        SendMessage("skil_4321", 4321);
    }

    public void skil_1234(object value)
    {
        if (int.TryParse(value.ToString(), out int result))
            Debug.Log("TEST skil_1234 LOG" + result);
    }
    public void skil_4321(object value)
    {
        if (int.TryParse(value.ToString(), out int result))
            Debug.Log("TEST skil_4321 LOG" + result);
    }
}
