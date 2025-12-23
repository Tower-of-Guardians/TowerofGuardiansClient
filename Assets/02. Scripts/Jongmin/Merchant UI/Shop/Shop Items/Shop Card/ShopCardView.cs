using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCardView : CardView, IShopCardView
{
    [Space(30f), Header("추가 UI 관련 컴포넌트")]
    [Header("카드 가격 텍스트")]
    [SerializeField] private TMP_Text m_cost_label;

    [Header("구매 버튼")]
    [SerializeField] private Button m_purchase_button;

    [Header("구매 완료 이미지")]
    [SerializeField] private GameObject m_already_purchased_image;

    private ShopCardPresenter m_presenter;

    private void OnDisable()
        => m_presenter?.Dispose();

    public void Inject(ShopCardPresenter presenter)
    {
        m_presenter = presenter;

        m_purchase_button.onClick.AddListener(m_presenter.OnClickedPurchase);
    }

    public void InitUI(ShopCardData card_data, bool can_purchase)
    {
        InitUI(card_data.Card.data);   
        SetPurchaseButtonAlpha(1f);   
        m_already_purchased_image.SetActive(false);  

        m_cost_label.text = can_purchase ? $"{card_data.Cost}G"
                                         : $"<color=red>{card_data.Cost}G</color>";

        m_purchase_button.interactable = can_purchase;
    }

    public void UpdateUI(ShopCardData card_data, bool can_purchase)
    {
        if(m_presenter.Purchased)
        {
            m_cost_label.text = string.Empty;
            
            m_purchase_button.interactable = false;
            SetPurchaseButtonAlpha(0f);

            m_already_purchased_image.SetActive(m_presenter.Purchased);
        }
        else
        {
            InitUI(card_data, can_purchase);
        }
    }

    private void SetPurchaseButtonAlpha(float alpha)
    {
        var color = m_purchase_button.image.color;
        color.a = alpha;
        m_purchase_button.image.color = color;
    }
}
