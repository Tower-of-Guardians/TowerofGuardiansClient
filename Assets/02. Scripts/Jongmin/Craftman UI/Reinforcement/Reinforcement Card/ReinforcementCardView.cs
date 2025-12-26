using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ReinforcementCardView : CardView, IReinforcementCardView
{
    private Animator m_animator;

    private void Awake()
        => m_animator = GetComponent<Animator>();

    public void UpgradeATK(float atk)
    {
       m_card_atk_label.text = atk.ToString();
       m_animator.SetTrigger("ATK");
    }

    public void UpgradeBoth(float atk, float def)
    {
        m_card_atk_label.text = atk.ToString();
        m_card_def_label.text = def.ToString();
        m_animator.SetTrigger("Both");
    }

    public void UpgradeDEF(float def)
    {
        m_card_def_label.text = def.ToString();
        m_animator.SetTrigger("DEF");
    }
}
