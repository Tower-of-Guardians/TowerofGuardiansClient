using System.Collections;
using UnityEngine;

public class MonsterAnimation : MonoBehaviour
{
    [Header("Animation")]
    private Animator animator;

    [Header("Animator Parameters")]
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int HitHash = Animator.StringToHash("Hit");

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

    public void PlayAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(AttackHash);
        }
    }

    public void PlayHitAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(HitHash);
        }
    }

    public void ResetAnimationState()
    {
        if (animator != null)
        {
            animator.ResetTrigger(AttackHash);
            animator.ResetTrigger(HitHash);
        }
    }

    /// <summary>
    /// 공격 애니메이션이 완료될 때까지 대기합니다.
    /// </summary>
    public IEnumerator WaitForAttackAnimationComplete()
    {
        if (animator == null)
        {
            yield break;
        }

        float transitionTimeout = 1f;
        float elapsedTime = 0f;
        bool attackStateFound = false;

        while (elapsedTime < transitionTimeout)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            if (stateInfo.IsName("Attack"))
            {
                attackStateFound = true;
                break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!attackStateFound)
        {
            // Attack 상태를 찾지 못했으면 바로 종료
            yield break;
        }

        int previousStateHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;

        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            int currentStateHash = stateInfo.fullPathHash;

            if (currentStateHash != previousStateHash && !stateInfo.IsName("Attack"))
            {
                break;
            }

            if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1.0f)
            {
                yield return null;
                break;
            }

            previousStateHash = currentStateHash;
            yield return null;
        }
    }
}

