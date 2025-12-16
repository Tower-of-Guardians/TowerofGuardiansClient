public class InventorySortPresenter
{
    private readonly IInventorySortView m_view;
    private readonly CardInventoryUI m_card_inventory;

    private SortType m_current_sort_type = SortType.Time;
    private bool m_is_ascending = true;

    public InventorySortPresenter(IInventorySortView view,
                                  CardInventoryUI card_inventory)
    {
        m_view = view;
        m_card_inventory = card_inventory;

        m_view.Inject(this);
        Initialize();
    }

    private void Initialize()
        => UpdateSortText();

    private void SetPrevSortType()
    {
        m_current_sort_type = m_current_sort_type switch
        {
            SortType.Time       => SortType.Defense,
            SortType.Grade      => SortType.Time,
            SortType.Attack     => SortType.Grade,
            SortType.Defense    => SortType.Attack,
            _                   => SortType.Time
        };
    }

    private void SetNextSortType()
    {
        m_current_sort_type = m_current_sort_type switch
        {
            SortType.Time       => SortType.Grade,
            SortType.Grade      => SortType.Attack,
            SortType.Attack     => SortType.Defense,
            SortType.Defense    => SortType.Time,
            _                   => SortType.Time
        };
    }

    private void UpdateSortText()
    {
        string sort_text = m_current_sort_type switch
        {
            SortType.Time       => "획득순",
            SortType.Grade      => "등급순",
            SortType.Attack     => "공격력순",
            SortType.Defense    => "보호력순",
            _                   => "획득순"
        };        

        m_view.UpdateSortLabel(sort_text);
    }

    public void OnClickedLeftButton()
    {
        SetPrevSortType();
        UpdateSortText();
        ResetCriterion();
        CommitChange();
    }

    public void OnClickedRightButton()
    {
        SetNextSortType();
        UpdateSortText();
        ResetCriterion();
        CommitChange();
    }

    public void OnClickedCriterionButton()
    {
        m_is_ascending = !m_is_ascending;
        m_view.UpdateSortButton(m_is_ascending);
        CommitChange();
    }

    private void CommitChange()
    {   
        DataCenter.Instance.SortUserCards(m_current_sort_type);
        m_card_inventory.RefreshCardInventory();
    }

    private void ResetCriterion()
    {
        m_is_ascending = true;
        m_view.UpdateSortButton(m_is_ascending);
    }
}
