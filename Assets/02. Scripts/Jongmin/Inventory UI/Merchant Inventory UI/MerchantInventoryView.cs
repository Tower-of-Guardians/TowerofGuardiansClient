using UnityEngine;
using UnityEngine.UI;

public class MerchantInventoryView : CardInventoryView
{
    [Header("UI 관련 컴포넌트")]
    [Header("판매 버튼")]
    [SerializeField] private Button m_sale_button;

    [Header("상점 버튼")]
    [SerializeField] private Button m_back_button;

    private MerchantInventoryPresenter m_presenter;

    public override void Inject(CardInventoryPresenter presenter)
    {
        m_presenter = presenter as MerchantInventoryPresenter;

        m_sale_button.onClick.AddListener(m_presenter.OnClickedSale);
        m_back_button.onClick.AddListener(m_presenter.OnClickedBack);
    }
}