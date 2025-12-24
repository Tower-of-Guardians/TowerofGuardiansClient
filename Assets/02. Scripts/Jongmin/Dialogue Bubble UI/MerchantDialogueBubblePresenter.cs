public class MerchantDialogueBubblePresenter : DialogueBubblePresenter
{
    public MerchantDialogueBubblePresenter(IDialogueBubbleView view) 
        : base(view)
    {}

    public void OpenUI(CardInventoryPresenter inventory_presenter)
    {
        inventory_presenter.OnSelectedCardsChanged += UpdateSelectedCards;
        OpenUI();
    }

    public void CloseUI(CardInventoryPresenter inventory_presenter)
    {
        inventory_presenter.OnSelectedCardsChanged -= UpdateSelectedCards;
        CloseUI();
    }

    private void UpdateSelectedCards(int current_card_count, int max_card_count, int total_money)
    {
        if(current_card_count == 0)
        {
            SetBubble($"<color=#99CCFF>{max_card_count}</color>장까지 구매해줄 수 있다.");
        }
        else
        {
            if(total_money != 0)
                SetBubble($"총 <color=#99CCFF>{current_card_count}</color>장을 판매하겠다고?\n전부 합해서 <color=#99CCFF>{total_money}G</color>에 구매하도록 하지.");
            else
                SetBubble($"미안하지만 그 카드는 돈을 줄 수 없어.\n쓸모 없으면 내가 대신 버려주도록 하지.");
        }
    }
}
