using System;
using UnityEngine;
using DG.Tweening;

public class TemporaryCardAnimator : MonoBehaviour
{
    [Header("임시 카드 팩토리")]
    [SerializeField] private TemporaryCardFactory m_factory;

    public event Action<CardData> OnAnimationEnd;

    public void Animate(CardData card_data,
                        Vector3 start_position,
                        Vector3 end_position,
                        float scale,
                        float arc_power,
                        float duration)
    {
        var temp_card = m_factory.InstantiateCard(card_data);
        temp_card.transform.position = start_position;
        temp_card.transform.localScale = Vector3.forward;

        var sequence_animator = DOTween.Sequence();
        sequence_animator.Append(temp_card.transform.DOScale(scale, duration).SetEase(Ease.OutBack));
        sequence_animator.Join(temp_card.transform.DOJump(end_position, arc_power, 1, duration).SetEase(Ease.InQuad));
        sequence_animator.OnComplete(() => { OnAnimationEnd?.Invoke(card_data); 
                                             ReturnCard(temp_card); });
    }

    private void ReturnCard(GameObject temp_card)
    {
        ObjectPoolManager.Instance.Return(temp_card);
    }
}