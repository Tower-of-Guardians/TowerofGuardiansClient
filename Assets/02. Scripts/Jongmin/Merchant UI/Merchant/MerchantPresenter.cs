public class MerchantPresenter
{
    private readonly IMerchantView m_view;
    private readonly MerchantShopPresenter m_shop_presenter;
    private readonly IDialogueUI m_dialogue_ui;

    public MerchantPresenter(IMerchantView view,
                             MerchantShopPresenter shop_presenter,
                             IDialogueUI dialogue_ui)
    {
        m_view = view;
        m_shop_presenter = shop_presenter;
        m_dialogue_ui = dialogue_ui;
    
        m_view.Inject(this);
    }

    public void OpenUI()
    {
        SubscribeDialogueEvent();

        m_view.OpenUI();
    }

    public void CloseUI()
    {
        UnsubscribeDialogueEvent();

        CloseShop();
        m_view.CloseUI();
    }

    public void ToggleDialogue(bool active)
    {
        if(active)
            m_dialogue_ui.StartDialogue("DefaultMerchant");
        else
            m_dialogue_ui.StopDialogue();
    }

    private void OpenShop()
        => m_shop_presenter.OpenUI();

    private void CloseShop()
        => m_shop_presenter.CloseUI();


    private void SubscribeDialogueEvent()
    {
        m_dialogue_ui.InitPortrait(CharacterCode.Eccliss, CharacterCode.Kravian);
        
        (m_dialogue_ui as YarnDialogueUI).Runner.onDialogueComplete.AddListener(OpenShop);
        (m_dialogue_ui as YarnDialogueUI).Runner.onDialogueStart.AddListener(CloseShop);
    }

    private void UnsubscribeDialogueEvent()
    {
        (m_dialogue_ui as YarnDialogueUI).Runner.onDialogueComplete.RemoveListener(OpenShop);
        (m_dialogue_ui as YarnDialogueUI).Runner.onDialogueStart.RemoveListener(CloseShop);
    }
}
