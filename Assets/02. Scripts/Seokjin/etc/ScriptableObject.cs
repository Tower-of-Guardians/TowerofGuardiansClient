using System.Collections.Generic;
using UnityEngine;

// CSV의 한 행에 대응하는 데이터 구조
[CreateAssetMenu(fileName = "CardData", menuName = "Data/CardData")]
public class CardData : ScriptableObject
{
    public string id;
    public string itemName;
    public Sprite image;
    public int grade;
    public int star;
    public string synergy1, synergy2, synergy3;
    public string synergy1ID, synergy2ID, synergy3ID;
    public string effectDescription;
    public float ATK;
    public float DEF;
    public string effect1ID, effect2ID;
    public string effect1Value, effect2Value;
    // ... 필요한 필드 추가
}

public class BattleCardData
{
    public int index;
    public CardData data;
}