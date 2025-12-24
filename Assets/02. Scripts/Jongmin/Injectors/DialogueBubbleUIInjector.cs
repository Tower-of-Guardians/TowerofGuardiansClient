using UnityEngine;

public class DialogueBubbleUIInjector : MonoBehaviour, IInjector
{
    [Header("상인 말풍선 UI")]
    [SerializeField] private MerchantDialogueBubbleView m_merchant_dialogue_bubble_view;

    public void Inject()
    {
        InjectMerchantBubble();
    }

    private void InjectMerchantBubble()
    {
        var merchant_dialogue_bubble_presenter = new MerchantDialogueBubblePresenter(m_merchant_dialogue_bubble_view);
        DIContainer.Register<MerchantDialogueBubblePresenter>(merchant_dialogue_bubble_presenter);
    }
}
