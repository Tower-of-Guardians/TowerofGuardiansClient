using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ReinforcementCardView : CardUI, IReinforcementCardView
{
    private Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void UpgradeATK(float atk)
    {
       UpdateATKTextLabel(atk);
       m_animator.SetTrigger("ATK");
    }

    public void UpgradeBoth(float atk, float def)
    {
        UpdateATKTextLabel(atk);
        UpdateDEFTextLabel(def);
        m_animator.SetTrigger("Both");
    }

    public void UpgradeDEF(float def)
    {
        UpdateDEFTextLabel(def);
        m_animator.SetTrigger("DEF");
    }

    private void UpdateATKTextLabel(float atk)
        => _cardATKLabel.text = atk.ToString();

    private void UpdateDEFTextLabel(float def)
        => _cardDEFLabel.text = def.ToString();
}
