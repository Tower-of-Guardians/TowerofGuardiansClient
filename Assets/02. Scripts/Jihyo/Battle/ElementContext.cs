using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 파이프라인 요소들이 사용할 의존성들을 담는 컨텍스트 클래스
/// DI Container에서 의존성 주입을 받아 BattleManager에 제공
/// </summary>
public class ElementContext
{
    public TurnManager turn_manager { get; set; }
    public FieldPresenter field_presenter { get; set; }
    public HandPresenter hand_presenter { get; set; }
    public BattleActionController battle_action_controller { get; set; }
    public BattleSetupController setup_controller { get; set; }
    public BattleManager battle_manager { get; set; }
    
    /// <summary>
    /// 전투 상태 정보 (파이프라인 요소들 간 상태 공유용)
    /// </summary>
    public CombatState combat_state { get; set; }
}

/// <summary>
/// 전투 중 상태를 저장하는 클래스
/// </summary>
public class CombatState
{
    public List<IDamageable> player_targets { get; set; }
    public Monster primary_monster_target { get; set; }
    public Vector3? attack_anchor_position { get; set; }
    public int current_attack { get; set; }
    public bool player_attack_hits_all { get; set; }
}