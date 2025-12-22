using UnityEngine;

// CSV의 한 행에 대응하는 데이터 구조
[CreateAssetMenu(fileName = "MonsterEncounterData", menuName = "Data/MonsterEncounterData")]
public class MonsterEncounterData : ScriptableObject
{
    public string Id;
    public string Name;
    public int Section;
}