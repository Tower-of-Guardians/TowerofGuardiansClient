using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandView : MonoBehaviour, IHandView
{
    [Header("디자이너")]
    [SerializeField] private HandUIDesigner m_designer;

    [Space(30f), Header("UI 관련 컴포넌트")]
    [Header("캔버스 그룹")]
    [SerializeField] private CanvasGroup m_canvas_group;

    [Space(30f), Header("에디터 테스트 컴포넌트")]
    [Header("테스트 획득 버튼")]
    [SerializeField] private Button m_add_button;

    [Header("테스트 제거 버튼")]
    [SerializeField] private Button m_remove_button;

    [Space(30f), Header("의존성 목록")]
    [Header("카드 프리펩")]
    [SerializeField] private GameObject m_card_prefab;

    private HandPresenter m_presenter;

    public IHandCardView InstantiateCardView()
    {
        var card_obj = ObjectPoolManager.Instance.Get(m_card_prefab);
        card_obj.transform.SetParent(transform, false);
        
        var card_view = card_obj.GetComponent<IHandCardView>();
        card_view.OnPointerEnterAction  += ()           => { OnPointerEnterInCard(card_view); };
        card_view.OnPointerExitAction   += ()           => { OnPointerExitFromCard(); };
        card_view.OnBeginDragAction     += ()           => { OnBeginDragCard(); };
        card_view.OnDragAction          += (position)   => { OnDragCard(position); };
        card_view.OnEndDragAction       += ()           => { OnEndDragCard(); };

        return card_view; 
    }

    public void Inject(HandPresenter presenter)
    {
        m_presenter = presenter;

        m_add_button.onClick.AddListener(() => { m_presenter.InstantiateCard(null); });
        m_remove_button.onClick.AddListener(Test_RemoveCard);
    }

    public void OpenUI() => ToggleUI(true);
    public void CloseUI() => ToggleUI(false);

    private void ToggleUI(bool active)
    {
        m_canvas_group.alpha = active ? 1f : 0f;
        m_canvas_group.interactable = active;
        m_canvas_group.blocksRaycasts = active;
    }

    public void UpdateUI()
    {
        GetChildrenRectTransform();
        UpdateLayout();
    }

    private void RebuildSiblingOrder()
    {
        for (int i = 0; i < m_presenter.Cards.Count; i++)
            (m_presenter.Cards[i] as HandCardView).transform.SetSiblingIndex(i); 
    }

    private void GetChildrenRectTransform()
    {
        var card_array = transform.GetComponentsInChildren<HandCardView>(false);
        m_presenter.Cards = new(card_array);
    }

    private void UpdateLayout()
    {
        var card_count = m_presenter.Cards.Count;
        if(card_count == 0) 
            return;

        for (int i = 0; i < card_count; i++)
        {
            var card_view = m_presenter.Cards[i];

            var target_transform = CardLayoutCalculator.CalculatedTransform(i, 
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
            var hoverd_index = m_presenter.Cards.IndexOf(m_presenter.HoverCard);

            var offset = index < hoverd_index ? -m_designer.Strength 
                                              :  m_designer.Strength;
            target.Position.x += offset;
        }
    }

    private void AnimateCardTransform(CardLayoutData target,
                                      IHandCardView card)
    {
        var concrete_card = card as HandCardView; 

        concrete_card.transform.DOLocalMove(new Vector3(target.Position.x, 
                                                        card == m_presenter.HoverCard ? m_designer.HoverY 
                                                                                      : target.Position.y, 
                                                        target.Position.z), m_designer.AnimeSPD).SetEase(Ease.OutBack);
        concrete_card.transform.DOLocalRotate(target.Rotation, m_designer.AnimeSPD).SetEase(Ease.OutBack);
        concrete_card.transform.DOScale(target.Scale, m_designer.AnimeSPD).SetEase(Ease.OutBack);
    }

    #region Events
    private void OnPointerEnterInCard(IHandCardView card_view)
    {
        m_presenter.OnPointerEnter(card_view);
        UpdateLayout();
    }

    private void OnPointerExitFromCard()
    {
        m_presenter.OnPointerExit();
        UpdateLayout();
    }

    private void OnBeginDragCard() {}

    private void OnDragCard(Vector2 position)
    {
        if(m_presenter.HoverCard == null)
            return;

        var target_card = m_presenter.HoverCard as HandCardView;
        var hit = CheckField(out var pointer_data);
        var drop_handler = hit?.gameObject.GetComponent<IDropHandler>();

        if(drop_handler != null)
            target_card.transform.DOScale(0.75f, m_designer.AnimeSPD);
        else
            target_card.transform.DOScale(m_designer.Scale, m_designer.AnimeSPD);

        m_presenter.ToggleFieldPreview(true);
        
        target_card.transform.position = position;
    }

    private void OnEndDragCard()
    {
        if(m_presenter.HoverCard == null)
            return;

        var hit = CheckField(out var pointer_data);
        var drop_handler = hit?.gameObject.GetComponent<IDropHandler>();
        if(drop_handler != null)
            ExecuteEvents.Execute(hit?.gameObject, pointer_data, ExecuteEvents.dropHandler);
            
        m_presenter.ToggleFieldPreview(false);

        UpdateLayout();
    }

    private RaycastResult? CheckField(out PointerEventData pointer_data)
    {
        pointer_data = new PointerEventData(EventSystem.current);
        pointer_data.position = Input.mousePosition;
        pointer_data.pointerDrag = (m_presenter.HoverCard as HandCardView).gameObject;

        var ray_hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer_data, ray_hits);

        foreach(var hit in ray_hits)
        {
            var drop_handler = hit.gameObject.GetComponent<IDropHandler>();
            if(drop_handler != null)
                return hit; 
        } 

        return null;
    }
    #endregion Events

#region Test
    private void Test_RemoveCard()
    {
        var target = m_presenter.Test_RemoveCard();
        var concrete_target = target as HandCardView;

        if(concrete_target != null)
        {
            concrete_target.transform.DOKill();
            ObjectPoolManager.Instance.Return(concrete_target.gameObject);
        }

        UpdateUI();
    }
#endregion
}