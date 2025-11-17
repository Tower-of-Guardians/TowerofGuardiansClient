using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;


[Serializable]
public class SaveData
{
    public List<string> userDeckData = new List<string>();
}

[Serializable]
public class CardData
{
    public string Id;
    public string Name;
    public string Description;

    public CardData(string id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
}