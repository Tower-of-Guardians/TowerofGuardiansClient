using UnityEngine;

public class MerchantUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("상인 UI")]
    [SerializeField] private MerchantView m_merchant_view;

    [Header("상점 UI")]
    [SerializeField] private MerchantShopView m_shop_view;

    [Header("대화 UI")]
    [SerializeField] private YarnDialogueUI m_dialogue_ui;
    
    public void Inject()
    {
        InjectShop();
        InjectMerchant();
    }

    private void InjectShop()
    {
        DIContainer.Register<IMerchantShopView>(m_shop_view);

        var shop_presenter = new MerchantShopPresenter(m_shop_view);
        DIContainer.Register<MerchantShopPresenter>(shop_presenter);
    }

    private void InjectMerchant()
    {
        DIContainer.Register<IMerchantView>(m_merchant_view);

        var merchant_presenter = new MerchantPresenter(m_merchant_view,
                                                       DIContainer.Resolve<MerchantShopPresenter>(),
                                                       m_dialogue_ui);
        DIContainer.Register<MerchantPresenter>(merchant_presenter);
    }
}
