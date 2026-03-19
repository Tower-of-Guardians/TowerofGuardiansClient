using UnityEngine;
using UnityEngine.Serialization;

// public class DialogueBubbleUIInjector : MonoBehaviour, IInjector
// {
//     [FormerlySerializedAs("m_merchant_dialogue_bubble_view")]
//     [Header("의존성 목록")]
//     [Header("상인 말풍선 UI")]
//     [SerializeField] private MerchantDialogueBubbleUI mMerchantDialogueBubbleUI;
//
//     [FormerlySerializedAs("mCraftmanDialogueBubleUI")]
//     [FormerlySerializedAs("m_craftman_dialogue_buble_view")]
//     [Header("대장장이 말풍선 UI")]
//     [SerializeField] private DialogueBubbleUIBase mDialogueBubleUIBase;
//
//     public void Inject()
//     {
//         InjectMerchantBubble();
//         InjectCraftmanBubble();
//     }
//
//     private void InjectMerchantBubble()
//     {
//         var merchant_dialogue_bubble_presenter = new MerchantDialogueBubblePresenter(mMerchantDialogueBubbleUI);
//         DIContainer.Register<MerchantDialogueBubblePresenter>(merchant_dialogue_bubble_presenter);
//     }
//
//     private void InjectCraftmanBubble()
//     {
//         var craftman_dialogue_bubble_presenter = new CraftmanDialogueBubblePresenter(mDialogueBubleUIBase);
//         DIContainer.Register<CraftmanDialogueBubblePresenter>(craftman_dialogue_bubble_presenter);
//     }
// }
