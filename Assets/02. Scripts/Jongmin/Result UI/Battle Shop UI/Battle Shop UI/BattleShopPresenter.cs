public class BattleShopPresenter
{
    private readonly IBattleShopView m_view;
    // TOOD: 카드에 필요한 의존성 변수

    public BattleShopPresenter(IBattleShopView view)
    {
        m_view = view;
        m_view.Inject(this);
    }

    public void InstantiateSlot()
    {
        var slot_view = m_view.InstantiateSlotView();
        var slot_presenter = new BattleShopSlotPresenter(slot_view);
    }

    public void OpenUI()
    {
        // TODO: 레벨에 따른 확률 정보 로드
        m_view.OpenUI();
    }

    public void CloseUI()
    {
        m_view.CloseUI();
    }
}
