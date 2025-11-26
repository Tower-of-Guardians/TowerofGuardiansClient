using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LayoutThrowView : MonoBehaviour, IThrowView
{
    [Header("디자이너")]
    [SerializeField] private ThrowUIDesigner m_designer;

    [Space(30f), Header("UI 관련 컴포넌트")]
    [Header("캔버스")]
    [SerializeField] private Canvas m_canvas;

    [Header("슬롯의 부모 트랜스폼")]
    [SerializeField] private Transform m_slot_root;

    [Header("프리뷰 카드")]
    [SerializeField] private GameObject m_preview_object;

    [Header("UI 열기 버튼")]
    [SerializeField] private Button m_open_button;

    [Header("UI 닫기 버튼")]
    [SerializeField] private Button m_close_button;

    [Header("버리기 버튼")]
    [SerializeField] private Button m_throw_button;

    [Space(30f), Header("의존성 목록")]
    [Header("교체 애니메이터")]
    [SerializeField] private Animator m_animator;

    [Header("임시 카드 컨트롤러")]
    [SerializeField] private TemporaryCardController m_temp_card_controller;

    [Header("교체 카드 프리펩")]
    [SerializeField] private GameObject m_throw_card_prefab;

    [Header("팝업 알리미 프리펩")]
    [SerializeField] private GameObject m_popup_notice_prefab;

    [Header("카드 패 뷰")]
    [SerializeField] private Transform m_hand_view_transform;

    [Header("교체 카드 버튼")]
    [SerializeField] private Transform m_throw_button_transform;

    private ThrowPresenter m_presenter;

    private void OnDestroy()
    {
        if(m_temp_card_controller != null && m_presenter != null)
            m_temp_card_controller.OnAnimationEnd -= m_presenter.OnTempCardAnimeEnd;

        m_presenter?.Dispose();
    }

    public void Inject(ThrowPresenter presenter)
    {
        m_presenter = presenter;

        m_open_button.onClick.AddListener(m_presenter.OnClickedOpenUI);
        m_close_button.onClick.AddListener(m_presenter.OnClickedCloseUI);
        m_throw_button.onClick.AddListener(m_presenter.OnClickedThrowCards);

        m_temp_card_controller.OnAnimationEnd += m_presenter.OnTempCardAnimeEnd;
    }

    public void OpenUI()
        => ToggleAnime(true);

    public void CloseUI()
    {
        ToggleAnime(false);

        var throw_cards = m_slot_root.GetComponentsInChildren<IThrowCardView>();
        var throw_card_positions = new List<Vector3>();
        foreach(var throw_card in throw_cards)
            throw_card_positions.Add((throw_card as ThrowCardView).transform.position);

        m_temp_card_controller.PlayAnimeFromThis(m_presenter.GetCardDatas(),
                                                 throw_card_positions.ToArray(),
                                                 m_hand_view_transform.position,
                                                 0.75f,
                                                 100f,
                                                 0.5f,
                                                 0.1f);
    }

    public void UpdateUI(bool open_active, bool throw_active)
    {
        m_open_button.interactable = open_active;
        m_throw_button.interactable = throw_active;
    }

    public void ThrowUI()
    {
        ToggleAnime(false); 

        var throw_cards = m_slot_root.GetComponentsInChildren<IThrowCardView>();
        var throw_card_positions = new List<Vector3>();
        foreach(var throw_card in throw_cards)
            throw_card_positions.Add((throw_card as ThrowCardView).transform.position);

        m_temp_card_controller.PlayAnimeFromThis(m_presenter.GetCardDatas(),
                                                 throw_card_positions.ToArray(),
                                                 m_throw_button_transform.position,
                                                 0.35f,
                                                 0f,
                                                 0.75f,
                                                 0.1f);    
    }

    private void ToggleAnime(bool active)
        => m_animator.SetBool("Open", active);

    public void ToggleManual(bool active)
    {
        UpdateLayout(active, !active);
        m_preview_object.SetActive(active);
    }

    public IThrowCardView InstantiateCardView()
    {
        var card_obj = ObjectPoolManager.Instance.Get(m_throw_card_prefab);
        card_obj.transform.SetParent(m_slot_root, false); 
        card_obj.transform.localScale = Vector3.one;

        var card_view = card_obj.GetComponent<IThrowCardView>();
        card_view.OnBeginDragAction     += ()           => { OnBeginDragCard(card_view); };
        card_view.OnDragAction          += (position)   => { OnDragCard(position); };
        card_view.OnEndDragAction       += ()           => { OnEndDragCard(); };        

        UpdateLayout(false, false);
        
        return card_view;
    }

    public void ReturnCard(IThrowCardView card_view, CardData card_data)
    {
        var concrete_card = card_view as ThrowCardView;
        
        m_temp_card_controller.PlayAnime(card_data,
                                         concrete_card.transform.position,
                                         m_hand_view_transform.position,
                                         1f,
                                         0f,
                                         0f);  

        ObjectPoolManager.Instance.Return(concrete_card.gameObject);

        UpdateLayout(false, true);
    }

    public void ReturnCards()
    {
        var card_views = m_slot_root.GetComponentsInChildren<IThrowCardView>();
        foreach(var card_view in card_views)
        {
            var card_obj = (card_view as ThrowCardView).gameObject;
            ObjectPoolManager.Instance.Return(card_obj);
        }        
    }

    public void PrintNotice(string notice_text)
    {
        var popup_notice_obj = ObjectPoolManager.Instance.Get(m_popup_notice_prefab);

        var popup_notice_ui = popup_notice_obj.GetComponent<IPopupNoticeView>();
        popup_notice_ui.OpenUI(notice_text);
    }

    private void UpdateLayout(bool include_preview, bool is_removing)
    {
        var card_views = m_slot_root.GetComponentsInChildren<IThrowCardView>();
        var card_count = include_preview ? card_views.Length + 1
                                         : card_views.Length;

        if(card_count == 0)
            return;

        var prev_preview_position = card_views.Length > 0 ? ((card_views[^1] as ThrowCardView).transform as RectTransform).anchoredPosition
                                                          : Vector2.zero; 

        var total_width = (card_count - 1) * m_designer.Space;
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

#region Events
    public void OnBeginDragCard(IThrowCardView card_view)
    {
        m_presenter.HoverCard = card_view;
        
        var target_card = card_view as ThrowCardView;
        target_card.transform.DOKill();
        target_card.transform.SetParent(m_canvas.transform, false);
    }

    public void OnDragCard(Vector2 position)
    {
        if(m_presenter.HoverCard == null)
            return;

        var target_card = m_presenter.HoverCard as ThrowCardView;
        target_card.transform.position = position;
    }

    public void OnEndDragCard()
    {
        if(m_presenter.HoverCard == null)
            return;

        var target_card = m_presenter.HoverCard as ThrowCardView;
        target_card.transform.SetParent(m_slot_root, false);

        var hit = CheckField(out var pointer_data);
        var drop_handler = hit?.gameObject.GetComponent<IDropHandler>();
        if(drop_handler != null)
            ExecuteEvents.Execute(hit?.gameObject, pointer_data, ExecuteEvents.dropHandler);

        UpdateLayout(false, true);
    }


    public void OnDrop(PointerEventData eventData)
    {
        var dropped_object = eventData.pointerDrag;
        if(dropped_object != null)
        {
            var card_view = dropped_object.GetComponent<IHandCardView>();
            if(card_view != null)
                m_presenter.OnDroped(card_view);
        }

        ToggleManual(false);
        m_preview_object.transform.SetAsFirstSibling();
    }

    private RaycastResult? CheckField(out PointerEventData pointer_data)
    {
        pointer_data = new PointerEventData(EventSystem.current);
        pointer_data.position = Input.mousePosition;
        pointer_data.pointerDrag = (m_presenter.HoverCard as ThrowCardView).gameObject;

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
}