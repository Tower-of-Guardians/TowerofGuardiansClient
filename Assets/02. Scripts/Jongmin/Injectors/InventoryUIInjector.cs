using UnityEngine;

public class InventoryUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("인벤토리 UI")]
    [SerializeField] private InventoryView m_inventory_view;

    [Header("카드 인벤토리 UI")]
    [SerializeField] private CardInventoryUI m_card_inventory_ui;

    [Header("인벤토리 정렬 뷰")]
    [SerializeField] private InventorySortView m_inven_sort_view;

    [Header("인벤토리 탭 뷰")]
    [SerializeField] private InventoryTabView m_inven_tab_view;

    public void Inject()
    {
        InjectTab();
        InjectInventory();
        InjectSort();
    }

    private void InjectInventory()
    {
        DIContainer.Register<IInventoryView>(m_inventory_view);

        var inventory_presenter = new InventoryPresenter(m_inventory_view,
                                                         DIContainer.Resolve<InventoryTabPresenter>());
        DIContainer.Register<InventoryPresenter>(inventory_presenter);
    }

    private void InjectSort()
    {
        DIContainer.Register<IInventorySortView>(m_inven_sort_view);

        var inven_sort_presenter = new InventorySortPresenter(m_inven_sort_view, 
                                                              m_card_inventory_ui);
        DIContainer.Register<InventorySortPresenter>(inven_sort_presenter);
    }

    private void InjectTab()
    {
        DIContainer.Register<IInventoryTabView>(m_inven_tab_view);

        var inven_tab_presenter = new InventoryTabPresenter(m_inven_tab_view,
                                                            m_card_inventory_ui);
        DIContainer.Register<InventoryTabPresenter>(inven_tab_presenter);
    }
}
