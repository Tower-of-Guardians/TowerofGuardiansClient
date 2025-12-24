using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryCardView : CardView, IInventoryCardView, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("선택 완료 이미지")]
    [SerializeField] private GameObject m_selected_image;

    protected InventoryCardPresenter m_presenter;

    private void OnDisable()
        => m_presenter?.DeselectRequest();

    public void Inject(InventoryCardPresenter presenter)
        => m_presenter = presenter;

    public void ShowHighlight(bool active)
        => m_selected_image.SetActive(active);

    public virtual void OnPointerClick(PointerEventData eventData)
        => m_presenter.OnClick();

    public virtual void OnPointerEnter(PointerEventData eventData)
        => m_presenter.OnPointerEnter();

    public virtual void OnPointerExit(PointerEventData eventData)
        => m_presenter.OnPointerExit();
}
