using UnityEngine;
using UnityEngine.EventSystems;

public class FieldCardEventController : MonoBehaviour, IDropHandler
{
    [Header("의존성 목록")]
    [Header("캔버스")]
    [SerializeField] private Canvas m_canvas;

    [Header("카드의 부모 트랜스폼")]
    [SerializeField] private Transform m_slot_root;

    [Header("프리뷰 오브젝트")]
    [SerializeField] private GameObject m_preview_object;

    private IFieldView m_view;
    private FieldPresenter m_presenter;

    public void Inject(IFieldView view,
                       FieldPresenter presenter)
    {
        m_view = view;
        m_presenter = presenter;
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

        m_presenter.ToggleManual(false);
        m_preview_object.transform.SetAsLastSibling();
    }
}
