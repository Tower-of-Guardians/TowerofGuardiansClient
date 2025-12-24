using System.Collections.Generic;
using UnityEngine;

public class MerchantUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("상인 UI")]
    [SerializeField] private MerchantView m_merchant_view;

    [Header("상점 UI")]
    [SerializeField] private MerchantShopView m_shop_view;

    [Header("품목 디스펜서")]
    [SerializeField] private MerchantShopDispenser m_shop_dispenser;

    [Header("대화 UI")]
    [SerializeField] private YarnDialogueUI m_dialogue_ui;

    [Header("알리미 UI")]
    [SerializeField] private Notice m_notice;

    [Space(30f), Header("카드 아이템 부모")]
    [SerializeField] private Transform m_card_root;

    [Header("회복 포션")]
    [SerializeField] private ShopPotionView m_potion_view;

    [Header("인벤토리 UI")]
    [SerializeField] private MerchantInventoryView m_inventory_view;

    [Header("인벤토리 카드 팩토리")]
    [SerializeField] private CardInventoryFactory m_card_factory;
    
    public void Inject()
    {
        InjectDispenser();
        InjectShop();
        InjectMerchant();
    }

    private void InjectDispenser()
    {
        var card_views = m_card_root.GetComponentsInChildren<IShopCardView>();
        var card_presenter_list = new List<ShopCardPresenter>();
        
        foreach(var card_view in card_views)
        {
            var card_presenter = new ShopCardPresenter(card_view, 
                                                       m_shop_dispenser);
            card_presenter_list.Add(card_presenter);
        }

        var potion_presenter = new ShopPotionPresenter(m_potion_view, 
                                                       m_shop_dispenser);
        DIContainer.Register<ShopPotionPresenter>(potion_presenter);

        m_shop_dispenser.Inject(card_presenter_list,
                                potion_presenter);

        var select_behavior = new SelectCardBehavior();
        var inventory_presenter = new MerchantInventoryPresenter(m_inventory_view, 
                                                                 m_card_factory,
                                                                 select_behavior,
                                                                 m_notice,
                                                                 DIContainer.Resolve<MerchantDialogueBubblePresenter>());
        DIContainer.Register<MerchantInventoryPresenter>(inventory_presenter);
    }

    private void InjectShop()
    {
        DIContainer.Register<IMerchantShopView>(m_shop_view);

        var inventory_presenter = DIContainer.Resolve<MerchantInventoryPresenter>();
        var shop_presenter = new MerchantShopPresenter(m_shop_view,
                                                       m_shop_dispenser,
                                                       inventory_presenter);
        DIContainer.Register<MerchantShopPresenter>(shop_presenter);

        inventory_presenter.Inject(shop_presenter);
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
