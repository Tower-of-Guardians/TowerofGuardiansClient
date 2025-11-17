using UnityEngine;

[RequireComponent(typeof(PlayerUnit))]
public class PlayerDescriptor : BaseDescriptor
{
    private PlayerUnit m_player_unit;

    private void Awake()
    {
        m_player_unit = GetComponent<PlayerUnit>();
    }

    public override TooltipData GetTooltipData()
    {
        return new TooltipData{ Description = $"현재 {m_player_unit.Stats.Attack}의 기본 공격력을 가지고 있습니다.",
                                Position = m_tooltip_position};
    }
}
