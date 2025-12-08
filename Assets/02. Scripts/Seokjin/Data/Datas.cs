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
    public CardData data = ScriptableObject.CreateInstance<CardData>();
}

[Serializable]
public class PlayerState
{
    public int levle;
    public int experience;
    public int hp;
    public int lhp;
    public int atk;
    public int latk;
    public int maxmagic;
}