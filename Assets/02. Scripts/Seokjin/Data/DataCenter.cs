using System.Collections.Generic;
using UnityEngine;

public class DataCenter : Singleton<DataCenter>
{
    public List<CardData> cardDatas = new List<CardData>();

    public List<string> userDeck = new List<string>();

    public void LoadCardDataTable()
    {
        cardDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read("SkillTable.csv");
        for (var i = 0; i < data.Count; i++)
        {
            CardData rdata = new CardData(string.Format("{0:D4}", data[i]["ID"].ToString()), data[i]["Name"].ToString(), data[i]["Description"].ToString());
            cardDatas.Add(rdata);
        }
    }
}
