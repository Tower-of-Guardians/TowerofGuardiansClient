using UnityEngine;

public class CraftmanDialogueBubblePresenter : DialogueBubblePresenter
{
    public CraftmanDialogueBubblePresenter(IDialogueBubbleView view) 
        : base(view)
    {}

    public void OpenUI(CardInventoryPresenter inventory_presenter)

        => OpenUI();

    public void CloseUI(CardInventoryPresenter inventory_presenter)
        => CloseUI();

    public void UpdateDefaultBubble()
        => SetBubble("어떤 카드를 강화해줄까?");

    public void UpdateSelectedBubble(CardData card_data)
        => SetBubble("그래, 이 카드를 강화하고 싶다고?\n어떻게 해줄까?");

    public void UpdateEnforcedBubble()
        => SetBubble("열심히 해.");
}
