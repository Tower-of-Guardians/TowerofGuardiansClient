using DG.Tweening;
using UnityEngine;

public class HandCardLayoutController : MonoBehaviour
{
    [Header("의존성 목록")]
    [Header("핸드 뷰 디자이너")]
    [SerializeField] private HandUIDesigner m_designer;

    [Header("프리뷰 카드")]
    [SerializeField] private GameObject m_preview_object;

    private HandCardContainer m_container;
    private HandPresenter m_presenter;

    public void Inject(HandCardContainer container,
                       HandPresenter presenter)
    {
        m_container = container;
        m_presenter = presenter;
    }

    public void UpdateLayout()
    {
        var card_count = m_container.Cards.Count;
        if(card_count == 0) 
            return;

        for (int i = 0; i < card_count; i++)
        {
            var card_view = m_container.Cards[i];

            var target_transform = CardLayoutCalculator.CalculatedHandCardTransform(i, 
                                                                                    card_count,
                                                                                    m_designer.Radius,
                                                                                    m_designer.Angle,
                                                                                    m_designer.Depth);

            ApplyHoverEffect(target_transform, card_view, i);
            AnimateCardTransform(target_transform, card_view);
        }

        if (m_presenter.HoverCard == null)
            RebuildSiblingOrder();
    }

    private void RebuildSiblingOrder()
    {
        for (int i = 0; i < m_container.Cards.Count; i++)
            (m_container.Cards[i] as HandCardView).transform.SetSiblingIndex(i); 
    }

    private void ApplyHoverEffect(CardLayoutData target,
                                  IHandCardView card_view,
                                  int index)
    {
        if (card_view == m_presenter.HoverCard)
        {
            target.Scale = Vector3.one * m_designer.Scale;
            target.Rotation = Vector3.zero;

            (card_view as HandCardView).transform.SetAsLastSibling();
        }
        else if (m_presenter.HoverCard != null)
        {
            var hoverd_index = m_container.GetIndex(m_presenter.HoverCard);

            var offset = index < hoverd_index ? -m_designer.Strength 
                                              :  m_designer.Strength;
            target.Position.x += offset;
        }
    }

    private void AnimateCardTransform(CardLayoutData target,
                                      IHandCardView card)
    {
        var concrete_card = card as HandCardView; 

        concrete_card.transform.DOKill();
        concrete_card.transform.DOLocalMove(new Vector3(target.Position.x, 
                                                        card == m_presenter.HoverCard ? m_designer.HoverY 
                                                                                      : target.Position.y, 
                                                        target.Position.z), m_designer.AnimeSPD).SetEase(Ease.OutBack);
        concrete_card.transform.DOLocalRotate(target.Rotation, m_designer.AnimeSPD).SetEase(Ease.OutBack);
        concrete_card.transform.DOScale(target.Scale, m_designer.AnimeSPD).SetEase(Ease.OutBack);
    }
}
