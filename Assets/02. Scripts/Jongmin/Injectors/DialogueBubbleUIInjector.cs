using UnityEngine;

public class DialogueBubbleUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("상인 말풍선 UI")]
    [SerializeField] private MerchantDialogueBubbleView m_merchant_dialogue_bubble_view;

    [Header("대장장이 말풍선 UI")]
    [SerializeField] private CraftmanDialogueBubbleView m_craftman_dialogue_buble_view;

    public void Inject()
    {
        InjectMerchantBubble();
        InjectCraftmanBubble();
    }

    private void InjectMerchantBubble()
    {
        var merchant_dialogue_bubble_presenter = new MerchantDialogueBubblePresenter(m_merchant_dialogue_bubble_view);
        DIContainer.Register<MerchantDialogueBubblePresenter>(merchant_dialogue_bubble_presenter);
    }

    private void InjectCraftmanBubble()
    {
        var craftman_dialogue_bubble_presenter = new CraftmanDialogueBubblePresenter(m_craftman_dialogue_buble_view);
        DIContainer.Register<CraftmanDialogueBubblePresenter>(craftman_dialogue_bubble_presenter);
    }
}
