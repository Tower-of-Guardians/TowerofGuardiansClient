using DG.Tweening;
using UnityEngine;

public class FieldCardLayoutController : MonoBehaviour
{
    [Header("의존성 목록")]
    [Header("필드 뷰 디자이너")]
    [SerializeField] private FieldUIDesigner m_designer;

    [Header("프리뷰 오브젝트")]
    [SerializeField] private GameObject m_preview_object;

    private FieldCardContainer m_container;
    private FieldPresenter m_presenter;

    public void Inject(FieldCardContainer container,
                       FieldPresenter presenter)
    {
        m_container = container;
        m_presenter = presenter;
    }

    public void UpdateLayout(bool include_preview, bool is_anime = true, bool is_sorting = true)
    {
        var card_views = m_container.Cards;
        var card_count = include_preview ? card_views.Count + 1
                                         : card_views.Count;

        if(card_count == 0)
            return;

        var prev_preview_position = card_views.Count > 0 ? ((card_views[^1] as FieldCardView).transform as RectTransform).anchoredPosition
                                                          : CardLayoutCalculator.CalculatedFieldCardPosition(0, m_designer.ATKLimit, m_designer.Space); 
        
        for(int i = 0; i < card_views.Count; i++)
        {
            if(m_presenter.HoverCard == card_views[i])
                continue;
            
            var target_position = CardLayoutCalculator.CalculatedFieldCardPosition(i, m_designer.ATKLimit, m_designer.Space);
            var concrete_card = (card_views[i] as FieldCardView).transform as RectTransform;
            
            concrete_card.transform.DOKill();

            if(is_sorting || is_anime)
                concrete_card.DOAnchorPos(target_position, m_designer.AnimeDuration).SetEase(Ease.InOutSine);
            else
            {
                if(card_count - i > 1)
                    concrete_card.DOAnchorPos(target_position, m_designer.AnimeDuration).SetEase(Ease.InOutSine);
                else
                    concrete_card.anchoredPosition = target_position;
            }
        }        

        if(include_preview)
        {
            var preview_rect = m_preview_object.transform as RectTransform; 
            preview_rect.anchoredPosition = prev_preview_position;
            
            var preview_position = CardLayoutCalculator.CalculatedFieldCardPosition(card_count - 1, m_designer.ATKLimit, m_designer.Space);
            preview_rect.DOAnchorPos(preview_position, m_designer.AnimeDuration);            
        }
    }
}