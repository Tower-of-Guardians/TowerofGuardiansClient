using UnityEngine;
using UnityEngine.UI;

public class MerchantInventoryView : MonoBehaviour, IDeckInvenUI
{
    [Header("UI 관련 컴포넌트")]
    [Header("판매 버튼")]
    [SerializeField] private Button m_sale_button;

    [Header("상점 버튼")]
    [SerializeField] private Button m_back_button;

    private MerchantDeckInvenPresenter _merchantDeckInvenPresenter;

    public void Construct(DeckInvenPresenter deckInvenPresenter)
    {
        _merchantDeckInvenPresenter = deckInvenPresenter as MerchantDeckInvenPresenter;

        m_sale_button.onClick.AddListener(_merchantDeckInvenPresenter.OnClickedSale);
        m_back_button.onClick.AddListener(_merchantDeckInvenPresenter.OnClickedBack);
    }

    public void OpenUI()
    {
        throw new System.NotImplementedException();
    }

    public void CloseUI()
    {
        throw new System.NotImplementedException();
    }
}