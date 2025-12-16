public class InventoryTabPresenter
{
    private readonly IInventoryTabView m_view;
    private readonly CardInventoryUI m_card_inventory;

    public InventoryTabPresenter(IInventoryTabView view,
                                 CardInventoryUI card_inventory)
    {
        m_view = view;
        m_card_inventory = card_inventory;

        m_view.Inject(this);
    }

    public void Initialize()
    {
        m_view.Initialize();
    }

    public void OnValueChangedCard(bool isOn)
    {
        if(isOn)
            m_card_inventory.OpenPanel();
        else
            m_card_inventory.ClosePanel();

        m_view.UpdateCardToggle(isOn);
    }

    public void OnValueChangedMagic(bool isOn)
    {
        // TODO: 마법 인벤토리 여닫이 기능 추가
        m_view.UpdateMagicToggle(isOn);
    }
}
