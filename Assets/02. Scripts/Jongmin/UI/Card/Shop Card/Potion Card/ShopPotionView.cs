using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPotionView : MonoBehaviour, IShopPotionView
{
    [Header("UI 관련 컴포넌트")]
    [Header("구매 완료 이미지")]
    [SerializeField] private GameObject m_already_purchased_image;

    [Header("구매 버튼")]
    [SerializeField] private Button m_purchase_button;

    [Header("구매 버튼 텍스트")]
    [SerializeField] private TMP_Text m_cost_label;

    private ShopPotionPresenter m_presenter;

    private void OnDisable()
        => m_presenter?.Dispose();

    public void Inject(ShopPotionPresenter presenter)
    {
        m_presenter = presenter;

        m_purchase_button.onClick.AddListener(m_presenter.OnClickedPurchase);
    }

    public void InitUI(int cost, bool can_purchase)
    {
        m_already_purchased_image.SetActive(false);

        m_cost_label.text = can_purchase ? $"{cost}G"
                                         : $"<color=red>{cost}G</color>";

        m_purchase_button.interactable = can_purchase;
        SetPurchaseButtonAlpha(1f);
    }

    public void UpdateUI(int cost, bool can_purchase)
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
            InitUI(cost, can_purchase);
        }
    }

    private void SetPurchaseButtonAlpha(float alpha)
    {
        var color = m_purchase_button.image.color;
        color.a = alpha;
        m_purchase_button.image.color = color;
    }
}
