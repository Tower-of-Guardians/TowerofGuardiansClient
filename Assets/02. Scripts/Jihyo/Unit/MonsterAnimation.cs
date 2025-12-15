using UnityEngine;

public class MonsterAnimation : MonoBehaviour
{
    [Header("Animation")]
    private Animator animator;

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
    }
}

