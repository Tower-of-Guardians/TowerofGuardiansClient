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
            DataCenter.Instance.GetCardData("11000001", (data) =>
            {
                card = Instantiate(data);
            });
        });
        textbutton2.onClick.AddListener(() =>
        {
            card.ATK += 10;
            //card.time = DateTime.Now.ToString();
        });

        textbutton3.onClick.AddListener(() =>
        {
            /*if(DateTime.TryParse(card.time, out DateTime resultDate))
                Debug.LogFormat("{0}", resultDate);;*/
        });
    }
}
