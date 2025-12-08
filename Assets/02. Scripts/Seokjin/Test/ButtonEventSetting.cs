using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEventSetting : MonoBehaviour
{
    [SerializeField] Button testbutton1 , textbutton2, textbutton3;
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
    }
}
