using UnityEngine;
using UnityEngine.EventSystems;

public class FieldView : MonoBehaviour, IFieldView
{
    [Header("UI 관련 컴포넌트")]
    [Header("카드의 부모 트랜스폼")]
    [SerializeField] private Transform m_slot_root;

    [Header("프리뷰 오브젝트")]
    [SerializeField] private GameObject m_preview_object;

    [Space(30f), Header("의존성 목록")]
    [Header("필드 카드 프리펩")]
    [SerializeField] private GameObject m_card_prefab; 

    [Header("팝업 알리미 프리펩")]
    [SerializeField] private GameObject m_popup_notice_prefab;

    private FieldPresenter m_presenter;

    private void OnDestroy()
        => m_presenter?.Dispose();

    public void Inject(FieldPresenter presenter)
    {
        m_presenter = presenter;
    }

    public IFieldCardView InstantiateCardView()
    {
        var card_obj = ObjectPoolManager.Instance.Get(m_card_prefab);
        card_obj.transform.SetParent(m_slot_root, false);
        
        return card_obj.GetComponent<IFieldCardView>();
    }

    public void PrintNotice(string notice_text)
    {
        var popup_notice_obj = ObjectPoolManager.Instance.Get(m_popup_notice_prefab);
        
        var popup_notice_ui = popup_notice_obj.GetComponent<IPopupNoticeView>();
        popup_notice_ui.OpenUI(notice_text);
    }

    public void ReturnCards()
    {
        var card_views = m_slot_root.GetComponentsInChildren<IFieldCardView>();

        foreach(var card_view in card_views)
        {
            var card_obj = (card_view as FieldCardView).gameObject;
            ObjectPoolManager.Instance.Return(card_obj);
        }
    }

    public void ToggleManual(bool active)
    {
        m_preview_object.SetActive(active);
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
        m_preview_object.transform.SetAsLastSibling();
    }
}
