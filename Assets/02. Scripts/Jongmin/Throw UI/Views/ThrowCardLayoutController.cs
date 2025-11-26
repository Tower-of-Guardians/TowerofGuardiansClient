using UnityEngine;
using DG.Tweening;

public class ThrowCardLayoutController : MonoBehaviour
{
    [Header("의존성 목록")]
    [Header("교체 뷰 디자이너")]
    [SerializeField] private ThrowUIDesigner m_designer;

    [Header("프리뷰 카드")]
    [SerializeField] private GameObject m_preview_object;

    public void UpdateLayout(Transform root, bool include_preview, bool is_removing)
    {
        var card_views = root.GetComponentsInChildren<IThrowCardView>();
        var card_count = include_preview ? card_views.Length + 1
                                         : card_views.Length;

        if(card_count == 0)
            return;

        var prev_preview_position = card_views.Length > 0 ? ((card_views[^1] as ThrowCardView).transform as RectTransform).anchoredPosition
                                                          : Vector2.zero; 

        for(int i = 0; i < card_views.Length; i++)
        {
            var target_position = CardLayoutCalculator.CalculatedThrowCardPosition(i, card_count, m_designer.Space);
            var concrete_card = (card_views[i] as ThrowCardView).transform as RectTransform;

            if(!is_removing)
                if(card_count - i > 1)
                    concrete_card.DOAnchorPos(target_position, m_designer.AnimeDuration).SetEase(Ease.InOutSine);
                else
                    concrete_card.anchoredPosition = target_position;
            else
                concrete_card.DOAnchorPos(target_position, m_designer.AnimeDuration).SetEase(Ease.InOutSine);

        }

        if(include_preview)
        {
            var preview_rect = m_preview_object.transform as RectTransform; 
            preview_rect.anchoredPosition = prev_preview_position;
            
            var preview_position = CardLayoutCalculator.CalculatedThrowCardPosition(card_count - 1, card_count, m_designer.Space);
            preview_rect.DOAnchorPos(preview_position, m_designer.AnimeDuration);
        }        
    }
}
