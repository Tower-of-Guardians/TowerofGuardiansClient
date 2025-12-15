using System.Collections;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Animation")]
    private Animator animator;
    [SerializeField] private string attack1TriggerName = "Attack1";
    [SerializeField] private string attack1StateName = "Player1_Attack1";
    [SerializeField] private string attack2TriggerName = "Attack2";
    [SerializeField] private string attack2StateName = "Player1_Attack2";
    [SerializeField] private string attack3TriggerName = "Attack3";
    [SerializeField] private string attack3StateName = "Player1_Attack3";

    [Header("Animation Settings")]
    [SerializeField] private float attackMotionDuration = 0.3f;

    [Header("Attack Thresholds")]
    [SerializeField] private int lightAttack = 10;
    [SerializeField] private int normalAttack = 20;

    private void Awake()
    {
        InitializeAnimator();
    }

    private void InitializeAnimator()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void PlayAttackAnimation(int attackValue)
    {
        if (animator == null)
        {
            return;
        }

        string triggerName = GetAttackTriggerName(attackValue);
        if (!string.IsNullOrEmpty(triggerName))
        {
            animator.SetTrigger(triggerName);
        }
    }

    public IEnumerator WaitForAttackAnimationComplete(int attackValue)
    {
        string stateName = GetAttackStateName(attackValue);
        
        if (animator == null || string.IsNullOrEmpty(stateName))
        {
            if (attackMotionDuration > 0f)
            {
                yield return new WaitForSeconds(attackMotionDuration);
            }
            yield break;
        }

        int attackStateHash = Animator.StringToHash(stateName);
        
        // Attack 상태로 전환될 때까지 대기
        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            if (stateInfo.shortNameHash == attackStateHash)
            {
                break;
            }
            
            yield return null;
        }
        
        // Attack 상태가 끝날 때까지 대기
        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            if (stateInfo.shortNameHash != attackStateHash)
            {
                break;
            }
            
            if (stateInfo.normalizedTime >= 1.0f)
            {
                break;
            }
            
            yield return null;
        }
    }

    private string GetAttackTriggerName(int attackValue)
    {
        if (attackValue < lightAttack)
        {
            return attack1TriggerName;
        }
        else if (attackValue < normalAttack)
        {
            return attack2TriggerName;
        }
        else
        {
            return attack3TriggerName;
        }
    }

    private string GetAttackStateName(int attackValue)
    {
        if (attackValue < lightAttack)
        {
            return attack1StateName;
        }
        else if (attackValue < normalAttack)
        {
            return attack2StateName;
        }
        else
        {
            return attack3StateName;
        }
    }
}

