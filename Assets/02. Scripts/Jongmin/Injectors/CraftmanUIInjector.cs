// using UnityEngine;
// using UnityEngine.Serialization;
//
// public class CraftmanUIInjector : MonoBehaviour, IInjector
// {
//     [FormerlySerializedAs("m_craftman_view")]
//     [Header("의존성 목록")]
//     [Header("대장장이 UI")]
//     [SerializeField] private CraftmanUI mCraftmanUI;
//
//     [Header("인벤토리 UI")]
//     [SerializeField] private CraftmanDeckInvenUI m_inventory_view;
//
//     [Header("인벤토리 카드 팩토리")]
//     [SerializeField] private DeckInvenFactory m_card_factory;
//
//     [Header("강화 UI")]
//     [SerializeField] private ForgeUI m_reinforcement_view;
//
//     [FormerlySerializedAs("m_card_view")]
//     [Header("강화 카드 UI")]
//     [SerializeField] private ForgeCardUI mCardUI;
//
//     [Header("강화 데이터베이스")]
//     [SerializeField] private ForgeDatabase m_reinforcement_db;
//
//     [Header("알리미 UI")]
//     [SerializeField] private INotifierUI m_notice;
//
//     public void Inject()
//     {
//         InjectCard();
//         InjectDB();
//         InjectReinforcement();
//         InjectInventory();
//         InjectCraftman();
//     }
//
//     private void InjectCard()
//     {
//         DIContainer.Register<ForgeCardUI>(mCardUI);
//
//         var reinforcement_card_presenter = new ReinforcementCardPresenter(mCardUI);
//         DIContainer.Register<ReinforcementCardPresenter>(reinforcement_card_presenter);
//     }
//
//     private void InjectDB()
//         => DIContainer.Register<IForgeDatabase>(m_reinforcement_db);
//
//     private void InjectReinforcement()
//     {
//         DIContainer.Register<IForgeUI>(m_reinforcement_view);
//
//         var reinforcement_presenter = new ForgePresenter(m_reinforcement_view,
//                                                                  DIContainer.Resolve<ReinforcementCardPresenter>(),
//                                                                  m_reinforcement_db);
//         DIContainer.Register<ForgePresenter>(reinforcement_presenter);
//     }
//
//     private void InjectInventory()
//     {
//         var reinforcement_presenter = DIContainer.Resolve<ForgePresenter>();
//         var selection_behavior = new SelectCardBehavior();
//         var inventory_presenter = new CraftmanDeckInvenPresenter(m_inventory_view,
//                                                                  m_card_factory,
//                                                                  new CardContainer<IDeckInvenCardUI, DeckInvenCardPresenter>(),
//                                                                  selection_behavior,
//                                                                  DIContainer.Resolve<CraftmanDialogueBubblePresenter>(),
//                                                                  reinforcement_presenter);
//         DIContainer.Register<CraftmanDeckInvenPresenter>(inventory_presenter);
//
//         
//     }
//
//     private void InjectCraftman()
//     {
//         DIContainer.Register<ICraftmanUI>(mCraftmanUI);
//
//         var reinforcement_presenter = DIContainer.Resolve<ForgePresenter>();
//         var inventory_presenter = DIContainer.Resolve<CraftmanDeckInvenPresenter>();
//         var craftman_presenter = new CraftmanPresenter(mCraftmanUI,
//                                                        inventory_presenter);
//         DIContainer.Register<CraftmanPresenter>(craftman_presenter);
//
//         reinforcement_presenter.Inject(craftman_presenter,
//                                        inventory_presenter);
//     }
// }
