using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SaveData
{
    public List<string> userDeckData = new List<string>();
}

[Serializable]
public class BattleCardData
{
    public int index;
    public CardData data;
}