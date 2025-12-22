using UnityEngine;

// CSV의 한 행에 대응하는 데이터 구조
[CreateAssetMenu(fileName = "MonsterActionData", menuName = "Data/MonsterActionData")]
public class MonsterActionData : ScriptableObject
{
    public string Id;
    public string Name;
    public int Target;
    public string StatusEffect;
    public string Description;
}