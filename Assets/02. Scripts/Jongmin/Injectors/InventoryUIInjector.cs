using UnityEngine;

public class InventoryUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("인벤토리 UI")]
    [SerializeField] private CardInventoryUI m_card_inventory_ui;

    [Header("인벤토리 정렬 뷰")]
    [SerializeField] private InventorySortView m_inven_sort_view;

    public void Inject()
    {
        InjectSort();
    }

    private void InjectSort()
    {
        DIContainer.Register<IInventorySortView>(m_inven_sort_view);

        var inven_sort_presenter = new InventorySortPresenter(m_inven_sort_view, m_card_inventory_ui);
        DIContainer.Register<InventorySortPresenter>(inven_sort_presenter);
    }
}
