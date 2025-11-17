using UnityEngine;

[RequireComponent(typeof(MonsterUnit))]
public class MonsterDescriptor : BaseDescriptor
{
    private MonsterUnit m_monster_unit;

    private void Awake()
    {
        m_monster_unit = GetComponent<MonsterUnit>();
    }

    public override TooltipData GetTooltipData()
    {
        return new TooltipData{ Description = $"이 적은 5의 수치로 <color=red>공격</color>하려고 합니다.",
                                Position = m_tooltip_position};
    }
}
