using UnityEngine;

// CSV의 한 행에 대응하는 데이터 구조
[CreateAssetMenu(fileName = "SynergyData", menuName = "Data/SynergyData")]
public class SynergyData : ScriptableObject
{
    public string ID;
    public string Name;
    public string Description;
    public int Tier;
    public string Effect1ID;
    public int Effect1Synergy1;
    public int Effect1Synergy2;
    public int Effect1Synergy3;
    public int Effect1Synergy4;
    public int Effect1Synergy5;
    public string Effect2ID;
    public int Effect2Synergy1;
    public int Effect2Synergy2;
    public int Effect2Synergy3;
    public int Effect2Synergy4;
    public int Effect2Synergy5;
    public string Effect3ID;
    public int Effect3Synergy1;
    public int Effect3Synergy2;
    public int Effect3Synergy3;
    public int Effect3Synergy4;
    public int Effect3Synergy5;
}