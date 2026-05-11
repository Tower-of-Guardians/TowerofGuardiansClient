using UnityEngine;

// CSV의 한 행에 대응하는 데이터 구조
[CreateAssetMenu(fileName = "MonsterData", menuName = "Data/MonsterData")]
public class MonsterData : ScriptableObject
{
    public string Id;
    public string Name;
    public Sprite Image;
    public int HP;
    public int ATKMin;
    public int ATKMax;
    public int DEFMin;
    public int DEFMax;
    public int Kind;
    public int PatternType;
    public string PassiveID;
    public int PassiveValue;
    public string StatusEffect1ID;
    public int Target1;
    public int Value1;
    public string StatusEffect2ID;
    public int Target2;
    public int Value2;
    public string StatusEffect3ID;
    public int Target3;
    public int Value3;

    //ID,Name,HP,ATKMin,ATKMax,DEFMin,DEFMax,Kind,PatternType,PassiveID,PassiveValue,StatusEffect1ID,Target1,Value1,StatusEffect2ID,Target2,Value2,StatusEffect3ID,Target3,Value3
}